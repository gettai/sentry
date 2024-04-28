using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TaiSentry.Server;
using Timer = System.Timers.Timer;

namespace TaiSentry.Servicer
{
    /// <summary>
    /// WebSocket服务器管理服务，用于监听订阅端连接状态，在超时未连接时终止Tai Sentry进程
    /// </summary>
    public class WSServerManagerServicer : IWSServerManagerServicer
    {
        private readonly IWSServer _wSServer;
        //  订阅端连接超时时间（毫秒）
        private readonly int _OutTime = 60000;

        private bool _isRunning = false;
        private Timer _timer;
        private bool _isTimerRunning = false;
        public WSServerManagerServicer(IWSServer wSServer_)
        {
            _wSServer = wSServer_;
        }

        public void Start()
        {
            if (_isRunning) return;

            WSServerEvent.OnClientConnected += _wSServer_OnClientConnected; ;
            WSServerEvent.OnClientDisconnected += _wSServer_OnClientDisconnected;
            StartTimer();
            _isRunning = true;
        }



        public void Stop()
        {
            if (!_isRunning) return;

            WSServerEvent.OnClientConnected -= _wSServer_OnClientConnected; ;
            WSServerEvent.OnClientDisconnected -= _wSServer_OnClientDisconnected;
            StopTimer();
            _isRunning = false;
        }

        private void StartTimer()
        {
            if (_isTimerRunning) return;
            _timer = new Timer();
            _timer.Interval = _OutTime;
#if DEBUG
            _timer.Interval = 10000;
#endif
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
            _isTimerRunning = true;
            Debug.WriteLine("等待客户端连接中...");
        }

        private void StopTimer()
        {
            _timer?.Stop();
            _isTimerRunning = false;
        }

        private void _timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            StopTimer();
            Handle();
        }

        private void Handle()
        {
            if (!_wSServer.IsConnected)
            {
                //   没有连接时终止Tai Sentry进程
                Environment.Exit(0);
            }
        }

        private void _wSServer_OnClientConnected(IWSServer sender)
        {
            Debug.WriteLine("客户端已连接！");
            StopTimer();
        }

        private void _wSServer_OnClientDisconnected(IWSServer sender)
        {
            StartTimer();
        }
    }
}
