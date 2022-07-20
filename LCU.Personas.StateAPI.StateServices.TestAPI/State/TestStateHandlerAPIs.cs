using System.Threading.Tasks;
using Fathym;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs;
using System;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Fathym.API;

namespace LCU.Personas.StateAPI.StateServices.TestAPI.State
{
    public class TestStateHandlerAPIs : LCUStateAPI
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public TestStateHandlerAPIs(ILogger<TestStateHandlerAPIs> logger)
            : base(logger)
        { }
        #endregion

        #region API Methods
        #region Routes
        private const string getStateRoute = $"attach";
        #endregion

        [FunctionName($"{nameof(TestStateHandlerAPIs_AttachState)}")]
        public virtual async Task<HttpResponseMessage> TestStateHandlerAPIs_AttachState(ILogger log,
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = getStateRoute)] HttpRequestMessage req,
            [StateService(URL = "TEST_STATE_HUB_URL")] TestAPIStateService stateSvc)
        {
            return await withAPIBoundary<MetadataModel, BaseResponse>(req, async (request, response) =>
            {
                var stateKey = request.Metadata["StateKey"].ToString();

                logger.LogInformation($"TestStateHandlerAPIs_AttachState => {stateKey}");

                await stateSvc.AttachState(stateKey);

                response.Status = Status.Success;

                return response;
            }).Run();
        }

        [FunctionName($"{nameof(TestStateHandlerAPIs)}_{nameof(HandleBroadcast)}")]
        public virtual async Task HandleBroadcast(ILogger log,
            [StateServiceTrigger(URL = "TEST_STATE_HUB_URL")] string stateStr,
            [StateService(URL = "TEST_STATE_HUB_URL")] TestAPIStateService stateSvc)
        {
            var state = stateStr.FromJSON<MetadataModel>();
        }
        #endregion
    }
}