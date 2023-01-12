using System;

namespace Fathym.LCU.Services.StateAPIs
{
    public static class ObjectExtensions
    {
        public static Newtonsoft.Json.Linq.JToken ToJToken(this object obj)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JToken>(obj.ToJSON());
        }

        public static T ToJToken<T>(this object obj)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(obj.ToJSON());
        }
    }

}
