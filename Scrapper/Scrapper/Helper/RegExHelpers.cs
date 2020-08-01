using System.Text.RegularExpressions;

namespace Scrapper.Helper
{
    public static class RegExHelpers
    {
        public static string FindUrl(string html)
        {
            // check
            string pattern = @"<a\s+(?:[^>]*?\s+)?href=([""'])(.*?)\1";
            var result = Regex.Match(html, pattern, RegexOptions.IgnoreCase);

            if (result.Success)
            {
                return result.Groups[2].Value;
            }

            return null;
        }
    }
}
