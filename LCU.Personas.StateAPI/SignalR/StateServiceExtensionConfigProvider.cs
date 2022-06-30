using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LCU.Personas.StateAPI.SignalRClient
{
    public class StateServiceExtensionConfigProvider : IExtensionConfigProvider
    {
        #region Fields
        protected readonly ConcurrentDictionary<string, IStateService> clientCache;

        protected readonly IStateServiceFactory serviceFactory;
        #endregion

        #region Constructors
        public StateServiceExtensionConfigProvider(IStateServiceFactory serviceFactory)
        {
            this.serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

            clientCache = new ConcurrentDictionary<string, IStateService>();
        }
        #endregion

        #region API Methods
        public void Initialize(ExtensionConfigContext context)
        {
            var triggerRule = context.AddBindingRule<StateServiceTriggerAttribute>();

            triggerRule.BindToTrigger(new StateServiceTriggerBindingProvider(this));

            var inputRule = context.AddBindingRule<StateServiceAttribute>();

            inputRule.BindToValueProvider(async (attribute, type) =>
            {
                var client = loadStateService(attribute);

                return new StateServiceValueBinder(client);
            });
            //inputRule.BindToInput(attribute => loadStateService(attribute));
        }

        public StateServiceTriggerContext CreateContext(StateServiceTriggerAttribute attribute)
        {
            return new StateServiceTriggerContext(attribute, loadStateService(attribute));
        }
        #endregion

        #region Helpers
        protected virtual IStateService loadStateService(StateServiceAttribute attribute)
        {
            var entLookup = attribute.GetEnterpriseLookup();

            var url = attribute.GetURL();

            if (!clientCache.ContainsKey(url))
            {
                clientCache[url] = serviceFactory.CreateStateService(attribute.GetEnterpriseLookup(), url, attribute.Transport);
            }

            return clientCache[url];
        }
        #endregion
    }
}