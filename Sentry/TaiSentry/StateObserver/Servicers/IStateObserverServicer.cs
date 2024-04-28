using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaiSentry.StateObserver.Enums;

namespace TaiSentry.StateObserver.Servicers
{
    /// <summary>
    /// 状态观察服务接口
    /// </summary>
    public interface IStateObserverServicer
    {
        /// <summary>
        /// 当前状态
        /// </summary>
        StateType Status { get; }
        /// <summary>
        /// 当状态变更时触发
        /// </summary>
        event StateEventHandler OnStateChanged;
        void Start();
        void Stop();
    }
}
