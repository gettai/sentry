using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaiSentry.Notification
{
    public class MsgJsonModel
    {
        public string Type { get; set; }
        public string Msg { get; set; }
        public long CreateTime { get; set; }
    }
}
