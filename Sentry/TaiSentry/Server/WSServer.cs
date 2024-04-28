using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;
using WebSocketSharp;
using Newtonsoft.Json;
using System.Diagnostics;

namespace TaiSentry.Server
{
    /// <summary>
    /// WebSocket服务端，用于接收客户端的连接、消息发送
    /// </summary>
    public class WSServer : WebSocketBehavior, IWSServer
    {
        private WebSocketServer _webSocket;
        private bool _isStart = false;
        private bool _isConnected = false;
        public bool IsConnected => _isConnected;

        public WSServer()
        {
            IgnoreExtensions = true;
        }
        public void Start()
        {
            if (_isStart) return;
            try
            {
                var path = StartupParams.Get("wspath");
                var port = StartupParams.Get("wsport");
                _webSocket = new WebSocketServer(string.IsNullOrEmpty(port) ? 21123 : int.Parse(port), false);
                _webSocket.AddWebSocketService<WSServer>(string.IsNullOrEmpty(path) ? "/TaiSentry" : $"/{path}");
                _webSocket.Start();
                _isStart = true;

                Debug.WriteLine("WebSocket服务已启动，端口号： " + _webSocket.Port + "" + _webSocket.WebSocketServices.Paths.First());
            }
            catch (Exception ex)
            {
            }
        }

        public void Stop()
        {
            if (!_isStart) return;
            _webSocket?.Stop();
            _isStart = false;
        }

        public void SendMsg(string msg_)
        {
            try
            {
                if (!_isStart) return;
                _webSocket.WebSocketServices.Broadcast(msg_);
                Debug.WriteLine("发送WS MSG：" + msg_);
            }
            catch (Exception ec)
            {
            }
        }


        protected override void OnMessage(MessageEventArgs e)
        {
            try
            {
                WSServerEvent.InvokeOnMsgReceived(this);
                Debug.WriteLine("收到消息：" + ID + " - " + e.ToString());
                //Sessions.CloseSession(ID);
                //var log = JsonConvert.DeserializeObject<NotifyWeb>(e.Data);
            }
            catch
            {

            }
        }

        protected override void OnOpen()
        {
            base.OnOpen();

            ClearSessions();
            _isConnected = true;
            WSServerEvent.InvokeOnClientConnected(this);
            Sessions.TryGetSession(ID, out var session);
            Debug.WriteLine("连接成功" + ID + "," + session.Context.Host);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            _isConnected = false;
            WSServerEvent.InvokeOnClientDisconnected(this);
            Debug.WriteLine("断开连接", e.ToString());
            Debug.WriteLine("当前连接数", Sessions.IDs.Count());
        }

        #region 清理多个连接
        /// <summary>
        /// 清理多个连接，只允许一个订阅端
        /// </summary>
        private void ClearSessions()
        {
            Sessions.IDs.ToList().ForEach(id =>
            {
                if (id != ID)
                {
                    Debug.WriteLine("Remove session:" + id);
                    Sessions.CloseSession(id);
                }
            });
        }
        #endregion
    }
}
