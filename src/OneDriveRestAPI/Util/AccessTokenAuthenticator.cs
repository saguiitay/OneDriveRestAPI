using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace OneDriveRestAPI.Util
{
    public class AccessTokenAuthenticator : DelegatingHandler
    {
        private readonly Func<string> _accessToken;

        public AccessTokenAuthenticator(Func<string> accessToken, HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            _accessToken = accessToken;
        }

        #region Overrides of DelegatingHandler

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var uri = new UriBuilder(request.RequestUri);
            if (string.IsNullOrEmpty(uri.Query))
                uri.Query = string.Format("access_token={0}", _accessToken());
            else
                uri.Query = uri.Query.TrimStart('?') + string.Format("&access_token={0}", _accessToken());

            request.RequestUri = uri.Uri;

            return base.SendAsync(request, cancellationToken);

        }

        #endregion
    }
}