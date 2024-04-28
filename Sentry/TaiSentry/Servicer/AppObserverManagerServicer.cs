using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaiSentry.AppObserver.Servicers;
using TaiSentry.Notification;

namespace TaiSentry.Servicer
{
    public class AppObserverManagerServicer : IAppObserverManagerServicer
    {
        private readonly IAppObserver _appObserver;
        private readonly ISubscriberManager _subscriberManager;
        private bool _isRunning = false;
        public AppObserverManagerServicer(
            IAppObserver appObserver_,
            ISubscriberManager subscriberManager_
            )
        {
            _appObserver = appObserver_;
            _subscriberManager = subscriberManager_;
        }

        public void Start()
        {
            if (_isRunning) return;
            _appObserver.OnAppActiveChanged += _appObserver_OnAppActiveChanged;
            _isRunning = true;
        }

        private void _appObserver_OnAppActiveChanged(object sender, AppObserver.Events.AppActiveChangedEventArgs e)
        {
            SendNotify(e);
        }

        public void Stop()
        {
            if (!_isRunning) return;
            _appObserver.OnAppActiveChanged -= _appObserver_OnAppActiveChanged;
            _isRunning = false;
        }

        #region 发送通知给订阅端
        private void SendNotify(AppObserver.Events.AppActiveChangedEventArgs e)
        {
            //  是否订阅
            bool isSub = StartupParams.Get("activedata") != null;
            if (isSub)
            {
                _subscriberManager.SendActiveDataMsg(e);
            }
        }
        #endregion
    }
}
