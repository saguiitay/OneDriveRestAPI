using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using OneDriveRestAPI.Model;
using OneDriveRestAPI.Util;

namespace OneDriveRestAPI
{
    public class RequestGenerator : IRequestGenerator
    {
        private const string OAuthUrlBase = @"https://login.live.com";
        private const string ContentUrlBase = @"https://apis.live.net/v5.0/";

        private const string OAuthResource = "oauth20_{verb}.srf";
        private const string AuthorizeVerb = "authorize";
        private const string TokenVerb = "token";

        public IRequest Authorize(string clientId, string callbackUrl, IEnumerable<Scope> requestedScopes, string state = null)
        {
            var request = new Request
            {
                BaseAddress = OAuthUrlBase,
                Resource = OAuthResource.Replace("{verb}", AuthorizeVerb),
                Method = HttpMethod.Get
            };

            request.AddParameter("client_id", clientId);
            request.AddParameter("scope", string.Join(" ", requestedScopes.OrderBy(s => s).Select(s => s.GetDescription())));
            request.AddParameter("response_type", "code");
            request.AddParameter("redirect_uri", callbackUrl);

            if (!string.IsNullOrEmpty(state))
                request.AddParameter("state", state);

            return request;
        }

        public IRequest GetAccessToken(string clientId, string clientSecret, string callbackUrl, string authorizationCode)
        {
            var nvc = new Dictionary<string, string>();
            nvc["client_id"] = clientId;
            nvc["redirect_uri"] = callbackUrl;
            nvc["client_secret"] = clientSecret;
            nvc["code"] = authorizationCode;
            nvc["grant_type"] = "authorization_code";

            var request = new Request
            {
                BaseAddress = OAuthUrlBase,
                Resource = OAuthResource.Replace("{verb}", TokenVerb),
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(nvc)
            };


            return request;
        }

        public IRequest RefreshAccessToken(string clientId, string clientSecret, string callbackUrl, string refreshToken)
        {
            var nvc = new Dictionary<string, string>();
            nvc["client_id"] = clientId;
            nvc["redirect_uri"] = callbackUrl;
            nvc["client_secret"] = clientSecret;
            nvc["refresh_token"] = refreshToken;
            nvc["grant_type"] = "refresh_token";

            return new Request
            {
                BaseAddress = OAuthUrlBase,
                Resource = OAuthResource.Replace("{verb}", TokenVerb),
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(nvc)
            };
        }

        public IRequest Get(string id)
        {
            string resource = string.IsNullOrWhiteSpace(id) ? "me/skydrive" : id;

            return ContentRequest(HttpMethod.Get, ContentUrlBase, resource);
        }

        public IRequest GetMe()
        {
            return ContentRequest(HttpMethod.Get, ContentUrlBase, "me");
        }

        public IRequest GetProfilePicture(PictureSize size = PictureSize.Medium)
        {
            var request = ContentRequest(HttpMethod.Get, ContentUrlBase, "me/picture");
            request.AddParameter("type", size.GetDescription());

            return request;
        }

        public IRequest GetContents(string id, bool pretty = false)
        {
            Request request = string.IsNullOrWhiteSpace(id)
                ? ContentRequest(HttpMethod.Get, ContentUrlBase, "me/skydrive/files")
                : ContentRequest(HttpMethod.Get, ContentUrlBase, id + "/files");

            request.AddParameter("pretty", pretty.ToString());

            return request;
        }

        public IRequest CreateFolder(string parentFolderId, string name, string description = null)
        {
            var resource = string.IsNullOrWhiteSpace(parentFolderId) ? "me/skydrive" : parentFolderId;
            Request request = ContentRequest(HttpMethod.Post, ContentUrlBase, resource);

            request.Content = new JsonContent(new { name, description });
            return request;
        }

        public IRequest Copy(string sourceId, string newParentId)
        {
            var request = CopyMove(sourceId, newParentId, "COPY");
            return request;
        }

        public IRequest Delete(string id)
        {
            var request = ContentRequest(HttpMethod.Delete, ContentUrlBase, id);
            return request;
        }

        public IRequest Write(string parentFolderId, string name, Stream content, string contentType)
        {
            string resource = string.IsNullOrWhiteSpace(parentFolderId) ? "me/skydrive/files" : parentFolderId + "/files";
            var request = ContentRequest(HttpMethod.Post, ContentUrlBase, resource);

            request.Content = new StreamContent(content);

            return request;
        }

        public IRequest Upload(string parentFolderId, string name, Stream content, OverwriteOption overwrite = OverwriteOption.Overwrite, bool downsizePhotoUpload = false)
        {
            string resource = string.IsNullOrWhiteSpace(parentFolderId) ? "me/skydrive/files/" + name : parentFolderId + "/files/" + name;
            Request request = ContentRequest(HttpMethod.Put, ContentUrlBase, resource);
            request.AddParameter("overwrite", overwrite.GetDescription());
            request.AddParameter("downsizePhotoUpload", downsizePhotoUpload.ToString());

            if (content.CanSeek)
                content.Position = 0;
            request.Content = new CompressedContent(new StreamContent(content, 64 * 1024), "gzip");

            return request;
        }

        public IRequest Read(string id, ulong startByte = 0, ulong? endByte = null)
        {
            var request = ContentRequest(HttpMethod.Get, ContentUrlBase, id + "/content");
            if (startByte != 0 && endByte.HasValue)
                request.AddHeader("Range", string.Format("bytes={0}-{1}", startByte, endByte));
            else if (startByte != 0)
                request.AddHeader("Range", string.Format("bytes={0}-", startByte));
            return request;
        }

        public IRequest Move(string sourceId, string newParentId)
        {
            var request = CopyMove(sourceId, newParentId, "MOVE");
            return request;
        }

        private Request CopyMove(string sourceId, string newParentId, string method)
        {
            var request = ContentRequest(new HttpMethod(method), ContentUrlBase, sourceId);
            request.Content = new JsonContent(new { destination = newParentId });
            return request;
        }

        public IRequest Rename(string id, string name)
        {
            var request = ContentRequest(HttpMethod.Put, ContentUrlBase, id);
            request.Content = new JsonContent(new { name });
            return request;
        }

        public IRequest Quota()
        {
            var request = ContentRequest(HttpMethod.Get, ContentUrlBase, "me/skydrive/quota");
            return request;
        }

        private Request ContentRequest(HttpMethod method, string baseUrl, string resource)
        {
            var request = new Request
            {
                BaseAddress = baseUrl,
                Resource = Append(resource),
                Method = method
            };
            return request;
        }

        private string Append(string apendage)
        {

            return string.Format("/{0}", apendage.Trim(new[] { '/' }));
        }


    }
}