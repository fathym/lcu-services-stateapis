using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LCU.Personas.StateAPI
{
    public class LCUStateHub<TStateEntity> : ServerlessHub
    {
        #region Helpers
        protected virtual async Task addStateListener(InvocationContext invocationContext, IDurableEntityClient client, string stateKey)
        {
            await Groups.AddToGroupAsync(invocationContext.ConnectionId, stateKey);

            await loadAndUpdateState(client, stateKey);
        }

        protected virtual async Task removeStateListener(InvocationContext invocationContext, string stateKey)
        {
            await Groups.RemoveFromGroupAsync(invocationContext.ConnectionId, stateKey);
        }

        protected virtual async Task connected(InvocationContext invocationContext, IDurableEntityClient client, ILogger logger)
        {
            logger.LogInformation($"{invocationContext.ConnectionId} has connected");

            //var entLookup = invocationContext.Claims["lcu-ent-lookup"];

            //await Groups.AddToGroupAsync(invocationContext.ConnectionId, entLookup);

            //await loadAndUpdateState(client, entLookup);

            //  TODO:  map connection to user
        }

        protected virtual async Task handleStateEmpty(IDurableEntityClient client, EntityId entityId)
        { }

        protected async Task<TStateEntity> loadAndUpdateState(IDurableEntityClient client, string stateKey)
        {
            var state = await loadState(client, stateKey);

            await Clients.Groups(stateKey).SendAsync("state-update", state);

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

            claims.Add(new Claim("lcu-ent-lookup", req.Headers.GetValues("lcu-ent-lookup").FirstOrDefault()));

            return await NegotiateAsync(new NegotiationOptions
            {
                UserId = claims?.FirstOrDefault(c => c.Type == "emails")?.Value,
                Claims = claims
            });
        }
        #endregion
    }
}
