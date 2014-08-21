using System;
using System.Net.Http;

namespace OneDriveRestAPI.Http
{
    public interface IHttpClientFactory
    {
        HttpClient CreateHttpClient(HttpClientOptions options);
    }
}
