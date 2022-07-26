using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LCU.Personas.StateAPI.StateServices
{
    public static class StateServiceWebJobsBuilderExtensions
    {
        public static IWebJobsBuilder AddStateServiceExtension(this IWebJobsBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.AddExtension<StateServiceExtensionConfigProvider>();

            builder.Services.AddSingleton<IStateServiceFactory, StateServiceFactory>();

            return builder;
        }
    }
}