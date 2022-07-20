using LCU.Personas.StateAPI;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LCU.Personas.StateAPI.TestHub.State
{
    public class TestStateHub : LCUStateHub<TestStateEntity>
    {
        #region Life Cycle
        [FunctionName(nameof(TestStateHub_Negotiate))]
        public virtual Task<SignalRConnectionInfo> TestStateHub_Negotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous, Route = "TestStateHub/negotiate")] HttpRequestMessage req)
        {
            return negotiate(req);
        }

        [FunctionName(nameof(OnConnected))]
        public virtual Task OnConnected([SignalRTrigger] InvocationContext invocationContext,
            [DurableClient] IDurableEntityClient client, ILogger logger)
        {
            return connected(invocationContext, client, logger);
        }

        [FunctionName(nameof(OnDisconnected))]
        public virtual Task OnDisconnected([SignalRTrigger] InvocationContext invocationContext,
            [DurableClient] IDurableEntityClient client, ILogger logger)
        {
            return disconnected(invocationContext, client, logger);
        }

        [FunctionName($"{nameof(AttachState)}")]
        public virtual async Task AttachState([SignalRTrigger] InvocationContext invocationContext,
            [DurableClient] IDurableEntityClient client, ILogger logger, string stateKey)
        {
            await attachState(invocationContext, stateKey, client, logger);
        }

        [FunctionName($"{nameof(UnattachState)}")]
        public virtual async Task UnattachState([SignalRTrigger] InvocationContext invocationContext,
            ILogger logger, string stateKey)
        {
            await unattachState(invocationContext, stateKey, logger);
        }
        #endregion

        #region API Methods
        [FunctionName($"{nameof(TestStateHub_Broadcast)}")]
        public virtual async Task TestStateHub_Broadcast([SignalRTrigger] InvocationContext invocationContext,
            [DurableClient] IDurableEntityClient client, ILogger logger)
        {
            await loadAndUpdateState(client, "test-state");
        }
        #endregion

        #region Helpers
        #endregion
    }
}