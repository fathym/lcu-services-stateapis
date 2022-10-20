﻿using Fathym.API;
using Fathym.API.Fluent;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Fathym.LCU.Services.StateAPIs.Durable
{
    /// <summary>
    /// State Actions serve as the entry for external systems to access the Entity Stores and data configurations.
    /// </summary>
    public class StateActions : ServerlessHub
    {
        #region Fields
        #endregion

        #region Constructors
        #endregion

        #region Actions
        #endregion

        #region Helpers
        protected virtual async Task<TEntityStore> loadAndUpdateState<TEntityStore>(ILogger logger, IDurableEntityClient client, string stateKey)
        {
            logger.LogDebug($"Loading and updating state: {stateKey}");

            var state = await client.LoadEntityFromStore<TEntityStore>(stateKey);

            //var stateMeta = state.JSONConvert<MetadataModel>() ?? new MetadataModel();

            //stateMeta.Metadata["$stateKey"] = stateKey;

            await Clients.Groups(stateKey).SendAsync("state-update", state.ToJToken());

            return state;
        }

        protected virtual async Task<string> startOrchestration<T>(ILogger logger, IDurableOrchestrationClient orchClient, string orchestrationFunctionName, string instanceId = null, T input = default(T), int terminateTimeoutSeconds = 0)
        {
            logger.LogInformation($"Starting orchestration with id {instanceId}");

            if (!instanceId.IsNullOrEmpty())
            {
                var instanceStatus = await orchClient.GetStatusAsync(instanceId);

                var timeout = DateTime.UtcNow.AddSeconds(terminateTimeoutSeconds);

                if (instanceStatus.RuntimeStatus == OrchestrationRuntimeStatus.Running && instanceStatus.LastUpdatedTime < timeout)
                    await orchClient.TerminateAsync(instanceId, "Timeout");
            }

            instanceId = await orchClient.StartNewAsync(orchestrationFunctionName, instanceId, input);

            return instanceId;
        }

        #region Hub Life Cycle Helpers
        protected virtual async Task attachState<TEntityStore>(ILogger logger, InvocationContext invocationContext, IDurableEntityClient client, string stateKey)
        {
            logger?.LogInformation($"{invocationContext.ConnectionId} has connected");

            await Groups.AddToGroupAsync(invocationContext.ConnectionId, stateKey);

            await loadAndUpdateState<TEntityStore>(logger, client, stateKey);

            //  TODO:  map connection to user
        }

        protected virtual async Task connected(ILogger logger, InvocationContext invocationContext, IDurableEntityClient client)
        {
            logger?.LogInformation($"{invocationContext.ConnectionId} has connected");

            //  TODO:  map connection to user
        }

        protected virtual async Task disconnected(ILogger logger, InvocationContext invocationContext, IDurableEntityClient client)
        {
            logger?.LogInformation($"{invocationContext.ConnectionId} has disconnected");

            //  TODO:  unmap connection to user
        }

        protected virtual async Task<SignalRConnectionInfo> negotiate(ILogger logger, HttpRequestMessage req)
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

        protected virtual async Task unattachState<TEntityStore>(ILogger logger, InvocationContext invocationContext, IDurableEntityClient client, string stateKey)
        {
            logger?.LogInformation($"{invocationContext.ConnectionId} has connected");

            await Groups.RemoveFromGroupAsync(invocationContext.ConnectionId, stateKey);

            await loadAndUpdateState<TEntityStore>(logger, client, stateKey);
        }
        #endregion

        #region Rest Helpers
        protected virtual string loadEntLookup(HttpRequestMessage req)
        {
            var entLookup = req.Headers.GetValues("lcu-ent-lookup").FirstOrDefault();

            if (entLookup.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(entLookup));

            return entLookup;
        }

        protected virtual string loadUsername(HttpRequestMessage req)
        {
            var username = req.Headers.GetValues("x-ms-client-principal-id").FirstOrDefault();

            if (username.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(username));

            return username;
        }

        protected virtual IAPIBoundary<HttpResponseMessage> withAPIBoundary(ILogger logger)
        {
            return new APIBoundary<HttpResponseMessage>(logger);
        }

        protected virtual IAPIBoundaried<HttpResponseMessage> withAPIBoundary<T>(ILogger logger, HttpRequestMessage req, Func<T, Task<T>> action)
                where T : new()
        {
            return withAPIBoundaried(logger, api =>
            {
                return api
                    .SetDefaultResponse(req.CreateResponse())
                    .SetAction(async httpResponse =>
                    {
                        var response = new T();

                        logger.LogDebug("Calling boundary action");

                        response = await action(response);

                        logger.LogDebug("Writing response for boundary action");

                        var responseStr = response.ToJSON();

                        httpResponse.Content = new StringContent(responseStr, Encoding.UTF8, "application/json");

                        logger.LogDebug("Wrote response for boundary action");

                        return httpResponse;
                    });
            });
        }

        protected virtual IAPIBoundaried<HttpResponseMessage> withAPIBoundary<TRequest, TResponse>(ILogger logger, HttpRequestMessage req, Func<TRequest, TResponse, Task<TResponse>> action)
                where TRequest : class, new()
                where TResponse : BaseResponse, new()
        {
            return withAPIBoundary<TResponse>(logger, req, async response =>
            {
                var bodyStr = await req.Content.ReadAsStringAsync();

                var request = bodyStr?.FromJSON<TRequest>();

                logger.LogDebug("Calling boundary action");

                response = await action(request, response);

                return response;
            });
        }

        protected virtual IAPIBoundaried<HttpResponseMessage> withAPIBoundaried(ILogger logger, Func<IAPIBoundary<HttpResponseMessage>, IAPIBoundaried<HttpResponseMessage>> api)
        {
            return api(withAPIBoundary(logger));
        }
        #endregion
        #endregion
    }
}