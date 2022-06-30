using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LCU.Personas.StateAPI.StateServices
{
    public class StateServiceListener : IListener
    {
        #region Fields
        protected readonly ITriggeredFunctionExecutor executor;

        protected readonly StateServiceTriggerContext context;
        #endregion

        #region Constructors
        public StateServiceListener(ITriggeredFunctionExecutor executor, StateServiceTriggerContext context)
        {
            this.executor = executor;

            this.context = context;
        }
        #endregion

        #region API Methods
        public void Cancel()
        {
            if (context == null || context.State == null) return;

            StopAsync(CancellationToken.None).Wait();
        }

        public void Dispose()
        {
            StopAsync(CancellationToken.None).Wait();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            context.State.State += (sender, stateEvent) =>
            {
                var triggerData = new TriggeredFunctionData
                {
                    TriggerValue = stateEvent.State.ToJSON()
                };

                var task = executor.TryExecuteAsync(triggerData, CancellationToken.None);

                task.Wait();
            };
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return context.State.Stop();
        }
        #endregion
    }
}