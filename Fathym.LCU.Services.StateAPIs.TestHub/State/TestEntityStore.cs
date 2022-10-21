using Fathym.LCU.Services.StateAPIs.Durable;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Fathym.LCU.Services.StateAPIs.TestHub.State
{
    public interface ITestEntityStoreActions
    {
        Task SetTest(string test);
    }

    public class TestEntityStore : StateEntityStore<TestEntityStore>, ITestEntityStoreActions
    {
        #region Properties (State)
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
        public async Task TestEntityStore_Run([EntityTrigger] IDurableEntityContext ctx, [SignalR(HubName = nameof(TestStateActions))] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            await initializeStateEntity(ctx, signalRMessages);
        }

        #region Entity Actions
        public virtual async Task SetTest(string test)
        {
            Test = test;
        }
        #endregion
        #endregion

        #region Helpers
        protected override TestEntityStore loadInitialState()
        {
            return new TestEntityStore();
        }
        #endregion
    }
}