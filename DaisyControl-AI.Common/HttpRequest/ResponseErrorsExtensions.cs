using System.Text.Json;

namespace DaisyControl_AI.Common.HttpRequest
{
    public static class ResponseErrorsExtensions
    {
        public static string AsJson(this ResponseError responseError)
        {
            return JsonSerializer.Serialize(responseError);
        }

        public static ResponseError AsResponseError(this string serializedResponseError)
        {
            try
            {
                return JsonSerializer.Deserialize<ResponseError>(serializedResponseError);
            } catch (Exception)
            {
                return null;
            }
        }
    }
}
