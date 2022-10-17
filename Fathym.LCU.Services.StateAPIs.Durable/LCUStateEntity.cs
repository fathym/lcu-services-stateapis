using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;

namespace Fathym.LCU.Services.StateAPIs.Durable
{
    public class LCUStateEntity
    {
        #region Fields
        protected virtual IDurableEntityContext context
        {
            get
            {
                return Entity.Current;
            }
        }
        #endregion

        #region Helpers
        #endregion
    }
}
