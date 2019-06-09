using Newtonsoft.Json.Linq;
using System.Linq;

namespace CosmosManager.Extensions
{

    public static class JTokenExtensions
    {
        public static string ToStringValue(this JToken token, bool findFirstString = false)
        {
            if (token == null)
            {
                return "";
            }
            if (token.Type == JTokenType.Null || token.Type == JTokenType.None)
            {
                return "";
            }
            if (token.Type == JTokenType.Boolean)
            {
                return token.Value<string>()?.ToLowerInvariant();
            }
            if (token.Type.IsPrimitiveType())
            {
                return token.Value<string>();
            }
            if(findFirstString && !token.Type.IsPrimitiveType() && token.Children().Count() > 0)
            {
                return token.First().ToStringValue(findFirstString);
            }
            return string.Empty;
        }

        public static string GetObjectValue(this JToken token, string propToCheckFor)
        {
            if (token == null)
            {
                return "";
            }

            if (token.Type == JTokenType.Null || token.Type == JTokenType.None)
            {
                return "";
            }
            if (token.Type == JTokenType.Object)
            {
                if (!string.IsNullOrWhiteSpace(propToCheckFor))
                {
                    var jtoken = token.SelectToken(propToCheckFor);
                    if (jtoken != null && !jtoken.Type.IsPrimitiveType())
                    {
                        return jtoken.FirstOrDefault()?.ToStringValue(true);
                    }
                    else
                    {
                        return jtoken.ToStringValue();
                    }
                }
                return token.FirstOrDefault()?.ToStringValue(true);
            }
            return "";
        }

        public static bool IsPrimitiveType(this JTokenType token)
        {
            switch (token)
            {
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.String:
                case JTokenType.Boolean:
                case JTokenType.Date:
                case JTokenType.Guid:
                case JTokenType.TimeSpan:
                case JTokenType.None:
                case JTokenType.Uri:
                case JTokenType.Null:
                case JTokenType.Undefined:
                    return true;

                default:
                    return false;
            }
        }
    }
}