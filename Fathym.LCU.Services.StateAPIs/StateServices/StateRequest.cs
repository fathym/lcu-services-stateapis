using Fathym.API;

namespace Fathym.LCU.Services.StateAPIs.StateServices
{
    public class StateRequest : BaseRequest
    {
        public virtual string StateKey { get; set; }

        public virtual string StateType { get; set; }
    }
}
