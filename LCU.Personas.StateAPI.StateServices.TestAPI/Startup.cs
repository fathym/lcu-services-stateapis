using LCU.Personas.StateAPI.StateServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using LCU.Personas.StateAPI.StateServices.TestAPI;
using LCU.Personas.StateAPI.StateServices.TestAPI.State;

[assembly: FunctionsStartup(typeof(Startup))]

namespace LCU.Personas.StateAPI.StateServices.TestAPI
{

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();

            builder.Services.AddLogging();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSingleton<IStateService, TestAPIStateService>(svcProvider =>
            {
                var url = Environment.GetEnvironmentVariable("TEST_STATE_HUB_URL");

                return new TestAPIStateService(url, HttpTransportType.WebSockets);
            });
        }
    }
}