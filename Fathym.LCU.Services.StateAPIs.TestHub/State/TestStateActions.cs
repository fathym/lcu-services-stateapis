using Fathym.API;
using Fathym.LCU.Services.StateAPIs.Durable;
using Fathym.LCU.Services.StateAPIs.StateServices;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fathym.LCU.Services.StateAPIs.TestHub.State
{
    public class AddGroupRequest : StateRequest
    {
        public virtual string Group { get; set; }
    }

    public class BroadcastRequest : StateRequest
    { }

    public class SetTestRequest : StateRequest
    {
        public virtual string Test { get; set; }
    }

    public class TestStateActions : StateActions
    {
        #region Constructors
        public TestStateActions()
            : base()
        { }
        #endregion

        #region Life Cycle
        [FunctionName(nameof(TestStateActions_Negotiate))]
        public virtual Task<SignalRConnectionInfo> TestStateActions_Negotiate(ILogger logger, [HttpTrigger(AuthorizationLevel.Anonymous, Route = "negotiate")] HttpRequestMessage req)
        {
            return negotiate(logger, req);
        }

        [FunctionName(nameof(OnConnected))]
        public virtual Task OnConnected(ILogger logger, [SignalRTrigger] InvocationContext invocationContext, [DurableClient] IDurableEntityClient client)
        {
            return connected(logger, invocationContext, client);
        }

        [FunctionName(nameof(OnDisconnected))]
        public virtual Task OnDisconnected(ILogger logger, [SignalRTrigger] InvocationContext invocationContext,
            [DurableClient] IDurableEntityClient client)
        {
            return disconnected(logger, invocationContext, client);
        }

        [FunctionName($"{nameof(AttachState)}")]
        public virtual async Task AttachState(ILogger logger, [SignalRTrigger] InvocationContext invocationContext, [DurableClient] IDurableEntityClient client, string stateType, string stateKey)
        {
            if (stateType == "TestEntityStore")
                await attachState<TestEntityStore>(logger, invocationContext, client, stateKey);
            else if (stateType == "TestGroupEntityStore")
                await attachState<TestGroupEntityStore>(logger, invocationContext, client, stateKey);
        }

        [FunctionName($"{nameof(UnattachState)}")]
        public virtual async Task UnattachState(ILogger logger, [SignalRTrigger] InvocationContext invocationContext, [DurableClient] IDurableEntityClient client, string stateType, string stateKey)
        {
            if (stateType == "TestEntityStore")
                await unattachState<TestEntityStore>(logger, invocationContext, client, stateKey);
            else if (stateType == "TestGroupEntityStore")
                await unattachState<TestGroupEntityStore>(logger, invocationContext, client, stateKey);
        }
        #endregion

        #region State Actions
        [FunctionName($"{nameof(AddGroup)}")]
        public virtual async Task AddGroup(ILogger logger, [SignalRTrigger] InvocationContext invocationContext,
            [DurableClient] IDurableOrchestrationClient orchClient, AddGroupRequest request)
        {
            await startOrchestration(logger, orchClient, nameof(AddGroupOrchestrator), instanceId: request.StateKey, input: request.Group);
        }

        [FunctionName($"{nameof(Broadcast)}")]
        public virtual async Task Broadcast(ILogger logger, [SignalRTrigger] InvocationContext invocationContext,
            [DurableClient] IDurableEntityClient client, BroadcastRequest request)
        {
            if (request.StateType == "TestEntityStore")
                await loadAndUpdateState<TestEntityStore>(logger, client, request.StateKey);
            else if (request.StateType == "TestGroupEntityStore")
                await loadAndUpdateState<TestGroupEntityStore>(logger, client, request.StateKey);
        }

        [FunctionName($"{nameof(SetTest)}")]
        public virtual async Task SetTest(ILogger logger, [SignalRTrigger] InvocationContext invocationContext,
            [DurableClient] IDurableOrchestrationClient orchClient, SetTestRequest request)
        {
            await startOrchestration(logger, orchClient, nameof(SetTestOrchestrator), instanceId: request.StateKey, input: request.Test);
        }

        #region Rest Actions
        #region Routes
        private const string getStateRoute = $"{nameof(TestEntityStore)}/{{stateType}}/{{stateKey}}";
        #endregion

        [FunctionName($"GetState")]
        public virtual async Task<HttpResponseMessage> GetState(ILogger logger, [HttpTrigger(AuthorizationLevel.Function, "get", Route = getStateRoute)] HttpRequestMessage req, [DurableClient] IDurableEntityClient client, [SignalR(HubName = nameof(TestStateActions))] IAsyncCollector<SignalRMessage> signalRMessages, string stateType, string stateKey)
        {
            return await withAPIBoundary<BaseResponse<MetadataModel>>(logger, req, async (response) =>
            {
                if (stateType == "TestEntityStore")
                    response.Model = (await loadAndUpdateState<TestEntityStore>(logger, client, stateKey)).JSONConvert<MetadataModel>();
                else if (stateType == "TestGroupEntityStore")
                    response.Model = (await loadAndUpdateState<TestGroupEntityStore>(logger, client, stateKey)).JSONConvert<MetadataModel>();

                response.Status = response.Model != null ? Status.Success : Status.NotLocated;

                return response;
            }).Run();
        }
        #endregion

        #region Action Orchestrations
        #region Add Group Orchestration
        [FunctionName($"{nameof(AddGroupOrchestrator)}")]
        public virtual async Task AddGroupOrchestrator(ILogger logger, [OrchestrationTrigger] IDurableOrchestrationContext context, [SignalR(HubName = nameof(TestStateActions))] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var group = context.GetInput<string>();

            var store = context.CreateEntityProxy<TestGroupEntityStore, ITestGroupEntityStoreActions>();

            await store.AddGroup(group);
        }
        #endregion

        #region Set Test Orchestration
        [FunctionName($"{nameof(SetTestOrchestrator)}")]
        public virtual async Task SetTestOrchestrator(ILogger logger, [OrchestrationTrigger] IDurableOrchestrationContext context, [SignalR(HubName = nameof(TestStateActions))] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var test = context.GetInput<string>();

            var store = context.CreateEntityProxy<ITestEntityStoreActions>(new EntityId(typeof(TestEntityStore).Name, context.InstanceId));

            await store.SetTest(test);
        }
        #endregion

        #region Activity Helpers
        #endregion
        #endregion
        #endregion

        #region Helpers
        #endregion
    }
}