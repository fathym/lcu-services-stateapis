using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System.Threading.Tasks;

namespace LCU.Personas.StateAPI
{
    public class LCUStateHub<TStateEntity> : ServerlessHub
    {
        #region Helpers
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
