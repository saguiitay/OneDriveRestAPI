using System.Net.Http;
using System.Threading.Tasks;

namespace OneDriveRestAPI.Util
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> Execute(this HttpClient client, IRequest request)
        {
            var requestMessage = new HttpRequestMessage(request.Method, request.BuildUri());
            if (request.Content != null)
                requestMessage.Content = request.Content;

            if (request.Headers != null)
            {
                foreach (var header in request.Headers)
                {
                    requestMessage.Headers.Add(header.Key, header.Value);
                }
            }

            return await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
        }
    }
}