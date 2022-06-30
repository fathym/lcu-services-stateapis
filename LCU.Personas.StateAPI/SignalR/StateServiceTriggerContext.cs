namespace LCU.Personas.StateAPI.SignalRClient
{
    public class StateServiceTriggerContext
    {
        #region Properties
        public virtual StateServiceTriggerAttribute TriggerAttribute { get; set; }

        public virtual IStateService State { get; set; }
        #endregion

        #region Constructors
        public StateServiceTriggerContext(StateServiceTriggerAttribute attribute, IStateService state)
        {
            State = state;

            TriggerAttribute = attribute;
        }
        #endregion
    }
}