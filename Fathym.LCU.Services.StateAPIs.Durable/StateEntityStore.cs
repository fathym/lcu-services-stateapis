using Fathym;
using Fathym.API;
using Fathym.API.Fluent;
using Fathym.LCU.Services.StateAPIs.StateServices;
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
    public interface IStateEntityStore
    {
        Task _Load();

        Task _Reset();
    }

    public abstract class StateEntityStore<TEntityStore> : IStateEntityStore
        where TEntityStore : class
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
        public virtual async Task _Load()
        { }

        public virtual async Task _Reset()
        {
            context.DeleteState();
        }
        #endregion

        #region Entity Actions

        #region Action Orchestrations

        #endregion
        #endregion

        #region Helpers
        protected virtual async Task afterDispatchStore(IDurableEntityContext ctx, IAsyncCollector<SignalRMessage> messages)
        {
            await loadAndUpdateState(ctx, messages);
        }

        protected virtual async Task beforeDispatchStore(IDurableEntityContext ctx, IAsyncCollector<SignalRMessage> messages)
        { }

        protected virtual async Task executeStateEntityLifeCycle(IDurableEntityContext ctx, IAsyncCollector<SignalRMessage> messages)
        {
            if (!ctx.HasState)
                await handleStateEmpty(ctx, messages);

            await beforeDispatchStore(ctx, messages);

            await ctx.DispatchAsync<TEntityStore>();

            await afterDispatchStore(ctx, messages);
        }

        protected virtual async Task handleStateEmpty(IDurableEntityContext ctx, IAsyncCollector<SignalRMessage> messages)
        {
            ctx.SetState(loadInitialState());

            await loadAndUpdateState(ctx, messages);
        }

        protected virtual async Task loadAndUpdateState(IDurableEntityContext ctx, IAsyncCollector<SignalRMessage> messages)
        {
            var stateKey = ctx.EntityKey;

            logger.LogDebug($"Loading and updating state: {stateKey}");

            var state = ctx.GetState<TEntityStore>();

            var stateType = typeof(TEntityStore).Name;

            var stateLookup = $"{stateType}|{stateKey}";

            await messages.AddAsync(new SignalRMessage()
            {
                GroupName = stateLookup,
                Target = stateLookup,
                Arguments = new[] {
                    new StateUpdateRequest<TEntityStore>()
                    {
                        StateType = stateType,
                        StateKey = stateKey,
                        State = state
                    }.ToJToken()
                }
            });
        }

        protected abstract TEntityStore loadInitialState();
        #endregion
    }
}
