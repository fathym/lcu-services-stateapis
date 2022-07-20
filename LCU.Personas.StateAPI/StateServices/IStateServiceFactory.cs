using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using System;

namespace LCU.Personas.StateAPI.StateServices
{
    public interface IStateServiceFactory
    {
        public IStateService CreateStateService(string url, HttpTransportType transport, Type type = null);
    }
}