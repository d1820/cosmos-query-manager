using CosmosManager.Builders;
using System.Text;
using System.Text.RegularExpressions;

namespace CosmosManager.Parsers
{
    public class CommentTokenizer
    {
        public string TOKEN = "@##";

        public int CurrentMatchCount { get; private set; }

        public MatchCollection CurrentMatches { get; private set; }

        public string TokenizeComments(string query)
        {
            var tokenQuery = query;
            var builder = new RegExBuilder();
            var jsonRegex = builder.WithNoComments().Build();
            CurrentMatches = jsonRegex.Matches(tokenQuery);
            CurrentMatchCount = CurrentMatches.Count;
            return jsonRegex.Replace(tokenQuery, TOKEN);
        }

        public string DetokenizeComments(string query)
        {
            var tokenQuery = query;
            foreach (Match match in CurrentMatches)
            {
                var b = new StringBuilder(tokenQuery);
                var newValue = match.Value;
                b.Replace(TOKEN, newValue, tokenQuery.IndexOf(TOKEN), TOKEN.Length);
                tokenQuery = b.ToString();
            }
            return tokenQuery;
        }

        public string DetokenizeCommentsAt(string query, int matchIndex)
        {
            //if the count is off and greater then just remove the token
            if (CurrentMatches.Count - 1 < matchIndex)
            {
                return query.Replace(TOKEN, "");
            }
            var match = CurrentMatches[matchIndex];
            var b = new StringBuilder(query);
            var newValue = match.Value;
            b.Replace(TOKEN, newValue, query.IndexOf(TOKEN), TOKEN.Length);
            return b.ToString();
        }
    }
}