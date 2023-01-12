namespace Fathym.LCU.Services.StateAPIs.StateServices
{
    public class StateServiceTriggerContext
    {
        #region Properties
        public virtual StateServiceTriggerAttribute TriggerAttribute { get; set; }

        public virtual IStateActionsClient State { get; set; }
        #endregion

        #region Constructors
        public StateServiceTriggerContext(StateServiceTriggerAttribute attribute, IStateActionsClient state)
        {
            State = state;

            TriggerAttribute = attribute;
        }
        #endregion
    }
}