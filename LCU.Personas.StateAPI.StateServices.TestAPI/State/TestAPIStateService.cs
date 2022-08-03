using Microsoft.AspNetCore.Http.Connections;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Fathym;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace LCU.Personas.StateAPI.StateServices.TestAPI.State
{
    public class TestAPIStateService : StateService
    {
        #region Constructors
        public TestAPIStateService(string url, HttpTransportType transport)
            : base(url, transport)
        { }
        #endregion

        #region API Methods
        public virtual Task Broadcast(MetadataModel testModel)
        {
            return Hub.InvokeAsync("TestStateHub_Broadcast", testModel);
        }
        #endregion
    }
}