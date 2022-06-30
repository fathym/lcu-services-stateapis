using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;

namespace LCU.Personas.StateAPI.StateServices
{
    public interface IStateServiceFactory
    {
        public IStateService CreateStateService(string entLookup, string url, HttpTransportType transport);
    }
}