using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace CosmosManager.Extensions
{
    public static class JObjectExtensions
    {
        public static bool HasEmptyJArray(this JObject parent)
        {
            foreach (var property in parent.Properties())
            {
                if (property.Value is JArray)
                {
                    return ((JArray)property.Value).Count == 0;
                }

                if (property.Value is JObject)
                {
                    return HasEmptyJArray((JObject)property.Value);
                }
            }
            return false;
        }
    }
}