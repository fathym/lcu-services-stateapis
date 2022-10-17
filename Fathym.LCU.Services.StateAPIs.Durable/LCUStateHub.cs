using Fathym;
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
using System.Threading.Tasks;

namespace Fathym.LCU.Services.StateAPIs.Durable
{
    public class StateEntityStore<TStateEntity> : ServerlessHub
         where TStateEntity : class, new()
    {
        #region Fields
        protected virtual IDurableEntityContext context
        {
            get
            {
                return Entity.Current;
            }
        }
        #endregion

        #region Properties

        #endregion

        #region Constructors

        #endregion

        #region Life Cycle
        [FunctionName(nameof(TStateEntity))]
        protected virtual async Task initializeStateEntity([EntityTrigger] IDurableEntityContext ctx,
            [SignalR(HubName = nameof(TStateEntity))] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            if (!ctx.HasState)
                ctx.SetState(new TStateEntity());

            ctx.GetState<TStateEntity>();

            await ctx.DispatchAsync<TStateEntity>();
        }
        #endregion

        #region Actions

        #region Action Orchestrations

        #endregion
        #endregion

        #region Helpers
        protected virtual async Task attachState(InvocationContext invocationContext, string stateKey,
            IDurableEntityClient client, ILogger logger)
        {
            logger.LogInformation($"{invocationContext.ConnectionId} has connected");

            await Groups.AddToGroupAsync(invocationContext.ConnectionId, stateKey);

            await loadAndUpdateState(client, stateKey);

            //  TODO:  map connection to user
        }

        protected virtual async Task connected(InvocationContext invocationContext, IDurableEntityClient client, ILogger logger)
        {
            logger.LogInformation($"{invocationContext.ConnectionId} has connected");

            //  TODO:  map connection to user
        }

        protected virtual async Task disconnected(InvocationContext invocationContext, IDurableEntityClient client, ILogger logger)
        {
            logger.LogInformation($"{invocationContext.ConnectionId} has disconnected");

            //  TODO:  unmap connection to user
        }

        protected virtual async Task handleStateEmpty(IDurableEntityClient client, EntityId entityId)
        { }

        protected virtual async Task<TStateEntity> loadAndUpdateState(IDurableEntityClient client, string stateKey)
        {
            var state = await loadState(client, stateKey);

            var stateMeta = state.JSONConvert<MetadataModel>() ?? new MetadataModel();

            stateMeta.Metadata["$stateKey"] = stateKey;

            await Clients.Groups(stateKey).SendAsync("state-update", stateMeta.ToJToken());

            return state;
        }

        protected virtual async Task<TStateEntity> loadState(IDurableEntityClient client, string stateKey)
        {
            var entityId = new EntityId(typeof(TStateEntity).Name, stateKey);

            var state = await client.ReadEntityStateAsync<TStateEntity>(entityId);

            if (state.EntityState == null)
            {
                await handleStateEmpty(client, entityId);

                state = await client.ReadEntityStateAsync<TStateEntity>(entityId);
            }

            return state.EntityState;
        }

        protected virtual async Task<SignalRConnectionInfo> negotiate(HttpRequestMessage req)
        {
            var authorization = req.Headers.GetValues("Authorization").FirstOrDefault();

            var claims = GetClaims(authorization);

            //claims.Add(new Claim("lcu-state-key", req.Headers.GetValues("lcu-state-key").FirstOrDefault()));

            return await NegotiateAsync(new NegotiationOptions
            {
                UserId = claims?.FirstOrDefault(c => c.Type == "emails")?.Value,
                Claims = claims
            });
        }

        protected virtual async Task unattachState(InvocationContext invocationContext, string stateKey, ILogger logger)
        {
            logger.LogInformation($"{invocationContext.ConnectionId} has connected");

            await Groups.RemoveFromGroupAsync(invocationContext.ConnectionId, stateKey);
        }
        #endregion
    }
}
