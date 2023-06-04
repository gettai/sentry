using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaiSentry.AppObserver.Models;
using TaiSentry.StateObserver.Enums;

namespace TaiSentry.StateObserver.Events
{
    public class StateChangedEventArgs
    {
        public StateType Status { get; }
        public StateChangedEventArgs(StateType status_)
        {
            Status = status_;
        }

        public override string ToString()
        {
            return $"Status:{Status}";
        }
    }
}
