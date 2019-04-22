using System;
using System.Net;
using System.Net.Http;

namespace ApiClient.Exceptions
{
    public class ApiException : Exception

    {
    public ApiException(Uri requestUri, HttpMethod httpMethod, string jsonRequestString, HttpStatusCode responseStatusCode, string responseBody)
        : base(
            $"API call failed when sending a '{httpMethod.Method}' request to '{requestUri.AbsolutePath}' with the following data: '{jsonRequestString}'. Received a '{responseStatusCode}' status code with the following content: '{responseBody ?? string.Empty}'.")
    {
    }
    }
}