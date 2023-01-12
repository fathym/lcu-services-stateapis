using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using System;
using System.Threading.Tasks;

namespace Fathym.LCU.Services.StateAPIs.StateServices
{
    public interface IStateActionsClientFactory
    {
        public IStateActionsClient CreateStateActionsClient(string url, HttpTransportType transport, Func<Task<string>> accessTokenProvider, Type type);
    }
}