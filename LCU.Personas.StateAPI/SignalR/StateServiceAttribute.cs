using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using System;

namespace LCU.Personas.StateAPI.SignalRClient
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter)]
    public class StateServiceAttribute : Attribute
    {
        public virtual string EnterpriseLookup { get; set; }

        public virtual HttpTransportType Transport { get; set; }

        public virtual string URL { get; set; }

        public StateServiceAttribute()
        {
            Transport = HttpTransportType.WebSockets;
        }

        internal string GetEnterpriseLookup()
        {
            return Environment.GetEnvironmentVariable(EnterpriseLookup) ?? EnterpriseLookup;
        }

        internal string GetURL()
        {
            return Environment.GetEnvironmentVariable(URL) ?? URL;
        }
    }
}