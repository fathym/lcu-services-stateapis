using System.Threading.Tasks;
using Fathym;

namespace Fathym.LCU.Services.StateAPIs.TestHub.State
{
    public interface ITestState
    {
        Task SetTest(string test);
    }
}