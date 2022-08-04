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
using Fathym.LCU.Services.StateAPIs.TestStates;

namespace Fathym.LCU.Services.StateAPIs.TestHub.State
{
    public class TestStateEntity : TestState, ITestState
    {
        #region Fields
        protected readonly IHttpClientFactory clientFactory;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public TestStateEntity(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
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
        public async Task Run([EntityTrigger] IDurableEntityContext ctx)
        {
            if (!ctx.HasState)
                ctx.SetState(new TestState());

            await ctx.DispatchAsync<TestStateEntity>();
        }
        #endregion

        #region Helpers
        #endregion
    }
}