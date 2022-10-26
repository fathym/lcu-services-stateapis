using System.Threading.Tasks;
using Fathym;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs;
using System;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Fathym.API;
using Fathym.LCU.Services.StateAPIs.TestHub.State;

namespace Fathym.LCU.Services.StateAPIs.StateServices.TestAPI.State
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
        private const string addGroupRoute = "add-group";

        private const string attachStateRoute = "attach";

        private const string setTestStateRoute = "set-test";
        #endregion

        [FunctionName($"{nameof(TestStateHandlerAPIs_AttachState)}")]
        public virtual async Task<HttpResponseMessage> TestStateHandlerAPIs_AttachState(ILogger logger, [HttpTrigger(AuthorizationLevel.Function, "post", Route = attachStateRoute)] HttpRequestMessage req, [StateService(URL = "TEST_STATE_HUB_URL")] TestAPIStateService stateSvc)
        {
            return await withAPIBoundary<StateRequest, BaseResponse>(req, async (request, response) =>
            {
                logger.LogInformation($"TestStateHandlerAPIs_AttachState => {request.StateKey}");

                await stateSvc.AttachState(request.StateType, request.StateKey);

                response.Status = Status.Success;

                return response;
            }).Run();
        }

        [FunctionName($"{nameof(TestStateHandlerAPIs_HandleBroadcast)}")]
        public virtual async Task TestStateHandlerAPIs_HandleBroadcast(ILogger logger, [StateServiceTrigger(URL = "TEST_STATE_HUB_URL")] StateEventArgs stateEvent, [StateService(URL = "TEST_STATE_HUB_URL")] TestAPIStateService stateSvc)
        {
            var state = stateEvent.State;//.FromJSON<MetadataModel>();

            logger.LogInformation(state.ToJSON());
        }

        [FunctionName($"{nameof(TestStateHandlerAPIs_SetTest)}")]
        public virtual async Task<HttpResponseMessage> TestStateHandlerAPIs_SetTest(ILogger logger, [HttpTrigger(AuthorizationLevel.Function, "post", Route = setTestStateRoute)] HttpRequestMessage req, [StateService(URL = "TEST_STATE_HUB_URL")] TestAPIStateService stateSvc)
        {
            return await withAPIBoundary<SetTestRequest, BaseResponse>(req, async (request, response) =>
            {
                await stateSvc.SetTest(request);

                response.Status = Status.Success;

                return response;
            }).Run();
        }


        [FunctionName($"{nameof(TestStateHandlerAPIs_AddGroup)}")]
        public virtual async Task<HttpResponseMessage> TestStateHandlerAPIs_AddGroup(ILogger logger, [HttpTrigger(AuthorizationLevel.Function, "post", Route = addGroupRoute)] HttpRequestMessage req, [StateService(URL = "TEST_STATE_HUB_URL")] TestAPIStateService stateSvc)
        {
            return await withAPIBoundary<AddGroupRequest, BaseResponse>(req, async (request, response) =>
            {
                await stateSvc.AddGroup(request);

                response.Status = Status.Success;

                return response;
            }).Run();
        }
        #endregion
    }
}