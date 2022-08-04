using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Fathym.API;
using Fathym.LCU.Services.StateAPIs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Fathym;
using Fathym.LCU.Services.StateAPIs.Durable;

namespace Fathym.LCU.Services.StateAPIs.TestHub.State
{
    public class TestStateAPIs : LCUStateAPIs<TestStateEntity>
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public TestStateAPIs(ILogger<TestStateAPIs> logger)
            : base(logger)
        { }
        #endregion

        #region API Methods
        #region Routes
        private const string getStateRoute = $"{nameof(TestStateEntity)}/" + "{testId}";
        #endregion

        [FunctionName($"{nameof(TestStateAPIs)}_GetState")]
        public virtual async Task<HttpResponseMessage> EaCStateGetStateAPI(ILogger log,
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = getStateRoute)] HttpRequestMessage req,
            [DurableClient] IDurableEntityClient client,
            [SignalR(HubName = nameof(TestStateHub))] IAsyncCollector<SignalRMessage> signalRMessages, string testId)
        {
            return await getState(log, req, client, signalRMessages, testId);
        }
        #endregion

    }
}