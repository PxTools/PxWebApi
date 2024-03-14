using System.Text.RegularExpressions;

namespace PxWeb.Code
{
    public class LanguageUtil
    {
        public static bool HasValidLanguageCodePattern(string languageCode)
        {
            //Language code are either XX or XX-XX
            var pattern = @"^[a-z]{2}-[a-z]{2}$|^[a-z]{2}$";
            return Regex.IsMatch(languageCode, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        public static string SanitizeLangueCode(string languageCode)
        {
            return Regex.Replace(languageCode, @"[^a-zA-Z-]+", "?");
        }
    }
}
