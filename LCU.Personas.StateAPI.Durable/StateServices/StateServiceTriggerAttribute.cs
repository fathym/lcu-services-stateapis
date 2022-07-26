using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Azure.WebJobs.Description;
using System;

namespace LCU.Personas.StateAPI.StateServices
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter)]
    public class StateServiceTriggerAttribute : StateServiceAttribute//Attribute
    {
        public virtual string ClientType { get; set; }

        //[AutoResolve]
        //public virtual string StateKey { get; set; }

        //public virtual HttpTransportType Transport { get; set; }

        ////[AppSetting(Default = "STATE_SERVICE_URL")]
        //[AutoResolve]
        //public virtual string URL { get; set; }

        //public StateServiceTriggerAttribute()
        //{
        //    Transport = HttpTransportType.WebSockets;
        //}
    }
}