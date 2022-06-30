using System;

namespace LCU.Personas.StateAPI.SignalRClient
{
    public class HubConnectionStartedEventArgs : EventArgs
    {
        public virtual DateTimeOffset Started { get; set; }
    }
}