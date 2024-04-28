using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaiSentry.AppTimer.Events;
using TaiSentry.AppTimer.Servicers;
using TaiSentry.Notification;

namespace TaiSentry.Servicer
{
    /// <summary>
    /// APP计时器管理服务，用于监听计时数据并通知订阅端
    /// </summary>
    public class AppTimerManagerServicer : IAppTimerManagerServicer
    {
        private readonly IAppTimerServicer _appTimerServicer;
        private readonly ISubscriberManager _subscriberManager;

        private bool _isRunning = false;

        public AppTimerManagerServicer(
            IAppTimerServicer appTimerServicer_,
            ISubscriberManager subscriberManager_
            )
        {
            _appTimerServicer = appTimerServicer_;
            _subscriberManager = subscriberManager_;
        }

        public void Start()
        {
            if (_isRunning) return;

            _appTimerServicer.OnAppDurationUpdated += _appTimerServicer_OnAppDurationUpdated;

            _isRunning = true;
        }


        public void Stop()
        {

            if (!_isRunning) return;

            _appTimerServicer.OnAppDurationUpdated -= _appTimerServicer_OnAppDurationUpdated;

            _isRunning = false;
        }

        private void _appTimerServicer_OnAppDurationUpdated(object sender, AppDurationUpdatedEventArgs e)
        {
            SendNotify(e);
        }

        #region 发送通知给订阅端
        private void SendNotify(AppDurationUpdatedEventArgs data_)
        {
            _subscriberManager.SendAppDataMsg(data_);
            Debug.WriteLine("计时更新，发送通知给订阅端");
        }
        #endregion
    }
}
