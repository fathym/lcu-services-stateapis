using Fathym;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fathym.LCU.Services.StateAPIs.StateServices
{
    public class StateServiceExtensionConfigProvider : IExtensionConfigProvider
    {
        #region Fields
        protected readonly ConcurrentDictionary<string, IStateActionsClient> clientCache;

        protected readonly IStateActionsClientFactory serviceFactory;
        #endregion

        #region Constructors
        public StateServiceExtensionConfigProvider(IStateActionsClientFactory serviceFactory)
        {
            this.serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

            clientCache = new ConcurrentDictionary<string, IStateActionsClient>();
        }
        #endregion

        #region API Methods
        public void Initialize(ExtensionConfigContext context)
        {
            var inputRule = context.AddBindingRule<StateServiceAttribute>();

            inputRule.BindToValueProvider(async (attribute, type) =>
            {
                var client = loadStateService(attribute, async () => Environment.GetEnvironmentVariable("TEMP_JWT"), type);

                return new StateServiceValueBinder(client);
            });

            var triggerRule = context.AddBindingRule<StateServiceTriggerAttribute>();

            triggerRule.BindToTrigger(new StateServiceTriggerBindingProvider(this));
        }

        public StateServiceTriggerContext CreateContext(StateServiceTriggerAttribute attribute, Func<Task<string>> accessTokenProvider)
        {
            //var clientType = !attribute.ClientType.IsNullOrEmpty() ? Type.GetType(attribute.ClientType) : null;

            return new StateServiceTriggerContext(attribute, loadStateService(attribute, accessTokenProvider, null));//, clientType));
        }
        #endregion

        #region Helpers
        protected virtual IStateActionsClient loadStateService(StateServiceTriggerAttribute attribute, Func<Task<string>> accessTokenProvider, Type type)
        {
            return loadStateService(attribute.GetURL(), attribute.Transport, accessTokenProvider, type);
        }

        protected virtual IStateActionsClient loadStateService(StateServiceAttribute attribute, Func<Task<string>> accessTokenProvider, Type type)
        {
            return loadStateService(attribute.GetURL(), attribute.Transport, accessTokenProvider, type);
        }

        protected virtual IStateActionsClient loadStateService(string url, HttpTransportType transport, Func<Task<string>> accessTokenProvider, Type type)
        {
            var cacheKey = $"{url}";

            if (!clientCache.ContainsKey(cacheKey))
            {
                clientCache[cacheKey] = serviceFactory.CreateStateActionsClient(url, transport, accessTokenProvider, type);
            }

            return clientCache[cacheKey];
        }
        #endregion
    }
}