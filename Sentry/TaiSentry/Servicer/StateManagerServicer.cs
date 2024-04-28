using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaiSentry.AppObserver.Servicers;
using TaiSentry.AppTimer.Servicers;
using TaiSentry.Notification;
using TaiSentry.StateObserver.Enums;
using TaiSentry.StateObserver.Servicers;

namespace TaiSentry.Servicer
{
    /// <summary>
    /// 状态管理服务，用于监听用户是否离开电脑
    /// 并在状态切换时管理计时器和APP监听器服务的启停以及通知订阅端状态的更新
    /// </summary>
    public class StateManagerServicer : IStateManagerServicer
    {
        private readonly IStateObserverServicer _stateObserverServicer;
        private readonly IAppTimerServicer _appTimerServicer;
        private readonly IAppObserver _appObserver;
        private readonly ISubscriberManager _subscriberManager;

        private StateType _status;
        public StateManagerServicer(
            IStateObserverServicer stateObserverServicer_,
            IAppTimerServicer appTimerServicer_,
            IAppObserver appObserver_,
            ISubscriberManager subscriberManager_
            )
        {
            _stateObserverServicer = stateObserverServicer_;
            _appTimerServicer = appTimerServicer_;
            _appObserver = appObserver_;
            _subscriberManager = subscriberManager_;
        }

        public void Start()
        {
            _stateObserverServicer.OnStateChanged += _stateObserverServicer_OnStateChanged;
        }

        public void Stop()
        {
            _stateObserverServicer.OnStateChanged -= _stateObserverServicer_OnStateChanged;
        }

        private void _stateObserverServicer_OnStateChanged(object sender_, StateObserver.Events.StateChangedEventArgs e_)
        {
            _status = e_.Status;

            SendNotify();
            HandleAppTimerData();

            Debug.WriteLine("状态管理服务：" + e_);
        }

        #region 发送通知给订阅端
        /// <summary>
        /// 发送通知给订阅端
        /// </summary>
        private void SendNotify()
        {
            _subscriberManager.SendStatusMsg(_status);
        }
        #endregion

        #region 处理APP计时数据
        private void HandleAppTimerData()
        {
            if (_status == StateType.Active)
            {
                //  活跃时

                //  1. 启动计时器
                _appTimerServicer.Start();
                //  2. 启动APP监听
                _appObserver.Start();
            }
            else
            {
                //  离开时

                //  1. 响应当前统计数据
                _appTimerServicer.InvokeAppDurationUpdated();
                //  2. 停止计时器
                _appTimerServicer.Stop();
                //  3. 停止APP监听
                _appObserver.Stop();
            }
        }
        #endregion
    }
}
