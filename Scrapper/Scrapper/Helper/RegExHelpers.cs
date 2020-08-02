using System.Text.RegularExpressions;

namespace Scrapper.Helper
{
    public static class RegExHelpers
    {
        private static readonly Regex lineBreaks = new Regex(@"\r\n?|\n");
        private static readonly Regex urlPattern = new Regex(@"<a\s+(?:[^>]*?\s+)?href=([""'])(.*?)\1", RegexOptions.IgnoreCase);


        public static string FindUrl(string html)
        {
            var result = urlPattern.Match(html);
            if (result.Success)
            {
                return result.Groups[2].Value;
            }
            return null;
        }

        public static string Flatten(string input)
        {
            return lineBreaks.Replace(input, "");
        }
    }
}
