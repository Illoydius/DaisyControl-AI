
using System.Text.RegularExpressions;

namespace DaisyControl_AI.Common.Utils
{
    public static class StringUtils
    {
        public static string CapitalizeFirstLetter(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            input = input.ToLowerInvariant();
            return char.ToUpper(input[0]) + input.Substring(1);
        }

        public static string GetJsonFromString(string inputString)
        {
            if (string.IsNullOrWhiteSpace(inputString))
            {
                return "";
            }

            string pattern = @"\{(?:[^{}]|(?<Open>\{)|(?<-Open>\}))*\}";

            Match match = Regex.Match(inputString, pattern);
            if (match.Success)
            {
                return match.Value;
            } else
            {
                return inputString;
            }
        }
    }
}
