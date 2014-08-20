using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using OneDriveRestAPI.Model;
using OneDriveRestAPI.Model.Exceptions;
using OneDriveRestAPI.Util;
using File = OneDriveRestAPI.Model.File;
using FileInfo = OneDriveRestAPI.Model.FileInfo;

namespace OneDriveRestAPI
{
    public partial class Client
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _callbackUrl;

        private readonly HttpClient _clientOAuth;
        private readonly HttpClient _clientContent;
        private readonly HttpClient _clientContentNoRedirection;

        private UserToken _token;

        public UserToken UserToken { get { return _token; } }
        public IRequestGenerator RequestGenerator { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId">Application ClientId</param>
        /// <param name="clientSecret">Application secret</param>
        /// <param name="callbackUrl">Application Callback</param>
        /// <param name="accessToken">User AccessToken</param>
        /// <param name="refreshToken">User RefreshToken</param>
        /// <param name="handlerWrapper">Function that allows wrapping the various HttpMessageHandlers used to communicate with the API</param>
        public Client(string clientId, string clientSecret, string callbackUrl, string accessToken = null, string refreshToken = null,
            Func<HttpMessageHandler, HttpMessageHandler> handlerWrapper = null)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _callbackUrl = callbackUrl;

            var oauthHandler = new HttpClientHandler();
            if (oauthHandler.SupportsAutomaticDecompression)
                oauthHandler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            if (handlerWrapper != null)
                _clientOAuth = new HttpClient(handlerWrapper(oauthHandler));
            else
                _clientOAuth = new HttpClient(oauthHandler);


            var contentHandler = new HttpClientHandler();
            if (contentHandler.SupportsAutomaticDecompression)
                contentHandler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            var tokenHandler = new AccessTokenAuthenticator(() => _token.Access_Token, contentHandler);
            if (handlerWrapper != null)
                _clientContent = new HttpClient(handlerWrapper(tokenHandler));
            else
                _clientContent = new HttpClient(tokenHandler);

            var contentNoRedirectionHandler = new HttpClientHandler { AllowAutoRedirect = false };
            if (contentNoRedirectionHandler.SupportsAutomaticDecompression)
                contentNoRedirectionHandler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            var tokenNoRedirectionHandler = new AccessTokenAuthenticator(() => _token.Access_Token, contentNoRedirectionHandler);
            if (handlerWrapper != null)
                _clientContentNoRedirection = new HttpClient(handlerWrapper(tokenNoRedirectionHandler));
            else
                _clientContentNoRedirection = new HttpClient(tokenNoRedirectionHandler);


            RequestGenerator = new RequestGenerator();

            _token = new UserToken { Access_Token = accessToken, Refresh_Token = refreshToken };
        }


        public string GetAuthorizationRequestUrl(IEnumerable<Scope> requestedScopes, string state = null)
        {
            var request = RequestGenerator.Authorize(_clientId, _callbackUrl, requestedScopes, state);
            return request.BuildUri().AbsoluteUri;

        }


        public async Task<UserToken> GetAccessTokenAsync(string authorizationCode)
        {
            var getAccessToken = RequestGenerator.GetAccessToken(_clientId, _clientSecret, _callbackUrl, authorizationCode);
            var token = await ExecuteAuthorization<UserToken>(getAccessToken);

            Interlocked.Exchange(ref _token, token);
            return token;
        }

        public async Task<UserToken> RefreshAccessTokenAsync()
        {
            var refreshAccessToken = RequestGenerator.RefreshAccessToken(_clientId, _clientSecret, _callbackUrl, _token.Refresh_Token);
            var token = await ExecuteAuthorization<UserToken>(refreshAccessToken);
            Interlocked.Exchange(ref _token, token);

            return token;
        }

        public async Task<File> GetAsync(string id = null)
        {
            return await Execute<File>(() => RequestGenerator.Get(id));
        }

        public async Task<User> GetMeAsync()
        {
            return await Execute<User>(() => RequestGenerator.GetMe());
        }

