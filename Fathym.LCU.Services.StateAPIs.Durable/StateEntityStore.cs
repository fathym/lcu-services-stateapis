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

        protected readonly ILogger<StateEntityStore<TStateEntity>> logger;
        #endregion

        #region Properties

        #endregion

        #region Constructors
        public StateEntityStore(ILogger<StateEntityStore<TStateEntity>> logger)
        {
            this.logger = logger;
        }
        #endregion

        #region Life Cycle
        protected virtual async Task initializeStateEntity(IDurableEntityContext ctx, IAsyncCollector<SignalRMessage> signalRMessages)
        {
            if (!ctx.HasState)
                await handleStateEmpty(ctx);

            await ctx.DispatchAsync<TStateEntity>();

            var state = ctx.GetState<TStateEntity>();

            await signalStateUpdate(signalRMessages, ctx.EntityId.EntityKey, state);
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
        protected virtual async Task signalStateUpdate(IAsyncCollector<SignalRMessage> signalRMessages, string stateKey, TStateEntity state, string target = "state-update")
        {
            //var stateMeta = state.JSONConvert<MetadataModel>() ?? new MetadataModel();

            //stateMeta.Metadata["$stateKey"] = stateKey;

            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    GroupName = stateKey,
                    Target = target,
                    Arguments = new object[] { state.ToJToken() }
                });
        }
        #endregion
    }
}
