using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fathym.LCU.Services.StateAPIs.StateServices
{
    public class StateActionsClientFactory : IStateActionsClientFactory
    {
        #region Fields
        protected readonly IEnumerable<IStateActionsClient> stateSvcs;
        #endregion

        #region Constructors
        public StateActionsClientFactory(IEnumerable<IStateActionsClient> stateSvcs)
        {
            this.stateSvcs = stateSvcs;
        }
        #endregion

        public IStateActionsClient CreateStateActionsClient(string url, HttpTransportType transport,
            Type type = null)
        {
            var client = stateSvcs.FirstOrDefault(ss => ss.URL == url);

            if (client == null && type != null)
                client = (IStateActionsClient)Activator.CreateInstance(type, url, transport);
            
            if (client == null)
                client = new StateActionsClient(url, transport);

            client.Start().Wait();

            return client;
        }
    }
}