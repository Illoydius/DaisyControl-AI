﻿namespace DaisyControl_AI.Common.Exceptions.HTTP
{
    public class BadRequestWebApiException : WebApiException
    {
        public BadRequestWebApiException(int errorCode, string message, Exception innerException = null) : base(errorCode, message, innerException)
        {
        }
    }
}