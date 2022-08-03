using Fathym;
using System;

namespace LCU.Personas.StateAPI.StateServices
{
    public class StateEventArgs : EventArgs
    {
        public virtual MetadataModel State { get; set; }
    }
}