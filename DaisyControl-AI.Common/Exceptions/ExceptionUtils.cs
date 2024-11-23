namespace DaisyControl_AI.Common.Utils.MiscUtils
{
    /// <summary>
    /// Helping functions relative to exception
    /// </summary>
    public static class ExceptionUtils
    {
        public static string BuildExceptionAndInnerExceptionsMessage(Exception aException, int aSpacingLevel = 1)
        {
            if (aException == null)
                return null;

            string _Message = $"{"".PadLeft(aSpacingLevel)}[{Environment.NewLine}{"".PadLeft(aSpacingLevel)} Message=[{aException.Message}]{Environment.NewLine} {"".PadLeft(aSpacingLevel)}StackTrace=[{aException.StackTrace}]{Environment.NewLine}{"".PadLeft(aSpacingLevel)}]{Environment.NewLine}";

            if (aException.InnerException != null)
                _Message += $"{"".PadLeft(aSpacingLevel)}InnerException={Environment.NewLine}{"".PadLeft(aSpacingLevel)}[{Environment.NewLine}{BuildExceptionAndInnerExceptionsMessage(aException.InnerException, aSpacingLevel + 1)}{"".PadLeft(aSpacingLevel)}]{Environment.NewLine}";

            return _Message;
        }
    }
}
