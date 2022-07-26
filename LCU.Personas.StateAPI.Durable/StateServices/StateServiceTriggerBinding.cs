using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LCU.Personas.StateAPI.StateServices
{
    public class StateServiceTriggerBinding : ITriggerBinding
    {
        private readonly StateServiceTriggerContext context;

        public StateServiceTriggerBinding(StateServiceTriggerContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Type TriggerValueType => typeof(string);

        public IReadOnlyDictionary<string, Type> BindingDataContract => new Dictionary<string, Type>();

        public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
        {
            var valueProvider = new StateServiceValueBinder(value);

            var bindingData = new Dictionary<string, object>();

            var triggerData = new TriggerData(valueProvider, bindingData);

            return Task.FromResult<ITriggerData>(triggerData);
        }

        public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
        {
            var executor = context.Executor;

            var listener = new StateServiceListener(executor, this.context);

            return Task.FromResult<IListener>(listener);
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new TriggerParameterDescriptor
            {
                Name = "SignalR Client",
                DisplayHints = new ParameterDisplayHints
                {
                    Prompt = "SignalR Client",
                    Description = "SignalR Client message trigger"
                }
            };
        }
    }
}