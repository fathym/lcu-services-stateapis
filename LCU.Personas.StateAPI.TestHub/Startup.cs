using Microsoft.Extensions.DependencyInjection;
using System;
using LCU.Personas.StateAPI.TestHub;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using LCU.Personas.StateAPI.StateServices;

[assembly: FunctionsStartup(typeof(Startup))]

namespace LCU.Personas.StateAPI.TestHub
{

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();

            builder.Services.AddLogging();

            builder.Services.AddHttpContextAccessor();
        }
    }
}