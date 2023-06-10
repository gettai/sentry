using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TaiSentry.StateObserver.Enums;
using TaiSentry.Utils.Win32API;
using Timer = System.Timers.Timer;
using Point = TaiSentry.Utils.Win32API.Win32InputAPI.Point;
using System.Data;
using Microsoft.Win32;

namespace TaiSentry.StateObserver.Servicers
{
    public class StateObserverServicer : IStateObserverServicer
    {
        private StateType _status = StateType.Active;
        public StateType Status => _status;
        public event StateEventHandler OnStateChanged;

        private Point _lastCursorPoint;
        private DateTime _keyboardLastTime;
        private DateTime _soundLastTime;
        private Timer _timer;
        private bool _isStart = false;
        //  鼠标钩子
        private Win32InputAPI.LowLevelKeyboardProc _mouseProc;
        private static IntPtr _hookMouseID = IntPtr.Zero;
        private IntPtr _mouseHook;
        //  键盘钩子
        private Win32InputAPI.LowLevelKeyboardProc _keyboardProc;
        private static IntPtr _hookKeyboardID = IntPtr.Zero;
        private IntPtr _keyboardHook;

        public StateObserverServicer()
        {
            _keyboardProc = HookKeyboardCallback;
            _mouseProc = HookMouseCallback;
        }

        #region public methods
        public void Start()
        {
            if (_isStart) { return; }

            Init();

            _isStart = true;
        }

        public void Stop()
        {
            if (!_isStart) { return; }

            if (_timer != null)
            {
                _timer.Stop();
                _timer.Elapsed -= Timer_Elapsed;
            }

            //  卸载钩子
            Win32InputAPI.UnhookWindowsHookEx(_keyboardHook);
            Win32InputAPI.UnhookWindowsHookEx(_mouseHook);

            SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
            SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
        }
        #endregion

        #region private methods
        private void Init()
        {
            _status = StateType.Active;
            _lastCursorPoint = Win32InputAPI.GetCursorPosition();
            _keyboardLastTime = DateTime.Now;
            _soundLastTime = DateTime.MinValue;

            //  注册键盘钩子
            _keyboardHook = Win32InputAPI.SetKeyboardHook(_keyboardProc);
            ////  鼠标钩子，用于唤醒监听
            _mouseHook = Win32InputAPI.SetMouseHook(_mouseProc);

            _timer = new Timer();
            _timer.Interval = 5 * 1000 * 60;
#if DEBUG
            _timer.Interval = 5000;
#endif
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();

            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
        }

        private void UpdateCursorPoint()
        {
            _lastCursorPoint = Win32InputAPI.GetCursorPosition();
        }

        private async void CheckStatus()
        {
            bool isInputing = await IsInputtingAsync();
            bool isDepart = false;
            if (!isInputing)
            {
                //  没有输入状态时判断声音
                bool isPlayingSound = await IsPlayingSoundAsync();
                if (isPlayingSound)
                {
                    //  在播放声音时判断超时（120分钟视为离开）
                    if (_soundLastTime != DateTime.MinValue)
                    {
                        isDepart = (DateTime.Now - _soundLastTime).TotalMinutes > 120;
                    }
                    else
                    {
                        //  首次播放
                        _soundLastTime = DateTime.Now;
                    }
                }
                else
                {
                    //  没有输入也没有声音时进入离开状态
                    isDepart = true;
                }
            }

            if (isDepart)
            {
                SetDepart();
            }
            else
            {
                _timer?.Start();
                SetActive();
            }

        }

        /// <summary>
        /// 当前是否在播放声音（持续30秒检测）
        /// </summary>
        /// <returns>播放返回true</returns>
        private async Task<bool> IsPlayingSoundAsync()
        {
            Debug.WriteLine("声音检测");
            bool result = false;
            //  持续30秒
            int time = 30;
#if DEBUG
            time = 5;
#endif
            await Task.Run(() =>
            {
                while (time > 0)
                {
                    bool isPlay = Win32AudioAPI.IsPlayingSound();
                    if (isPlay)
                    {
                        result = true;
                        break;
                    }
                    else
                    {
                        time--;
                        Thread.Sleep(1000);
                    }
                }
            });
            return result;
        }

        /// <summary>
        /// 当前是否处于输入（鼠标键盘）活跃状态（持续30秒检测）
        /// </summary>
        /// <returns>活跃返回true</returns>
        private async Task<bool> IsInputtingAsync()
        {
            Debug.WriteLine("输入检测");
            bool result = false;
            //  持续30秒
            int time = 30;
            int outTimeMinutes = 5;
#if DEBUG
            outTimeMinutes = 1;
            time = 5;
#endif
            await Task.Run(() =>
            {
                while (time > 0)
                {
                    Point cursorPoint = Win32InputAPI.GetCursorPosition();
                    bool isMouseActive = cursorPoint.ToString() != _lastCursorPoint.ToString();
                    bool isKeyboardActive = (DateTime.Now - _keyboardLastTime).TotalMinutes < outTimeMinutes;
                    if (isMouseActive || isKeyboardActive)
                    {
                        result = true;
                        break;
                    }
                    else
                    {
                        time--;
                        Thread.Sleep(1000);
                    }
                }
            });
            return result;
        }

        private void SetActive()
        {
            if (_status == StateType.Active) { return; }

            _status = StateType.Active;
            _soundLastTime = DateTime.MinValue;
            _lastCursorPoint = Win32InputAPI.GetCursorPosition();
            _keyboardLastTime = DateTime.Now;

            _timer?.Start();

            //  通知事件
            InvokeEvent();
        }

        private void SetDepart()
        {
            if (_status == StateType.Depart) { return; }

            _status = StateType.Depart;
            _soundLastTime = DateTime.MinValue;

            _timer?.Stop();

            //  通知事件
            InvokeEvent();
        }

        private void InvokeEvent()
        {
            OnStateChanged?.Invoke(this, new Events.StateChangedEventArgs(Status));
        }
        #endregion

        #region event callback
        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            _timer?.Stop();
            CheckStatus();
            UpdateCursorPoint();
        }

        private IntPtr HookMouseCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && _status == StateType.Depart)
            {
                if (wParam == (IntPtr)Win32InputAPI.WM_LBUTTONDBLCLK || wParam == (IntPtr)Win32InputAPI.WM_WHEEL)
                {
                    SetActive();
                }

            }
            return Win32InputAPI.CallNextHookEx(_hookMouseID, nCode, wParam, lParam);
        }

        private IntPtr HookKeyboardCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)Win32InputAPI.WM_KEYDOWN)
            {
                if (_status == StateType.Depart)
                {
                    SetActive();
                }
                else
                {
                    _soundLastTime = DateTime.MinValue;
                    _keyboardLastTime = DateTime.Now;
                }
            }
            return Win32InputAPI.CallNextHookEx(_hookKeyboardID, nCode, wParam, lParam);
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.RemoteDisconnect || e.Reason == SessionSwitchReason.ConsoleDisconnect)
            {
                //  与这台设备远程桌面连接断开
                SetDepart();
            }
        }

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Suspend:
                    //  电脑休眠
                    SetDepart();
                    break;
                case PowerModes.Resume:
                    //  电脑恢复
                    SetActive();
                    break;
            }
        }
        #endregion

    }
}
