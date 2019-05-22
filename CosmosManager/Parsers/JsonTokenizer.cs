using CosmosManager.Builders;
using CosmosManager.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CosmosManager.Parsers
{
    public class JsonTokenizer
    {
        private MatchCollection _matches;
        const string TOKEN = "@@";

        public int CurrentMatchCount { get; private set; }

        public string TokenizeJsonSections(string query)
        {
            var tokenQuery = query;
            var builder = new RegExBuilder();
            var jsonRegex = builder.WithJSONContent().Build();
            _matches = jsonRegex.Matches(tokenQuery);
            CurrentMatchCount = _matches.Count;
            return jsonRegex.Replace(tokenQuery, TOKEN);
        }
        public string DetokenJsonSections(string query, bool reformatJson = false)
        {
            var tokenQuery = query;
            foreach (Match match in _matches)
            {
                var b = new StringBuilder(tokenQuery);
                var newValue = match.Value;
                if (reformatJson)
                {
                    try
                    {
                        if (newValue.TrimStart().StartsWith("["))
                        {
                            var arr = JArray.Parse(newValue);
                            newValue = JsonConvert.SerializeObject(arr, Formatting.Indented);
                        }
                        else
                        {
                            var obj = JObject.Parse(newValue);
                            newValue = JsonConvert.SerializeObject(obj, Formatting.Indented);
                        }
                        newValue = $"{Constants.NEWLINE}{newValue.Replace("\r\n", Constants.NEWLINE).Replace("\r", "")}{Constants.NEWLINE}";
                    }
                    catch
                    {
                        newValue = match.Value;
                    }
                }
                b.Replace(TOKEN, newValue, tokenQuery.IndexOf(TOKEN), TOKEN.Length);
                tokenQuery = b.ToString();
            }
            return tokenQuery;
        }
    }
}