using Fathym;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LCU.Personas.StateAPI.StateServices
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
            var inputRule = context.AddBindingRule<StateServiceAttribute>();

            inputRule.BindToValueProvider(async (attribute, type) =>
            {
                var client = loadStateService(attribute, type);

                return new StateServiceValueBinder(client);
            });

            var triggerRule = context.AddBindingRule<StateServiceTriggerAttribute>();

            triggerRule.BindToTrigger(new StateServiceTriggerBindingProvider(this));
        }

        public StateServiceTriggerContext CreateContext(StateServiceTriggerAttribute attribute)
        {
            //var clientType = !attribute.ClientType.IsNullOrEmpty() ? Type.GetType(attribute.ClientType) : null;

            return new StateServiceTriggerContext(attribute, loadStateService(attribute, null));//, clientType));
        }
        #endregion

        #region Helpers
        protected virtual IStateService loadStateService(StateServiceTriggerAttribute attribute, Type type)
        {
            return loadStateService(attribute.GetURL(), attribute.Transport, type);
        }

        protected virtual IStateService loadStateService(StateServiceAttribute attribute, Type type)
        {
            return loadStateService(attribute.GetURL(), attribute.Transport, type);
        }

        protected virtual IStateService loadStateService(string url, HttpTransportType transport, Type type)
        {
            var cacheKey = $"{url}";

            if (!clientCache.ContainsKey(cacheKey))
            {
                clientCache[cacheKey] = serviceFactory.CreateStateService(url, transport, type);
            }

            return clientCache[cacheKey];
        }
        #endregion
    }
}