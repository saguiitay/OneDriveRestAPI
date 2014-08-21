using System;
using System.Net;
using System.Net.Http;
using OneDriveRestAPI.Util;

namespace OneDriveRestAPI.Http
{
    public class HttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateHttpClient(HttpClientOptions options)
        {
            HttpMessageHandler handler = new HttpClientHandler { AllowAutoRedirect = options.AllowAutoRedirect };
            if (((HttpClientHandler)handler).SupportsAutomaticDecompression)
                ((HttpClientHandler)handler).AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            if (options.AddTokenToRequests)
                handler = new AccessTokenAuthenticator(options.TokenRetriever, handler);

            TimeSpanSemaphore readTimeSpanSemaphore = null;
            TimeSpanSemaphore writeTimeSpanSemaphore = null;
            if (options.ReadRequestsPerSecond.HasValue)
            {
                var delay = (1/options.ReadRequestsPerSecond.Value);
                readTimeSpanSemaphore = new TimeSpanSemaphore(1, TimeSpan.FromSeconds(delay));
            }
            if (options.WriteRequestsPerSecond.HasValue)
            {
                var delay = (1 / options.WriteRequestsPerSecond.Value);
                writeTimeSpanSemaphore = new TimeSpanSemaphore(1, TimeSpan.FromSeconds(delay));
            }

            if (readTimeSpanSemaphore != null || writeTimeSpanSemaphore != null)
            {
                handler = new ThrottlingMessageHandler(readTimeSpanSemaphore, writeTimeSpanSemaphore, handler);
            }
            return new HttpClient(handler);
        }
    }
}