using Fathym.API;
using Fathym.API.Fluent;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCU.Personas.StateAPI
{
    public class LCUStateAPI
    {
        #region Fields
        protected ILogger logger;
        #endregion

        #region Constructors
        public LCUStateAPI()
        { }
        #endregion

        #region Helpers
        protected virtual string loadEntLookup(HttpRequestData req)
        {
            var entLookup = req.Headers.GetValues("lcu-ent-lookup").FirstOrDefault();

            if (entLookup.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(entLookup));

            return entLookup;
        }

        protected virtual string loadUsername(HttpRequestData req)
        {
            var username = req.Headers.GetValues("x-ms-client-principal-id").FirstOrDefault();

            if (username.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(username));

            return username;
        }

        protected virtual void setLogger(FunctionContext context)
        {
            logger = context.GetLogger(GetType().Name);
        }

        protected virtual IAPIBoundary<HttpResponseData> withAPIBoundary(FunctionContext context)
        {
            setLogger(context);

            return new APIBoundary<HttpResponseData>(logger);
        }

        protected virtual IAPIBoundaried<HttpResponseData> withAPIBoundary<T>(HttpRequestData req, FunctionContext context, 
            Func<T, Task<T>> action)
                where T : new()
        {
            return withAPIBoundaried(context, api =>
            {
                return api
                    .SetDefaultResponse(req.CreateResponse())
                    .SetAction(async httpResponse =>
                    {
                        var response = new T();

                        logger.LogDebug("Calling boundary action");

                        response = await action(response);

                        logger.LogDebug("Writing response for boundary action");

                        //httpResponse.Headers.Add("Content-Type", "application/json");

                        var responseStr = response.ToJSON();

                        await httpResponse.WriteStringAsync(responseStr);

                        logger.LogDebug("Wrote response for boundary action");

                        return httpResponse;
                    });
            });
        }

        protected virtual IAPIBoundaried<HttpResponseData> withAPIBoundary<TRequest, TResponse>(HttpRequestData req, FunctionContext context,
            Func<TRequest, TResponse, Task<TResponse>> action)
                where TRequest : class, new()
                where TResponse : BaseResponse, new()
        {
            return withAPIBoundary<TResponse>(req, context, async response =>
            {
                using var streamRdr = new StreamReader(req.Body);

                var bodyStr = await streamRdr.ReadToEndAsync();

                var request = bodyStr?.FromJSON<TRequest>();

                logger.LogDebug("Calling boundary action");

                response = await action(request, response);

                return response;
            });
        }

        protected virtual IAPIBoundaried<HttpResponseData> withAPIBoundaried(FunctionContext context, 
            Func<IAPIBoundary<HttpResponseData>, IAPIBoundaried<HttpResponseData>> api)
        {
            return api(withAPIBoundary(context));
        }

        //protected virtual IAPIBoundary<BaseResponse<T>> withModeledAPIBoundary<T>(FunctionContext context)
        //{
        //    setLogger(context);

        //    return new APIResponseBoundary<BaseResponse<T>>();
        //}
        #endregion
    }

}
