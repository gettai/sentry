using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaiSentry.StateObserver.Enums;

namespace TaiSentry.StateObserver.Servicers
{
    public interface IStateObserverServicer
    {
        StateType Status { get; }
        event StateEventHandler OnStateChanged;
        void Start();
        void Stop();
    }
}
