using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using System;

namespace Fathym.LCU.Services.StateAPIs.StateServices
{
    public interface IStateServiceFactory
    {
        public IStateService CreateStateService(string url, HttpTransportType transport, Type type = null);
    }
}