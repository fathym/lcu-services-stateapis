using Fathym.LCU.Services.StateAPIs.Durable;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fathym.LCU.Services.StateAPIs.TestHub.State
{
    public interface ITestGroupEntityStoreActions : IStateEntityStore
    {
        Task AddGroup(string group);
    }

    public class TestGroupEntityStore : StateEntityStore<TestGroupEntityStore>, ITestGroupEntityStoreActions
    {
        #region Properties (State)
        public virtual HashSet<string> Groups { get; set; }
        #endregion

        #region Constructors
        public TestGroupEntityStore(ILogger<TestGroupEntityStore> logger = null)
            : base(logger)
        {
            Groups = new HashSet<string>();
        }
        #endregion

        #region API Methods
        [FunctionName(nameof(TestGroupEntityStore))]
        public async Task TestGroupEntityStore_Run([EntityTrigger] IDurableEntityContext ctx, [SignalR(HubName = nameof(TestStateActions))] IAsyncCollector<SignalRMessage> messages)
        {
            await executeStateEntityLifeCycle(ctx, messages);
        }

        #region Entity Actions
        public virtual async Task AddGroup(string group)
        {
            Groups.Add(group);
        }
        #endregion
        #endregion

        #region Helpers
        protected override TestGroupEntityStore loadInitialState()
        {
            return new TestGroupEntityStore();
        }
        #endregion
    }
}