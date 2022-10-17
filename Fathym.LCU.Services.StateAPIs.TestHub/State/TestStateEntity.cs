using System;
using System.Linq;
using System.IO;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Host;
using Fathym;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Fathym.LCU.Services.StateAPIs.Durable;

namespace Fathym.LCU.Services.StateAPIs.TestHub.State
{
    public interface ITestState
    {
        Task SetTest(string test);
    }

    public class TestStateEntity : LCUStateEntity, ITestState
    {
        #region Fields
        #endregion

        #region Properties (State)
        public virtual string Test { get; set; }
        #endregion

        #region Constructors
        public TestStateEntity()
        {
            Test = "Hello World";
        }
        #endregion

        #region API Methods
        public virtual async Task SetTest(string test)
        {
            Test = test;
        }
        #endregion

        #region Life Cycle
        [FunctionName(nameof(TestStateEntity))]
        public async Task Run([EntityTrigger] IDurableEntityContext ctx,
            [SignalR(HubName = nameof(TestStateHub))] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            if (!ctx.HasState)
                ctx.SetState(new TestStateEntity());

            ctx.GetState<TestStateEntity>();

            await ctx.DispatchAsync<TestStateEntity>();
        }
        #endregion

        #region Helpers
        #endregion
    }
}