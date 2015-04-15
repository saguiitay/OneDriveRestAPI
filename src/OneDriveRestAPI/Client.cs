using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using OneDriveRestAPI.Http;
using OneDriveRestAPI.Model;
using OneDriveRestAPI.Model.Exceptions;
using File = OneDriveRestAPI.Model.File;
using FileInfo = OneDriveRestAPI.Model.FileInfo;

namespace OneDriveRestAPI
{
    public partial class Client : IClient
    {
        private readonly HttpClient _clientOAuth;
        private readonly HttpClient _clientContent;
        private readonly HttpClient _clientContentNoRedirection;

        private readonly Options _options;

        public IRequestGenerator RequestGenerator { get; set; }

        public string UserAccessToken
        {
            get { return _options.AccessToken; }
            set { _options.AccessToken = value; }
        }

        public string UserRefreshToken
        {
            get { return _options.RefreshToken; }
            set { _options.RefreshToken = value; }
        }


        public Client(Options options)
            : this(new HttpClientFactory(), new RequestGenerator(), options)
        { }

        public Client(IHttpClientFactory clientFactory, IRequestGenerator requestGenerator, Options options)
        {
            _options = options;

            _clientOAuth = clientFactory.CreateHttpClient(new HttpClientOptions());
            _clientContent = clientFactory.CreateHttpClient(new HttpClientOptions {AddTokenToRequests = true, TokenRetriever = () => _options.AccessToken});
            _clientContentNoRedirection = clientFactory.CreateHttpClient(new HttpClientOptions {AllowAutoRedirect = false, AddTokenToRequests = true, TokenRetriever = () => _options.AccessToken});


            RequestGenerator = requestGenerator;
        }


        public string GetAuthorizationRequestUrl(IEnumerable<Scope> requestedScopes, string state = null)
        {
            var request = RequestGenerator.Authorize(_options.ClientId, _options.CallbackUrl, requestedScopes, state);
            return request.BuildUri().AbsoluteUri;
        }

        public async Task<UserToken> GetAccessTokenAsync(string authorizationCode)
        {
            var getAccessToken = RequestGenerator.GetAccessToken(_options.ClientId, _options.ClientSecret, _options.CallbackUrl, authorizationCode);
            var token = await ExecuteAuthorization<UserToken>(getAccessToken).ConfigureAwait(false);

            _options.AccessToken = token.Access_Token;
            _options.RefreshToken = token.Refresh_Token;

            return token;
        }

        public async Task<UserToken> RefreshAccessTokenAsync()
        {
            var refreshAccessToken = RequestGenerator.RefreshAccessToken(_options.ClientId, _options.ClientSecret, _options.CallbackUrl, UserRefreshToken);
            var token = await ExecuteAuthorization<UserToken>(refreshAccessToken).ConfigureAwait(false);

            _options.AccessToken = token.Access_Token;
            _options.RefreshToken = token.Refresh_Token;

            return token;
        }

        public async Task<User> GetMeAsync()
        {
            return await Execute<User>(() => RequestGenerator.GetMe()).ConfigureAwait(false);
        }

        public async Task<string> GetProfilePictureAsync(PictureSize size = PictureSize.Medium)
        {
            var response = await Execute(() => RequestGenerator.GetProfilePicture(size), _clientContentNoRedirection).ConfigureAwait(false);
            var locationHeader = response.Headers.Location;
            if (locationHeader != null)
                return locationHeader.AbsoluteUri;
            return null;
        }

        public async Task<Folder> GetFolderAsync(string id = null)
        {
            return await Execute<Folder>(() => RequestGenerator.Get(id)).ConfigureAwait(false);
        }

        public async Task<IEnumerable<File>> GetContentsAsync(string id = null)
        {
            var result = await Execute<File>(() => RequestGenerator.GetContents(id)).ConfigureAwait(false);
            return result.Data;
        }

        public async Task<File> GetFileAsync(string id = null)
        {
            return await Execute<File>(() => RequestGenerator.Get(id)).ConfigureAwait(false);
        }

        public async Task<Folder> CreateFolderAsync(string parentFolderId, string name, string description = null)
        {
            return await Execute<Folder>(() => RequestGenerator.CreateFolder(parentFolderId, name, description)).ConfigureAwait(false);
        }

        public async Task<Stream> DownloadAsync(string id)
        {
            HttpResponseMessage restResponse = await Execute(() => RequestGenerator.Read(id), _clientContentNoRedirection).ConfigureAwait(false);

            var locationHeader = restResponse.Headers.Location;
            if (locationHeader != null)
            {
                var stream = await _clientContent.GetStreamAsync(locationHeader.AbsoluteUri).ConfigureAwait(false);

                return stream;
            }

            return null;
        }

        public async Task<FileInfo> UploadAsync(string parentFolderId, Stream content, string name, OverwriteOption overwrite = OverwriteOption.DoNotOverwrite, bool downsizePhotoUpload = false, bool checkForQuota = false)
        {
            if (checkForQuota)
            {
                var quota = await QuotaAsync().ConfigureAwait(false);
                if (quota.Available <= content.Length)
                    throw new NotEnoughQuotaException();
            }

            return await Execute<FileInfo>(() => RequestGenerator.Upload(parentFolderId, name, content, overwrite, downsizePhotoUpload)).ConfigureAwait(false);
        }

        public async Task<File> CopyAsync(string sourceId, string newParentId)
        {
            return await Execute<File>(() => RequestGenerator.Copy(sourceId, newParentId)).ConfigureAwait(false);
        }

        public async Task<File> RenameFileAsync(string id, string name)
        {
            return await Execute<File>(() => RequestGenerator.Rename(id, name)).ConfigureAwait(false);
        }

        public async Task<Folder> RenameFolderAsync(string id, string name)
        {
            return await Execute<Folder>(() => RequestGenerator.Rename(id, name)).ConfigureAwait(false);
        }

        public async Task<File> MoveFileAsync(string id, string newParentId)
        {
            return await Execute<File>(() => RequestGenerator.Move(id, newParentId)).ConfigureAwait(false);
        }

        public async Task<Folder> MoveFolderAsync(string id, string newParentId)
        {
            return await Execute<Folder>(() => RequestGenerator.Move(id, newParentId)).ConfigureAwait(false);
        }

        public async Task DeleteAsync(string id)
        {
            await Execute(() => RequestGenerator.Delete(id)).ConfigureAwait(false);
        }

        public async Task<UserQuota> QuotaAsync()
        {
            return await Execute<UserQuota>(() => RequestGenerator.Quota()).ConfigureAwait(false);
        }

        public async Task<IEnumerable<File>> SearchAsync(string pattern)
        {
            var result = await Execute<File>(() => RequestGenerator.Search(pattern)).ConfigureAwait(false);
            return result.Data;
        }
    }
}