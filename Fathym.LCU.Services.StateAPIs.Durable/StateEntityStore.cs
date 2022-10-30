using Fathym;
using Fathym.API;
using Fathym.API.Fluent;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Fathym.LCU.Services.StateAPIs.Durable
{
    public abstract class StateEntityStore<TStateEntity>
        where TStateEntity : class
    {
        #region Fields
        protected virtual IDurableEntityContext context
        {
            get
            {
                return Entity.Current;
            }
        }

        protected string[] keyParts
        {
            get { return context.EntityKey.Split(splitKey); }
        }

        protected readonly ILogger logger;

        protected char splitKey;
        #endregion

        #region Properties

        #endregion

        #region Constructors
        public StateEntityStore(ILogger logger)
        {
            this.logger = logger;

            splitKey = '|';
        }
        #endregion
        
        #region Life Cycle
        protected virtual async Task initializeStateEntity(IDurableEntityContext ctx)
        {
            if (!ctx.HasState)
                await handleStateEmpty(ctx);

            await ctx.DispatchAsync<TStateEntity>();
        }
        #endregion

        #region Entity Actions

        #region Action Orchestrations

        #endregion
        #endregion

        #region Helpers
        protected virtual async Task handleStateEmpty(IDurableEntityContext ctx)
        {
            ctx.SetState(loadInitialState());
        }

        protected abstract TStateEntity loadInitialState();

        protected virtual T newtonsoftConvert<T>(T obj)
        {
            //  This is a little hack to handle the issue with Durable Entities not using System.Text.Json and JsonElement
            //      not serializing correctly inside newtonsoft.
            var objStr = obj.ToJSON();

            obj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(objStr);

            return obj;
        }
        #endregion
    }
}
