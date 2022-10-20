using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Threading.Tasks;

namespace Fathym.LCU.Services.StateAPIs.Durable
{
    public static class StateExtensions
    {
        public static async Task<TEntityStore> LoadEntityFromStore<TEntityStore>(this IDurableEntityClient client, string id)
        {
            var entityId = new EntityId(typeof(TEntityStore).Name, id);

            var state = await client.ReadEntityStateAsync<TEntityStore>(entityId);

            return state.EntityState;
        }

        public static async Task SignalEntityAction<TEntityStore, TEntityStoreActions>(this IDurableEntityClient client, string id, Action<TEntityStoreActions> action)
        {
            var entityId = new EntityId(typeof(TEntityStore).Name, id);

            await client.SignalEntityAsync(entityId, action);
        }
    }
}
