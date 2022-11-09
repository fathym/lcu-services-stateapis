using Microsoft.Extensions.DependencyInjection;
using System;
using Fathym.LCU.Services.StateAPIs.TestHub;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Fathym.LCU.Services.StateAPIs.StateServices;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Fathym.LCU.Services.StateAPIs.TestHub
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