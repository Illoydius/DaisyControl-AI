namespace DaisyControl_AI.Common.Exceptions.HTTP
{
    public class WebApiException : CommonException
    {
        public WebApiException(string errorCode, string message, Exception innerException = null) : base(errorCode, message, innerException)
        {
        }
    }
}
