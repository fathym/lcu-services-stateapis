using Microsoft.AspNetCore.Http.Connections;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Fathym;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Fathym.LCU.Services.StateAPIs.Durable;
using Fathym.LCU.Services.StateAPIs.TestHub.State;

namespace Fathym.LCU.Services.StateAPIs.StateServices.TestAPI.State
{
    public class TestAPIStateService : StateService
    {
        #region Constructors
        public TestAPIStateService(string url, HttpTransportType transport)
            : base(url, transport)
        { }
        #endregion

        #region API Methods
        public virtual Task AddGroup(AddGroupRequest request)
        {
            return Hub.InvokeAsync("AddGroup", request);
        }

        public virtual Task Broadcast(BroadcastRequest request)
        {
            return Hub.InvokeAsync("Broadcast", request);
        }

        public virtual Task SetTest(SetTestRequest request)
        {
            return Hub.InvokeAsync("SetTest", request);
        }
        #endregion
    }
}