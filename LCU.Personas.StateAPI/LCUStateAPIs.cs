using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
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
        protected virtual async Task signalStateUpdate(IDurableEntityClient client, IAsyncCollector<SignalRMessage> signalRMessages,
            string stateKey, string target = "state-update")
        {
            var entityId = new EntityId(typeof(TStateEntity).Name, stateKey);

            var state = await client.ReadEntityStateAsync<TStateEntity>(entityId);

            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    GroupName = stateKey,
                    Target = target,
                    Arguments = new object[] { state.EntityState }
                });
        }
        #endregion
    }
}
