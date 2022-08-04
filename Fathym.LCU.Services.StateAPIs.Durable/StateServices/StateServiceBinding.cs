using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(Fathym.LCU.Services.StateAPIs.StateServices.StateServiceBinding.Startup))]
namespace Fathym.LCU.Services.StateAPIs.StateServices
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