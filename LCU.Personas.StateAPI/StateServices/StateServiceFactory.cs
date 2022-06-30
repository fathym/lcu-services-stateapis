using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
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

        public IStateService CreateStateService(string entLookup, string url, HttpTransportType transport)
        {
            var client = stateSvcs.FirstOrDefault(ss => ss.URL == url);

            if (client == null)
                client = new StateService(url, transport);

            client.Start(entLookup).Wait();

            return client;
        }
    }
}