using System.Text.RegularExpressions;

namespace CosmosManager.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceWith(this string query,string searchTerm, string replaceTerm)
        {
            var pattern = $@"(?!\B[""\'][^""\']*)\b{searchTerm}\b(?![^""\']*[""\']\B)";
            return Regex.Replace(query, pattern, replaceTerm, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        public static string TruncateTo(this string text, int length)
        {
            var maxLength  = text.Length < length ? text.Length : length;
            return text.Substring(0, maxLength) + "...";
        }
    }
}