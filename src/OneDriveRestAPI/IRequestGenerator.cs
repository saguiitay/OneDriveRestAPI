using System.Collections.Generic;
using System.IO;
using OneDriveRestAPI.Model;
using OneDriveRestAPI.Util;

namespace OneDriveRestAPI
{
    public interface IRequestGenerator
    {
        IRequest Authorize(string clientId, string callbackUrl, IEnumerable<Scope> requestedScopes, string state = null);
        IRequest GetAccessToken(string clientId, string clientSecret, string callbackUrl, string authorizationCode);
        IRequest RefreshAccessToken(string clientId, string clientSecret, string callbackUrl, string refreshToken);
        IRequest Get(string id);
        IRequest GetMe();
        IRequest GetProfilePicture(PictureSize size = PictureSize.Medium);
        IRequest GetContents(string id, bool pretty = false);
        IRequest CreateFolder(string parentFolderId, string name, string description = null);
        IRequest Copy(string sourceId, string newParentId);
        IRequest Delete(string id);
        IRequest Write(string parentFolderId, string name, Stream content, string contentType);
        IRequest Upload(string parentFolderId, string name, Stream content, OverwriteOption overwrite = OverwriteOption.Overwrite, bool downsizePhotoUpload = false);
        IRequest Read(string id, ulong startByte = 0, ulong? endByte = null);
        IRequest Move(string sourceId, string newParentId);
        IRequest Rename(string id, string name);
        IRequest Quota();
    }
}