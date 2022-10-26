namespace Fathym.LCU.Services.StateAPIs.StateServices
{
    public class StateUpdateRequest<TState> : StateRequest
    {
        public virtual TState State { get; set; }
    }
}
