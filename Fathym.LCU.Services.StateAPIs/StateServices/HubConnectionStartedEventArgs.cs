using System;

namespace Fathym.LCU.Services.StateAPIs.StateServices
{
    public class HubConnectionStartedEventArgs : EventArgs
    {
        public virtual DateTimeOffset Started { get; set; }
    }
}