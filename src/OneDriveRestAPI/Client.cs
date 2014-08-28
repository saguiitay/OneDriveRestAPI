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
            var token = await ExecuteAuthorization<UserToken>(getAccessToken);

            _options.AccessToken = token.Access_Token;
            _options.RefreshToken = token.Refresh_Token;

            return token;
        }

        public async Task<UserToken> RefreshAccessTokenAsync()
        {
            var refreshAccessToken = RequestGenerator.RefreshAccessToken(_options.ClientId, _options.ClientSecret, _options.CallbackUrl, UserRefreshToken);
            var token = await ExecuteAuthorization<UserToken>(refreshAccessToken);

            _options.AccessToken = token.Access_Token;
            _options.RefreshToken = token.Refresh_Token;

            return token;
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

        public async Task<Folder> GetFolderAsync(string id = null)
        {
            return await Execute<Folder>(() => RequestGenerator.Get(id));
        }

        public async Task<IEnumerable<File>> GetContentsAsync(string id = null)
        {
            var result = await Execute<File>(() => RequestGenerator.GetContents(id));
            return result.Data;
        }

        public async Task<File> GetFileAsync(string id = null)
        {
            return await Execute<File>(() => RequestGenerator.Get(id));
        }

        public async Task<Folder> CreateFolderAsync(string parentFolderId, string name, string description = null)
        {
            return await Execute<Folder>(() => RequestGenerator.CreateFolder(parentFolderId, name, description));
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

        public async Task<File> RenameFileAsync(string id, string name)
        {
            return await Execute<File>(() => RequestGenerator.Rename(id, name));
        }

        public async Task<Folder> RenameFolderAsync(string id, string name)
        {
            return await Execute<Folder>(() => RequestGenerator.Rename(id, name));
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