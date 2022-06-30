using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System.Threading.Tasks;

namespace LCU.Personas.StateAPI
{
    public class StateHub<TStateEntity> : ServerlessHub
    {
        #region Helpers
        protected virtual async Task handleStateEmpty(IDurableEntityClient client, EntityId entityId)
        { }

        protected async Task<TStateEntity> loadAndUpdateEaCState(IDurableEntityClient client, string stateKey)
        {
            var state = await loadEaCState(client, stateKey);

            await Clients.Groups(stateKey).SendAsync("state-update", state);

            return state;
        }

        protected virtual async Task<TStateEntity> loadEaCState(IDurableEntityClient client, string stateKey)
        {
            var entityId = new EntityId(nameof(TStateEntity), stateKey);

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
