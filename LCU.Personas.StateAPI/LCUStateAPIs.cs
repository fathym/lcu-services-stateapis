using Fathym;
using Fathym.API;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LCU.Personas.StateAPI
{
    public class LCUStateAPIs<TStateEntity> : LCUStateAPI
    {
        #region Constructors
        public LCUStateAPIs(ILogger logger)
            : base(logger)
        { }
        #endregion

        #region Helpers
        public virtual async Task<HttpResponseMessage> getState(ILogger log, HttpRequestMessage req,
            IDurableEntityClient client, IAsyncCollector<SignalRMessage> signalRMessages, string stateKey)
        {
            return await withAPIBoundary<BaseResponse<TStateEntity>>(req, async (response) =>
            {
                var entityId = new EntityId(typeof(TStateEntity).Name, stateKey);

                var state = await client.ReadEntityStateAsync<TStateEntity>(entityId);

                response.Model = state.EntityState;

                response.Status = response.Model != null ? Status.Success : Status.NotLocated;

                await signalStateUpdate(client, signalRMessages, stateKey);

                return response;
            }).Run();
        }

        protected virtual async Task signalStateUpdate(IDurableEntityClient client, IAsyncCollector<SignalRMessage> signalRMessages,
            string stateKey, string target = "state-update")
        {
            var entityId = new EntityId(typeof(TStateEntity).Name, stateKey);

            var state = await client.ReadEntityStateAsync<TStateEntity>(entityId);

            var stateMeta = state.EntityState.JSONConvert<MetadataModel>() ?? new MetadataModel();

            stateMeta.Metadata["$stateKey"] = stateKey;

            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    GroupName = stateKey,
                    Target = target,
                    Arguments = new object[] { stateMeta }
                });
        }
        #endregion
    }
}
