using Fathym.LCU.Services.StateAPIs.StateServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using Fathym.LCU.Services.StateAPIs.StateServices.TestAPI;
using Fathym.LCU.Services.StateAPIs.StateServices.TestAPI.State;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Fathym.LCU.Services.StateAPIs.StateServices.TestAPI
{

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();

            builder.Services.AddLogging();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSingleton<IStateActionsClient, TestStateActionsClient>(svcProvider =>
            {
                var url = Environment.GetEnvironmentVariable("TEST_STATE_HUB_URL");

                return new TestStateActionsClient(url, HttpTransportType.WebSockets);
            });
        }
    }
}