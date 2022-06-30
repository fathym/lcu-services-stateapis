namespace LCU.Personas.StateAPI.StateServices
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