using System.Text.RegularExpressions;

namespace CosmosManager.Builders
{
    public class RegExBuilder
    {
        private string _startingKeywords;
        private string _endingKeywords;
        private string _noComments;
        private string _content;
        private string _variable;
        private string _jsonContent;

        public RegExBuilder HasStartingKeywords(params string[] keyWords)
        {
            _startingKeywords = string.Join("|", keyWords);
            return this;
        }

        public RegExBuilder GrabUntil()
        {
            _content = "(.*?)";
            return this;
        }

        public RegExBuilder GrabUntilEnd()
        {
            _content = "(.*)";
            return this;
        }

        public RegExBuilder WithJSONContent()
        {
            _jsonContent = @"(\[\s*{|{)+(.*)(\}\s*]|})+"; //singleline
            return this;
        }

        public RegExBuilder WithNoComments()
        {
            _noComments = @"(\/\*+[^*]*\*+(?:[^\/*][^*]*\*\/+))|(\/\*.*?\*\/)"; //singleline
            return this;
        }

        public RegExBuilder WithVariable()
        {
            _variable = @"[\s]*\@\w+[\s]*=[\s]*(SELECT)(.*?)";
            return this;
        }

        public RegExBuilder HasEndingKeyWordsNotInQuotes(params string[] keyWords)
        {
            _endingKeywords = string.Join("|", keyWords);
            return this;
        }

        public RegExBuilder Reset()
        {
            _endingKeywords = null;
            _noComments = null;
            _startingKeywords = null;
            _content = null;
            _variable = null;
            _jsonContent = null;
            return this;
        }

        public Regex Build()
        {
            if (!string.IsNullOrWhiteSpace(_variable))
            {
                return new Regex(_variable, RegexOptions.Compiled);
            }

            if (!string.IsNullOrWhiteSpace(_noComments))
            {
                return new Regex(_noComments, RegexOptions.Compiled | RegexOptions.Singleline);
            }

            if (!string.IsNullOrWhiteSpace(_jsonContent))
            {
                return new Regex(_jsonContent, RegexOptions.Compiled | RegexOptions.Singleline);
            }

            if (!string.IsNullOrWhiteSpace(_endingKeywords))
            {
                return new Regex($@"({_startingKeywords}){_content}(?=[^\'\""]{ _endingKeywords }[^\'\""])\s*", RegexOptions.Compiled | RegexOptions.Singleline);

                //return new Regex($@"({_startingKeywords}){_content}(?!\""\s*({_endingKeywords}).\s*\"")(?={_endingKeywords})", RegexOptions.Compiled | RegexOptions.Singleline);
            }
            if (!string.IsNullOrEmpty(_content))
            {
                return new Regex($"({_startingKeywords}){_content}", RegexOptions.Compiled | RegexOptions.Singleline);
            }
            return new Regex($"({_startingKeywords})", RegexOptions.Compiled);
        }
    }
}