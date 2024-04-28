using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaiSentry.Servicer
{
    /// <summary>
    /// 状态管理服务
    /// 在状态变更时给订阅者发送状态变更通知、管理APP计时服务
    /// </summary>
    public interface IStateManagerServicer : IBaseServicer
    {
    }
}
