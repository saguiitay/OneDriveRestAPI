using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using OneDriveRestAPI.Model;
using File = OneDriveRestAPI.Model.File;
using FileInfo = OneDriveRestAPI.Model.FileInfo;

namespace OneDriveRestAPI
{
    public interface IClient
    {
        string UserAccessToken { get; set; }
        string UserRefreshToken { get; set; }

        IRequestGenerator RequestGenerator { get; set; }
        
        string GetAuthorizationRequestUrl(IEnumerable<Scope> requestedScopes, string state = null);
        Task<UserToken> GetAccessTokenAsync(string authorizationCode);
        Task<UserToken> RefreshAccessTokenAsync();

        Task<User> GetMeAsync();
        Task<string> GetProfilePictureAsync(PictureSize size = PictureSize.Medium);
        Task<Folder> GetFolderAsync(string id = null);
        Task<IEnumerable<File>> GetContentsAsync(string id = null);
        Task<File> GetFileAsync(string id = null);
        Task<Folder> CreateFolderAsync(string parentFolderId, string name, string description = null);
        Task<Stream> DownloadAsync(string id);
        Task<FileInfo> UploadAsync(string parentFolderId, Stream content, string name, OverwriteOption overwrite = OverwriteOption.DoNotOverwrite, bool downsizePhotoUpload = false, bool checkForQuota = false);
        Task<File> CopyAsync(string sourceId, string newParentId);
        Task<File> RenameFileAsync(string id, string name);
        Task<Folder> RenameFolderAsync(string id, string name);
        Task<File> MoveFileAsync(string id, string newParentId);
        Task<Folder> MoveFolderAsync(string id, string newParentId);
        Task DeleteAsync(string id);
        Task<UserQuota> QuotaAsync();
    }
}