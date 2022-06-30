using Microsoft.Azure.WebJobs.Host.Triggers;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace LCU.Personas.StateAPI.SignalRClient
{
    public class StateServiceTriggerBindingProvider : ITriggerBindingProvider
    {
        private StateServiceExtensionConfigProvider provider;

        public StateServiceTriggerBindingProvider(StateServiceExtensionConfigProvider provider)
        {
            this.provider = provider;
        }

        public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
        {
            var parameter = context.Parameter;

            var attribute = parameter.GetCustomAttribute<StateServiceTriggerAttribute>(false);

            if (attribute == null)
                return Task.FromResult<ITriggerBinding>(null);

            //if (parameter.ParameterType != typeof(string))
            //throw new InvalidOperationException("Invalid parameter type");

            var triggerBinding = new StateServiceTriggerBinding(provider.CreateContext(attribute));

            return Task.FromResult<ITriggerBinding>(triggerBinding);
        }
    }
}