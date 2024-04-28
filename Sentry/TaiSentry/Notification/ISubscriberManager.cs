using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaiSentry.AppObserver.Events;
using TaiSentry.AppTimer.Events;
using TaiSentry.StateObserver.Enums;

namespace TaiSentry.Notification
{
    /// <summary>
    /// 订阅端管理器接口
    /// </summary>
    public interface ISubscriberManager
    {
        /// <summary>
        /// 发送状态消息
        /// </summary>
        /// <param name="state_">当前状态</param>
        void SendStatusMsg(StateType state_);
        /// <summary>
        /// 发送APP计时数据消息
        /// </summary>
        /// <param name="appData_"></param>
        void SendAppDataMsg(AppDurationUpdatedEventArgs appData_);
        /// <summary>
        /// 发送系统消息
        /// </summary>
        /// <param name="system_"></param>
        void SendSystemMsg(SystemMsgType system_);
        /// <summary>
        /// 发送焦点数据信息
        /// </summary>
        /// <param name="activeData_"></param>
        void SendActiveDataMsg(AppActiveChangedEventArgs activeData_);
    }
}
