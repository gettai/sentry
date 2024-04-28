using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaiSentry.AppObserver.Servicers;
using TaiSentry.AppTimer.Servicers;
using TaiSentry.Server;
using TaiSentry.StateObserver.Servicers;
using TaiSentry.Utils;

namespace TaiSentry.Servicer
{
    public class MainSeriver : IMainServicer
    {
        private readonly IAppObserver _appObserver;
        private readonly IAppTimerServicer _appTimerServicer;
        private readonly IStateObserverServicer _stateObserverServicer;
        private readonly IWSServer _wSServer;
        private readonly IStateManagerServicer _stateManagerServicer;
        private readonly IAppTimerManagerServicer _appTimerManagerServicer;
        private readonly IAppObserverManagerServicer _appObserverManagerServicer;
        private readonly IWSServerManagerServicer _wSServerManagerServicer;

        private bool _isRunning = false;

        public MainSeriver(
            IAppObserver appObserver_,
            IAppTimerServicer appTimerServicer_,
            IStateObserverServicer stateObserverServicer_,
            IWSServer wSServer_,
            IStateManagerServicer stateManagerServicer_,
            IAppTimerManagerServicer appTimerManagerServicer_,
            IAppObserverManagerServicer appObserverManagerServicer_,
            IWSServerManagerServicer wSServerManagerServicer_
            )
        {
            _appObserver = appObserver_;
            _appTimerServicer = appTimerServicer_;
            _stateObserverServicer = stateObserverServicer_;
            _wSServer = wSServer_;
            _stateManagerServicer = stateManagerServicer_;
            _appTimerManagerServicer = appTimerManagerServicer_;
            _appObserverManagerServicer = appObserverManagerServicer_;
            _wSServerManagerServicer = wSServerManagerServicer_;
        }

        public void Start()
        {
            if (_isRunning) return;

            //  计时服务必须比应用观察者先启动
            _appTimerServicer.Start();
            _appObserver.Start();
            _stateObserverServicer.Start();
            _wSServer.Start();
            _stateManagerServicer.Start();
            _appTimerManagerServicer.Start();
            _appObserverManagerServicer.Start();
            _wSServerManagerServicer.Start();
           
            _isRunning = true;

            Debug.WriteLine("Tai Sentry启动完毕");
        }

        public void Stop()
        {
            if (!_isRunning) return;

            _appTimerServicer.Stop();
            _appObserver.Stop();
            _stateObserverServicer.Stop();
            _stateManagerServicer.Stop();
            _appTimerManagerServicer.Stop();
            _appObserverManagerServicer.Stop();
            _wSServerManagerServicer.Stop();

            _isRunning = false;

        }
    }
}
