using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaiSentry.Server
{
    /// <summary>
    /// WebSocket服务接口
    /// 用于定义WebSocket服务的基本操作
    /// </summary>
    public interface IWSServer
    {
        bool IsConnected { get; }
        void Start();
        void Stop();
        void SendMsg(string msg_);
    }
}
