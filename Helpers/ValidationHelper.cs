using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SafeVault.Helpers
{
    public static class ValidationHelper
    {
        public static bool IsValidInput(string input, string allowedChars) =>
            !string.IsNullOrWhiteSpace(input) && input.All(c => char.IsLetterOrDigit(c) || allowedChars.Contains(c));

        public static bool IsValidXSSInput(string input)
        {
            if (string.IsNullOrEmpty(input))
                return true;

            // Detect script tags
            if (Regex.IsMatch(input, @"<script.*?>.*?</script>", RegexOptions.IgnoreCase))
                return false;

            // Detect event attributes (e.g., onclick, onmouseover)
            if (Regex.IsMatch(input, @"on\w+\s*=\s*(['""]?)[^'""]+\1", RegexOptions.IgnoreCase))
                return false;

            // Detect potentially dangerous tags
            if (Regex.IsMatch(input, @"<(iframe|object|embed|link|style|meta).*?>", RegexOptions.IgnoreCase))
                return false;

            return true;
        }

        public static bool ContainsXss(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            string[] xssPatterns = {
                @"<script.*?>.*?</script>",
                @"on\w+\s*=\s*(['""]?)[^'""]+\1",
                @"<(iframe|object|embed|link|style|meta).*?>",
                @"javascript\s*:",
                @"eval\s*\(",
                @"document\.cookie",
                @"document\.write",
                @"innerHTML\s*=",
                @"src\s*=\s*['""]?javascript:",
                @"expression\s*\(",
                @"vbscript\s*:"
            };

            return xssPatterns.Any(pattern => Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase));
        }
    }
}