        public async Task<string> GetProfilePictureAsync(PictureSize size = PictureSize.Medium)
        {
            var response = await Execute(() => RequestGenerator.GetProfilePicture(size), _clientContentNoRedirection);
            var locationHeader = response.Headers.Location;
            if (locationHeader != null)
                return locationHeader.AbsoluteUri;
            return null;
        }

        public async Task<IEnumerable<File>> GetContentsAsync(string id = null)
        {
            var result = await Execute<File>(() => RequestGenerator.GetContents(id));
            return result.Data;
        }

        public async Task<Folder> CreateFolderAsync(string parentFolderId, string name, string description = null)
        {
            return await Execute<Folder>(() => RequestGenerator.CreateFolder(parentFolderId, name, description));
        }

        public async Task<File> CreateFileAsync(string parentFolderId, string name, string contentType)
        {
            return await WriteAsync(parentFolderId, new byte[0], name, contentType);
        }

        public async Task<File> WriteAsync(string parentFolderId, byte[] content, string name, string contentType)
        {
            using (var stream = new MemoryStream(content))
            {
                return await WriteAsync(parentFolderId, stream, name, contentType);
            }
        }

        public async Task<File> WriteAsync(string parentFolderId, Stream content, string name, string contentType)
        {
            return await Execute<File>(() => RequestGenerator.Write(parentFolderId, name, content, contentType));
        }

        public async Task<byte[]> Read(string id, ulong startByte = 0, ulong? endByte = null)
        {
            var response = await Execute(() => RequestGenerator.Read(id, startByte, endByte));
            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<byte[]> ReadAsync(string id, ulong startByte = 0, ulong? endByte = null)
        {
            var response = await Execute(() => RequestGenerator.Read(id, startByte, endByte));
            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<Stream> DownloadAsync(string id)
        {
            HttpResponseMessage restResponse = await Execute(() => RequestGenerator.Read(id), _clientContentNoRedirection);

            var locationHeader = restResponse.Headers.Location;
            if (locationHeader != null)
            {
                var stream = await _clientContent.GetStreamAsync(locationHeader.AbsoluteUri);

                return stream;
            }

            return null;
        }

        public async Task<FileInfo> UploadAsync(string parentFolderId, Stream content, string name, OverwriteOption overwrite = OverwriteOption.DoNotOverwrite, bool downsizePhotoUpload = false, bool checkForQuota = false)
        {
            if (checkForQuota)
            {
                var quota = await QuotaAsync();
                if (quota.Available <= content.Length)
                    throw new NotEnoughQuotaException();
            }

            return await Execute<FileInfo>(() => RequestGenerator.Upload(parentFolderId, name, content, overwrite, downsizePhotoUpload));
        }


        public async Task<File> CopyAsync(string sourceId, string newParentId)
        {
            return await Execute<File>(() => RequestGenerator.Copy(sourceId, newParentId));
        }

        public async Task RenameAsync(string id, string name)
        {
            await Execute(() => RequestGenerator.Rename(id, name));
        }

        public async Task<File> RenameFileAsync(string id, string name)
        {
            return await Execute<File>(() => RequestGenerator.Rename(id, name));
        }

        public async Task<Folder> RenameFolder(string id, string name)
        {
            return await Execute<Folder>(() => RequestGenerator.Rename(id, name));
        }

        public async Task<Folder> RenameFolderAsync(string id, string name)
        {
            return await Execute<Folder>(() => RequestGenerator.Rename(id, name));
        }

        public async Task MoveAsync(string id, string newParentId)
        {
            await Execute(() => RequestGenerator.Move(id, newParentId));
        }

        public async Task<File> MoveFileAsync(string id, string newParentId)
        {
            return await Execute<File>(() => RequestGenerator.Move(id, newParentId));
        }

        public async Task<Folder> MoveFolderAsync(string id, string newParentId)
        {
            return await Execute<Folder>(() => RequestGenerator.Move(id, newParentId));
        }


        public async Task DeleteAsync(string id)
        {
            await Execute(() => RequestGenerator.Delete(id));
        }

        public async Task<UserQuota> QuotaAsync()
        {
            return await Execute<UserQuota>(() => RequestGenerator.Quota());
        }
    }
}