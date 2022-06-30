using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Azure.WebJobs.Description;
using System;

namespace LCU.Personas.StateAPI.StateServices
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter)]
    public class StateServiceTriggerAttribute : StateServiceAttribute
    {
    }
}