using System;

namespace LCU.Personas.StateAPI.StateServices
{
    public class HubConnectionStartedEventArgs : EventArgs
    {
        public virtual DateTimeOffset Started { get; set; }
    }
}