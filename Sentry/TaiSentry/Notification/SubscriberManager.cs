using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaiSentry.AppObserver.Events;
using TaiSentry.AppTimer.Events;
using TaiSentry.Server;
using TaiSentry.StateObserver.Enums;
using static TaiSentry.AppTimer.Servicers.AppTimerServicer;

namespace TaiSentry.Notification
{
    /// <summary>
    /// 订阅端管理服务，用于处理消息通知
    /// </summary>
    public class SubscriberManager : ISubscriberManager
    {
        private readonly IWSServer _wSServer;

        public SubscriberManager(IWSServer wSServer_)
        {
            _wSServer = wSServer_;
        }

        public void SendActiveDataMsg(AppActiveChangedEventArgs activeData_)
        {
            string msg = MsgConverter.ActiveDataToJson(activeData_);
            _wSServer.SendMsg(msg);
        }

        public void SendAppDataMsg(AppDurationUpdatedEventArgs appData_)
        {
            //if (_subscriber == null) return;
            string msg = MsgConverter.AppDataToJson(appData_);
            _wSServer.SendMsg(msg);
        }

        public void SendStatusMsg(StateType state_)
        {
            //if (_subscriber == null) return;
            string msg = MsgConverter.StatusToJson(state_);
            _wSServer.SendMsg(msg);
        }

        public void SendSystemMsg(SystemMsgType msg_)
        {
            //if (_subscriber == null) return;
            string msg = MsgConverter.SystemToJson(msg_);
            _wSServer.SendMsg(msg);
        }
    }
}
