using Fathym.LCU.Services.StateAPIs.StateServices;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fathym.LCU.Services.StateAPIs.Durable
{
    public static class StateExtensions
    {
        public static TActions CreateEntityProxy<TEntityStore, TActions>(this IDurableOrchestrationContext context, string instanceId = null)
        {
            var entityId = new EntityId(typeof(TEntityStore).Name, instanceId ?? context.InstanceId);

            var entityProxy = context.CreateEntityProxy<TActions>(entityId);

            return entityProxy;
        }

        public static bool IsRunning(this DurableOrchestrationStatus status)
        {
            return status != null &&
                status.RuntimeStatus != OrchestrationRuntimeStatus.Completed &&
                status.RuntimeStatus != OrchestrationRuntimeStatus.Failed &&
                status.RuntimeStatus != OrchestrationRuntimeStatus.Canceled &&
                status.RuntimeStatus != OrchestrationRuntimeStatus.Terminated;
        }

        public static TStateActionsClient LoadClient<TStateActionsClient>(this IEnumerable<IStateActionsClient> stateActions)
            where TStateActionsClient : class, IStateActionsClient
        {
            return (TStateActionsClient)stateActions.FirstOrDefault(sa => sa is TStateActionsClient);
        }

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

        public static async Task TerminateWithCheckAsync(this IDurableOrchestrationClient orchClient, string instanceId, string reason, int terminateTimeoutSeconds = 0)
        {
            var instanceStatus = await orchClient.GetStatusAsync(instanceId);

            var timeout = DateTime.UtcNow.AddSeconds(terminateTimeoutSeconds);

            if (instanceStatus.IsRunning() && instanceStatus.LastUpdatedTime < timeout)
                await orchClient.TerminateAsync(instanceId, reason);
        }
    }
}
