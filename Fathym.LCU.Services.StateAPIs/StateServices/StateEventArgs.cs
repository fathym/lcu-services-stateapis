using Fathym;
using System;

namespace Fathym.LCU.Services.StateAPIs.StateServices
{
    public class StateEventArgs : EventArgs
    {
        public virtual MetadataModel State { get; set; }

        public virtual string StateKey { get; set; }

        public virtual string StateType { get; set; }
    }
}