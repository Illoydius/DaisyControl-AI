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
    }
}
