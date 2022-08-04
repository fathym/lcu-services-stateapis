using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using System;

namespace Fathym.LCU.Services.StateAPIs.StateServices
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter)]
    public class StateServiceAttribute : Attribute
    {
        public virtual HttpTransportType Transport { get; set; }

        //[AppSetting(Default = "STATE_SERVICE_URL")]
        [AutoResolve]
        public virtual string URL { get; set; }

        public StateServiceAttribute()
        {
            Transport = HttpTransportType.WebSockets;
        }

        public string GetURL()
        {
            return Environment.GetEnvironmentVariable(URL) ?? URL;
        }
    }
}