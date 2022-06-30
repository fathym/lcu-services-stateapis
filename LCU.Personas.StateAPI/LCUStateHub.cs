using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace LCU.Personas.StateAPI
{
    public class LCUStateHub<TStateEntity> : ServerlessHub
    {
        #region Helpers
        protected async Task connected(InvocationContext invocationContext, IDurableEntityClient client, ILogger logger)
        {
            logger.LogInformation($"{invocationContext.ConnectionId} has connected");

            var entLookup = invocationContext.Claims["lcu-ent-lookup"];

            await Groups.AddToGroupAsync(invocationContext.ConnectionId, entLookup);

            await loadAndUpdateState(client, entLookup);
        }

        protected virtual async Task handleStateEmpty(IDurableEntityClient client, EntityId entityId)
        { }

        protected async Task<TStateEntity> loadAndUpdateState(IDurableEntityClient client, string stateKey)
        {
            var state = await loadState(client, stateKey);

            await Clients.Groups(stateKey).SendAsync("state-update", state);

            return state;
        }

        protected virtual async Task<TStateEntity> loadState(IDurableEntityClient client, string stateKey)
        {
            var entityId = new EntityId(typeof(TStateEntity).Name, stateKey);

            var state = await client.ReadEntityStateAsync<TStateEntity>(entityId);

            if (state.EntityState == null)
            {
                await handleStateEmpty(client, entityId);

                state = await client.ReadEntityStateAsync<TStateEntity>(entityId);
            }

            return state.EntityState;
        }
        #endregion
    }
}
