using CosmosManager.Domain;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace CosmosManager.Extensions
{
    public static class JPropertyExtensions
    {
        public static (string col1RowText, string col2RowText) ParseColumnText(this IEnumerable<JProperty> resultProps, string textPartitionKeyPath)
        {
            var col1RowText = string.Empty;
            var col2RowText = string.Empty;
            JProperty col1Prop = null;
            JToken col1Token = null;

            if (resultProps == null)
            {
                return (col1RowText, col2RowText);
            }

            if (resultProps.Count() > 0)
            {
                col1Prop = resultProps.FirstOrDefault(f => f.Name == Constants.DocumentFields.ID);
                if (col1Prop == null)
                {
                    col1Prop = resultProps.FirstOrDefault();
                }
                col1Token = col1Prop?.Value;
                if (col1Token != null)
                {
                    col1RowText = col1Token.Type.IsPrimitiveType() ? col1Token?.ToStringValue() : col1Token?.GetObjectValue(Constants.DocumentFields.ID);
                }
            }

            if (resultProps.Count() > 1)
            {
                JProperty col2Prop = null;
                JToken col2Token = null;

                col2Prop = resultProps.FirstOrDefault(f => f.Name == textPartitionKeyPath);
                if (col2Prop == null)
                {
                    var prop = resultProps.FirstOrDefault(f => f != col1Prop);
                    if (prop != null)
                    {
                        col2Prop = prop;
                    }
                }
                col2Token = col2Prop?.Value;
                if (col2Token != null)
                {
                    col2RowText = col2Token.Type.IsPrimitiveType() ? col2Token?.ToStringValue() : col2Token?.GetObjectValue("");
                }
            }
            return (col1RowText, col2RowText);
        }
    }
}