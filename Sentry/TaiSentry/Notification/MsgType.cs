using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaiSentry.Notification
{
    /// <summary>
    /// 通知消息类型
    /// </summary>
    public enum MsgType
    {
        /// <summary>
        /// 系统消息，如：服务启动、服务停止、连接成功、服务异常等
        /// </summary>
        System,
        /// <summary>
        /// 用户状态消息
        /// </summary>
        Status,
        /// <summary>
        /// APP计时数据
        /// </summary>
        AppData,
        /// <summary>
        /// 焦点信息
        /// </summary>
        ActiveData,
    }
}
