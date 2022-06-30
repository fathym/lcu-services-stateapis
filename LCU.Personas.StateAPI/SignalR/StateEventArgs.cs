using Fathym;
using System;

namespace LCU.Personas.StateAPI.SignalRClient
{
    public class StateEventArgs : EventArgs
    {
        public virtual MetadataModel State { get; set; }
    }
}