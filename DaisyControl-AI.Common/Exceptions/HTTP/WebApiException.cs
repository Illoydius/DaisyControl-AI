namespace DaisyControl_AI.Common.Exceptions.HTTP
{
    public class WebApiException : CommonException
    {
        public WebApiException(int errorCode, string message, Exception innerException = null) : base(errorCode, message, innerException)
        {
        }
    }
}
