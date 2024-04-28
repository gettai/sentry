using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaiSentry.AppObserver.Events;
using TaiSentry.Server;
using WebSocketSharp.Server;

public delegate void WSServerEvenetHandler(IWSServer sender);

namespace TaiSentry.Server
{
    public class WSServerEvent : WebSocketBehavior
    {
        public static event WSServerEvenetHandler OnClientConnected;
        public static event WSServerEvenetHandler OnClientDisconnected;
        public static event WSServerEvenetHandler OnMsgReceived;

        public static void InvokeOnClientConnected(IWSServer sender)
        {
            OnClientConnected?.Invoke(sender);
        }

        public static void InvokeOnClientDisconnected(IWSServer sender)
        {
            OnClientDisconnected?.Invoke(sender);
        }

        public static void InvokeOnMsgReceived(IWSServer sender)
        {
            OnMsgReceived?.Invoke(sender);
        }
    }
}