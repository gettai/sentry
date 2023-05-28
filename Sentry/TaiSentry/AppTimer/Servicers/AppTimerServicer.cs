using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TaiSentry.AppObserver.Enums;
using TaiSentry.AppObserver.Models;
using TaiSentry.AppObserver.Servicers;

namespace TaiSentry.AppTimer.Servicers
{
    public class AppTimerServicer : IAppTimerServicer
    {
        public event AppTimerEventHandler OnAppDurationUpdated;

        private readonly IAppObserver _appObserver;

        private bool _isStart = false;
        private int _appDuration = 0;
        private DateTime _startTime = DateTime.MinValue;
        private DateTime _endTime = DateTime.MinValue;
        private string _activeProcess;

        private Dictionary<string, AppData> _appData;
        private System.Timers.Timer _timer;

        public struct AppData
        {
            public AppInfo App { get; set; }
            public WindowInfo Window { get; set; }

        }
        public AppTimerServicer(IAppObserver appObserver_)
        {
            _appObserver = appObserver_;

        }

        private void Init()
        {
            _appData = new Dictionary<string, AppData>();
            _appDuration = 0;
            _activeProcess = string.Empty;

            _timer = new System.Timers.Timer();
            _timer.Interval = 1000;
            _timer.Elapsed += Timer_Elapsed;
        }



        public void Start()
        {
            if (_isStart) { return; }

            Init();

            _isStart = true;
            _appObserver.OnAppActiveChanged += AppObserver_OnAppActiveChanged;
        }


        public void Stop()
        {
            _isStart = false;
            StopTimer();
            _appObserver.OnAppActiveChanged -= AppObserver_OnAppActiveChanged;
        }

        private void AppObserver_OnAppActiveChanged(object sender, AppObserver.Events.AppActiveChangedEventArgs e)
        {
            string processName = e.App.Process;
            AppType appType = e.App.Type;

            if (processName != _activeProcess)
            {
                StopTimer();
                InvokeEvent();

                if (!string.IsNullOrEmpty(processName) && appType != AppType.SystemComponent)
                {
                    StartTimer();
                }

                _activeProcess = processName;
            }

            if (!string.IsNullOrEmpty(processName) && appType != AppType.SystemComponent)
            {
                var data = new AppData()
                {
                    App = e.App,
                    Window = e.Window,
                };
                if (!_appData.ContainsKey(processName))
                {
                    //  缓存应用信息
                    _appData.Add(processName, data);
                }
                else
                {
                    //  更新缓存
                    _appData[processName] = data;
                }
            }
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            _appDuration++;
        }

        private void StartTimer()
        {
            _timer.Start();
            _startTime = DateTime.Now;
            _appDuration = 0;
        }

        private void StopTimer()
        {
            _timer.Stop();
            _endTime = DateTime.Now;
        }

        private void InvokeEvent()
        {
            if (_appDuration > 0 && !string.IsNullOrEmpty(_activeProcess) && _appData.ContainsKey(_activeProcess))
            {
                var data = _appData[_activeProcess];
                var args = new Events.AppDurationUpdatedEventArgs(_appDuration, data.App, data.Window, _startTime, _endTime);
                Debug.WriteLine("【计时更新】" + args);
                OnAppDurationUpdated?.Invoke(this, args);
            }
        }

    }
}
