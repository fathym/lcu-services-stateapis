using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(LCU.Personas.StateAPI.StateServices.StateServiceBinding.Startup))]
namespace LCU.Personas.StateAPI.StateServices
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