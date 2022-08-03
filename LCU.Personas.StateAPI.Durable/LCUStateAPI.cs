using Fathym.API;
using Fathym.API.Fluent;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LCU.Personas.StateAPI.Durable
{
    public class LCUStateAPI
    {
        #region Fields
        protected ILogger logger;
        #endregion

        #region Constructors
        public LCUStateAPI(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        #endregion

        #region Helpers
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

        protected virtual IAPIBoundary<HttpResponseMessage> withAPIBoundary()
        {
            return new APIBoundary<HttpResponseMessage>(logger);
        }

        protected virtual IAPIBoundaried<HttpResponseMessage> withAPIBoundary<T>(HttpRequestMessage req,
            Func<T, Task<T>> action)
                where T : new()
        {
            return withAPIBoundaried(api =>
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

        protected virtual IAPIBoundaried<HttpResponseMessage> withAPIBoundary<TRequest, TResponse>(HttpRequestMessage req,
            Func<TRequest, TResponse, Task<TResponse>> action)
                where TRequest : class, new()
                where TResponse : BaseResponse, new()
        {
            return withAPIBoundary<TResponse>(req, async response =>
            {
                var bodyStr = await req.Content.ReadAsStringAsync();

                var request = bodyStr?.FromJSON<TRequest>();

                logger.LogDebug("Calling boundary action");

                response = await action(request, response);

                return response;
            });
        }

        protected virtual IAPIBoundaried<HttpResponseMessage> withAPIBoundaried(
            Func<IAPIBoundary<HttpResponseMessage>, IAPIBoundaried<HttpResponseMessage>> api)
        {
            return api(withAPIBoundary());
        }
        #endregion
    }
}
