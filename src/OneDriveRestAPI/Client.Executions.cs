using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OneDriveRestAPI.Model;
using OneDriveRestAPI.Model.Exceptions;
using OneDriveRestAPI.Util;

namespace OneDriveRestAPI
{
    public partial class Client
    {
        #region Request execution utilities methods

        private async Task<T> ExecuteAuthorization<T>(IRequest restRequest) where T : new()
        {
            return await Execute<T>(() => restRequest, _clientOAuth).ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> Execute(Func<IRequest> restRequest, HttpClient restClient = null)
        {
            if (restClient == null)
                restClient = _clientContent;

            HttpResponseMessage restResponse = null;
            bool refresh = false;
            try
            {
                var request = restRequest();

                restResponse = await restClient.Execute(request).ConfigureAwait(false);
                await CheckForError(restResponse).ConfigureAwait(false);
            }
            catch (TokenExpiredException)
            {
                refresh = true;

            }
            if (refresh)
            {
                await RefreshAccessTokenAsync().ConfigureAwait(false);

                var request = restRequest();
                restResponse = await restClient.Execute(request).ConfigureAwait(false);
                await CheckForError(restResponse).ConfigureAwait(false);
            }

            return restResponse;
        }

        private async Task<T> Execute<T>(Func<IRequest> restRequest, HttpClient restClient = null) where T : new()
        {
            if (restClient == null)
                restClient = _clientContent;

            var content = "";
            HttpResponseMessage restResponse;
            bool refresh = false;
            try
            {
                var request = restRequest();

                restResponse = await restClient.Execute(request).ConfigureAwait(false);
                content = await CheckForError(restResponse).ConfigureAwait(false);
            }
            catch (TokenExpiredException)
            {
                refresh = true;
            }

            if (refresh)
            {
                await RefreshAccessTokenAsync().ConfigureAwait(false);

                var request = restRequest();

                restResponse = await restClient.Execute(request).ConfigureAwait(false);
                content = await CheckForError(restResponse).ConfigureAwait(false);
            }

            var data = JsonConvert.DeserializeObject<T>(content);

            return data;
        }

        private async Task<string> CheckForError(HttpResponseMessage httpResponse)
        {
            var statusCode = httpResponse.StatusCode;
            var content = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (statusCode == 0)
                throw new HttpServerException((int)statusCode, content) { Attempts = 1 };

            if ((int)statusCode == 420)
            {
                if (httpResponse.Headers != null)
                {
                    var retryAfter = httpResponse.Headers.FirstOrDefault(x => x.Key == "Retry-After");
                    if (retryAfter.Value != null && retryAfter.Value.Any())
                    {
                        throw new RetryLaterException { RetryAfter = int.Parse(retryAfter.Value.First()) };
                    }
                }
                throw new RetryLaterException { RetryAfter = 10 };
            }
            if (statusCode == HttpStatusCode.Unauthorized ||
                statusCode == HttpStatusCode.BadRequest ||
                statusCode == HttpStatusCode.ServiceUnavailable)
            {
                var errorInfo = JsonConvert.DeserializeObject<ErrorInfo>(content);
                if (errorInfo == null || errorInfo.Error == null)
                    throw new HttpServerException((int)statusCode, content) { Attempts = 1 };

                if (errorInfo.Error.Code == "request_token_expired")
                    throw new TokenExpiredException();
                if (errorInfo.Error.Code == "server_busy")
                    throw new RetryLaterException { RetryAfter = 5 };
                throw new ServiceErrorException(errorInfo.Error.Code, errorInfo.Error.Message);
            }
            if (statusCode == HttpStatusCode.InternalServerError ||
                statusCode == HttpStatusCode.BadGateway)
            {
                throw new HttpServerException((int)statusCode, content) { Attempts = int.MaxValue };
            }

            return content;
        }

        #endregion
    }
}