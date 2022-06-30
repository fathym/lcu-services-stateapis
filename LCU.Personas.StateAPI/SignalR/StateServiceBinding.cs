using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(LCU.Personas.StateAPI.SignalRClient.StateServiceBinding.Startup))]
namespace LCU.Personas.StateAPI.SignalRClient
{
    public class StateServiceBinding
    {
        public class Startup : IWebJobsStartup
        {
            public void Configure(IWebJobsBuilder builder)
            {
                builder.AddStateServiceExtension();
            }
        }
    }
}