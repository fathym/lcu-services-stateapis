using Microsoft.Extensions.Logging;
using System;

namespace Fathym.LCU.Services.StateAPIs
{
    public abstract class LCUStateHarness<TState> : IDisposable
    {
        #region Fields
        protected ILogger logger;
        #endregion

        #region Properties
        public virtual TState State { get; protected set; }
        #endregion

        #region Constructors
        public LCUStateHarness(TState state, ILogger logger)
        {
            this.logger = logger;

            State = state;
        }
        #endregion

        #region API Methods
        public virtual void Dispose()
        { }
        #endregion
    }

}
