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

        public static bool IsRunning(this DurableOrchestrationStatus status)
        {
            return status != null &&
                status.RuntimeStatus != OrchestrationRuntimeStatus.Completed &&
                status.RuntimeStatus != OrchestrationRuntimeStatus.Failed &&
                status.RuntimeStatus != OrchestrationRuntimeStatus.Canceled &&
                status.RuntimeStatus != OrchestrationRuntimeStatus.Terminated;
        }

        public static async Task TerminateWithCheckAsync(this IDurableOrchestrationClient orchClient, string instanceId, string reason, int terminateTimeoutSeconds = 0)
        {
            var instanceStatus = await orchClient.GetStatusAsync(instanceId);

            var timeout = DateTime.UtcNow.AddSeconds(terminateTimeoutSeconds);

            if (instanceStatus.RuntimeStatus == OrchestrationRuntimeStatus.Running && instanceStatus.LastUpdatedTime < timeout)
                await orchClient.TerminateAsync(instanceId, reason);
            var processStatus = await orchClient.GetStatusAsync(instanceId);

            if (processStatus.IsRunning())
                await orchClient.TerminateAsync(instanceId, reason);
        }
    }
}
