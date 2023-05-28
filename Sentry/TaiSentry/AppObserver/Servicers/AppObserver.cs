using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static TaiSentry.AppObserver.Servicers.AppObserver;

namespace TaiSentry.AppObserver.Servicers
{
    public class AppObserver : IAppObserver
    {
        public event AppObserverEventHandler? OnAppActiveChanged;

        #region win32 api
        public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType,
         IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr
         hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess,
         uint idThread, uint dwFlags);
        [DllImport("user32.dll")]
        public static extern bool UnhookWinEvent(IntPtr hWinEventHook);
        #endregion

        //  获得焦点事件
        private WinEventDelegate _foregroundEventDelegate;
        private readonly IAppManager _appManager;
        private readonly IWindowManager _windowManager;
        private IntPtr _hook;
        private bool _isStart = false;
        public AppObserver(IAppManager appManager_, IWindowManager windowManager)
        {
            _appManager = appManager_;
            _windowManager = windowManager;
            _foregroundEventDelegate = new WinEventDelegate(ForegroundEventCallback);
        }

        private async void ForegroundEventCallback(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            DateTime activeTime = DateTime.Now;
            var stopwatch = Stopwatch.StartNew();
            var app = _appManager.GetAppInfo(hwnd);
            var window = _windowManager.GetWindowInfo(hwnd);
            Debug.WriteLine("【{0}ms】" + app.ToString(), stopwatch.Elapsed.TotalMilliseconds);
            Debug.WriteLine("【{0}ms】" + window.ToString(), stopwatch.Elapsed.TotalMilliseconds);
            //  响应事件
            OnAppActiveChanged?.Invoke(this, new Events.AppActiveChangedEventArgs(app, window, activeTime));
        }

        public void Start()
        {
            if (_isStart)
            {
                return;
            }
            _isStart = true;
            _hook = SetWinEventHook(0x0003, 0x0003, IntPtr.Zero, _foregroundEventDelegate, 0, 0, 0);
        }

        public void Stop()
        {
            _isStart = false;
            UnhookWinEvent(_hook);
        }
    }
}
