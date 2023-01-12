using Fathym.LCU.Services.StateAPIs.Durable;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Fathym.LCU.Services.StateAPIs.TestHub.State
{
    public interface ITestEntityStoreActions : IStateEntityStoreActions
    {
        Task SetTest(string test);
    }

    public class TestEntityStore : StateEntityStore<TestEntityStore>, ITestEntityStoreActions
    {
        #region Fields
        #endregion

        #region Properties
        public virtual string Test { get; set; }
        #endregion

        #region Constructors
        public TestEntityStore(ILogger<TestEntityStore> logger = null)
            : base(logger)
        {
            Test = "Hello World";
        }
        #endregion

        #region API Methods
        [FunctionName(nameof(TestEntityStore))]
        public async Task TestEntityStore_Run([EntityTrigger] IDurableEntityContext ctx, [SignalR(HubName = nameof(TestStateActions))] IAsyncCollector<SignalRMessage> messages)
        {
            await executeStateEntityLifeCycle(ctx, messages, loadInitialState);
        }

        #region Entity Actions
        public virtual async Task SetTest(string test)
        {
            Test = test;
        }
        #endregion
        #endregion

        #region Helpers
        protected virtual async Task<TestEntityStore> loadInitialState()
        {
            return new TestEntityStore();
        }
        #endregion
    }
}