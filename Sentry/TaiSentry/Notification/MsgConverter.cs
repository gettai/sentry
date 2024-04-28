using Newtonsoft.Json;
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
    public class MsgConverter
    {
        public static string SystemToJson(SystemMsgType type_)
        {
            return Msg(MsgType.Status, type_.ToString());
        }
        public static string StatusToJson(StateType state_)
        {
            return Msg(MsgType.Status, state_.ToString());
        }

        public static string AppDataToJson(AppDurationUpdatedEventArgs appData_)
        {
            return Msg(MsgType.AppData, JsonConvert.SerializeObject(appData_));
        }
        public static string ActiveDataToJson(AppActiveChangedEventArgs activeData_)
        {
            return Msg(MsgType.ActiveData, JsonConvert.SerializeObject(activeData_));
        }

        private static string Msg(MsgType type_, string msg_)
        {
            var msg = new MsgJsonModel();
            msg.Type = type_.ToString();
            msg.Msg = msg_;
            msg.CreateTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            return JsonConvert.SerializeObject(msg);
        }
    }
}
