﻿using Fathym.API;
using Fathym.API.Fluent;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Fathym.LCU.Services.StateAPIs
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
        protected virtual string loadAccessToken(HttpRequestMessage req)
        {
            var accessToken = req.Headers.Authorization.Parameter;

            if (accessToken.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(accessToken));

            return accessToken;
        }

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

        protected virtual async Task executeIfTokenValid(HttpRequestMessage req, Func<JwtSecurityToken, Task> action, Func<Task> invalidTokenHandler)
        {
            string accessToken = loadAccessToken(req);

            try
            {
                IdentityModelEventSource.ShowPII = true;

                var handler = new JwtSecurityTokenHandler();

                var token = handler.ReadToken(accessToken) as JwtSecurityToken;

                var iss = token.Issuer;

                var tfp = token.Payload["tfp"].ToString(); // Sign-in policy name

                var metadataEndpoint = $"{iss}.well-known/openid-configuration?p={tfp}";

                var cm = new ConfigurationManager<OpenIdConnectConfiguration>(metadataEndpoint,
                    new OpenIdConnectConfigurationRetriever(),
                    new HttpDocumentRetriever());

                var discoveryDocument = await cm.GetConfigurationAsync();

                var signingKeys = discoveryDocument.SigningKeys;

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = signingKeys,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };

                var claims = handler.ValidateToken(accessToken, validationParameters, out SecurityToken validatedToken);

                if (validatedToken != null)
                    await action(token);
            }
            catch (SecurityTokenException stex)
            {
                await invalidTokenHandler();
            }
        }

        protected virtual IAPIBoundary<HttpResponseMessage> withAPIBoundary()
        {
            return new APIBoundary<HttpResponseMessage>(logger);
        }

        protected virtual IAPIBoundaried<HttpResponseMessage> withSecureAPIBoundary<TResp>(HttpRequestMessage req, Func<TResp, JwtSecurityToken, Task<TResp>> action, Func<Task<TResp>> setInvalidTokenResponse)
                where TResp : new()
        {
            return withAPIBoundary<TResp>(req, async resp =>
            {
                await executeIfTokenValid(req, async accessToken =>
                {
                    resp = await action(resp, accessToken);
                },
                async () =>
                {
                    resp = await setInvalidTokenResponse();
                });

                return resp;
            });
        }

        protected virtual IAPIBoundaried<HttpResponseMessage> withAPIBoundary<TResp>(HttpRequestMessage req,
            Func<TResp, Task<TResp>> action)
                where TResp : new()
        {
            return withAPIBoundaried(api =>
            {
                return api
                    .SetDefaultResponse(req.CreateResponse())
                    .SetAction(async httpResponse =>
                    {
                        var response = new TResp();

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

        protected virtual IAPIBoundaried<HttpResponseMessage> withSecureAPIBoundary<TRequest, TResponse>(HttpRequestMessage req, Func<TRequest, TResponse, JwtSecurityToken, Task<TResponse>> action)
                where TRequest : class, new()
                where TResponse : BaseResponse, new()
        {
            return withAPIBoundary<TRequest, TResponse>(req, async (request, response) =>
            {
                await executeIfTokenValid(req, async accessToken =>
                {
                    response = await action(request, response, accessToken);
                },
                async () =>
                {
                    response = new TResponse() { Status = Status.Unauthorized };
                });

                return response;
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
