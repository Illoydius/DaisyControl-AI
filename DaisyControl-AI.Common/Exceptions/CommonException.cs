namespace DaisyControl_AI.Common.Exceptions
{
    /// <summary>
    /// Common basic exception.
    /// </summary>
    public class CommonException : Exception
    {
        public CommonException(string errorCode, string message, Exception innerException = null) :
            base($"{DateTime.UtcNow:yyyy-MM-dd mm:HH:ss fffff} - [{errorCode}] - {message}", innerException)
        { }
    }
}
