using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LCU.Personas.StateAPI.StateServices
{
    public class StateServiceFactory : IStateServiceFactory
    {
        #region Fields
        protected readonly IEnumerable<IStateService> stateSvcs;
        #endregion

        #region Constructors
        public StateServiceFactory(IEnumerable<IStateService> stateSvcs)
        {
            this.stateSvcs = stateSvcs;
        }
        #endregion

        public IStateService CreateStateService(string url, HttpTransportType transport,
            Type type = null)
        {
            var client = stateSvcs.FirstOrDefault(ss => ss.URL == url);

            if (client == null && type != null)
                client = (IStateService)Activator.CreateInstance(type, url, transport);
            
            if (client == null)
                client = new StateService(url, transport);

            client.Start().Wait();

            return client;
        }
    }
}