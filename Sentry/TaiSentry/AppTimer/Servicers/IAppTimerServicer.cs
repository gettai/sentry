using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaiSentry.AppTimer.Events;

namespace TaiSentry.AppTimer.Servicers
{
    /// <summary>
    /// 应用计时器服务（提供应用时长统计以及数据订阅）
    /// </summary>
    public interface IAppTimerServicer
    {
        /// <summary>
        /// 当有应用的计时更新时发生
        /// </summary>
        event AppTimerEventHandler OnAppDurationUpdated;

        /// <summary>
        /// 启动计时服务
        /// </summary>
        void Start();
        /// <summary>
        /// 停止计时服务
        /// </summary>
        void Stop();
        /// <summary>
        /// 重置计时（将当前计时清零）
        /// </summary>
        void Reset();
        /// <summary>
        /// 立即获取当前应用的计时数据
        /// </summary>
        AppDurationUpdatedEventArgs? GetAppDuration();
        /// <summary>
        /// 响应计时更新事件
        /// </summary>
        void InvokeAppDurationUpdated();
    }
}
