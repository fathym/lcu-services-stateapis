using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;

namespace LCU.Personas.StateAPI
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
        protected virtual T newtonsoftConvert<T>(T obj)
        {
            //  This is a little hack to handle the issue with Durable Entities not using System.Text.Json and JsonElement
            //      not serializing correctly inside newtonsoft.
            var objStr = obj.ToJSON();

            obj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(objStr);

            return obj;
        }
        #endregion
    }
}
