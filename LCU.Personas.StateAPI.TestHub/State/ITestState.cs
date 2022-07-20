using System.Threading.Tasks;
using Fathym;

namespace LCU.Personas.StateAPI.TestHub.State
{
    public interface ITestState
    {
        Task SetTest(string test);
    }
}