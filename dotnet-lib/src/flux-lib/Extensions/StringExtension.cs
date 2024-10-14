using System.Text.RegularExpressions;

namespace FluxFile.Extensions
{
    public static class StringExtension
    {
        public static string ToSnakeCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }

            str = Regex.Replace(str.Trim(), @"\s+", " ");

            str = Regex.Replace(str, "([a-z])([A-Z])", "$1_$2");
            
            return Regex.Replace(str, @"\s", "_").ToLower();
        }
    }
}