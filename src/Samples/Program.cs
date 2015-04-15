using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OneDriveRestAPI;
using OneDriveRestAPI.Model;
using File = OneDriveRestAPI.Model.File;
using System.Threading;

namespace Samples
{
    class Program
    {
        [STAThread]
        private static void Main()
        {
            bool running = true;
            try
            {
                Run().ContinueWith((task) => { running = false; });
                while (running) {
                    Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadKey();
        }

        public static async Task Run()
        {
            var options = new Options
                {
                    ClientId = "...",
                    ClientSecret = "...",
                    CallbackUrl = "https://login.live.com/oauth20_desktop.srf",

                    AutoRefreshTokens = true,
                    PrettyJson = false,
                    ReadRequestsPerSecond = 2,
                    WriteRequestsPerSecond = 2
                };

            // Initialize a new Client (without an Access/Refresh tokens
            var client = new Client(options);

            // Get the OAuth Request Url
            var authRequestUrl = client.GetAuthorizationRequestUrl(new[] {Scope.Basic, Scope.Signin, Scope.SkyDrive, Scope.SkyDriveUpdate});

            // Navigate to authRequestUrl using the browser, and retrieve the Authorization Code from the response
            var authCode = await GetAuthCode(client, new[] { Scope.Signin, Scope.Basic, Scope.SkyDrive });

            // Exchange the Authorization Code with Access/Refresh tokens
            var token = await client.GetAccessTokenAsync(authCode);

            // Get user profile
            var userProfile = await client.GetMeAsync();
            Console.WriteLine("Name: " + userProfile.Name);
            string preferredEmail = userProfile.Emails == null ? "(n/a)" : userProfile.Emails.Preferred;
            Console.WriteLine("Preferred Email: " + preferredEmail);

            // Get user photo
            var userProfilePicture = await client.GetProfilePictureAsync(PictureSize.Small);
            Console.WriteLine("Avatar: " + userProfilePicture);

            // Retrieve the root folder
            var rootFolder = await client.GetFolderAsync();
            Console.WriteLine("Root Folder: {0} (Id: {1})", rootFolder.Name, rootFolder.Id);

            // Retrieve the content of the root folder
            var folderContent = await client.GetContentsAsync(rootFolder.Id);
            foreach (var item in folderContent)
            {
                Console.WriteLine("\tItem ({0}: {1} (Id: {2})", item.Type, item.Name, item.Id);
            }


            // Initialize a new Client, this time by providing previously requested Access/Refresh tokens
            options.AccessToken = token.Access_Token;
            options.RefreshToken = token.Refresh_Token;
            var client2 = new Client(options);

            // Search for a file by pattern (e.g. *.docx for MS Word documents)
            //TODO: Uncomment the below when PR #5 is merged
            //var wordDocuments = await client.SearchAsync("*.docx");
            //Console.WriteLine(string.Format("Found {0} Word documents", wordDocuments.Count()));

            // Find a file in the root folder
            var file = folderContent.FirstOrDefault(x => x.Type == File.FileType);

            // Download file to a temporary local file
            var tempFile = Path.GetTempFileName();
            using (var fileStream = System.IO.File.OpenWrite(tempFile))
            {
                var contentStream = await client2.DownloadAsync(file.Id);
                await contentStream.CopyToAsync(fileStream);
            }


            // Upload the file with a new name
            using (var fileStream = System.IO.File.OpenRead(tempFile))
            {
                await client2.UploadAsync(rootFolder.Id, fileStream, "Copy Of " + file.Name);
            }

        }

        private static Task<string> GetAuthCode(Client client, Scope[] scopes)
        {
            TaskCompletionSource<string> completion = new TaskCompletionSource<string>();
            string code = null;

            //var authUriString = string.Format("https://login.live.com/oauth20_authorize.srf?client_id={0}&scope={1}&response_type=code&redirect_uri={2}", clientId, Uri.EscapeDataString(scopesString), Uri.EscapeDataString(redirectUri));
            var authUriString = client.GetAuthorizationRequestUrl(scopes);

            var browser = new BrowserWindow();
            browser.Navigating += (sender, eventArgs) => Console.WriteLine("Navigating: " + eventArgs.Uri);
            browser.Navigated += (sender, eventArgs) =>
            {
                /*
                 * Navigating: https://login.live.com/oauth20_authorize.srf?client_id=00000000480FBA5F&redirect_uri=https:%2F%2Flogin.live.com%2Foauth20_desktop.srf&scope=wl.signin wl.basic wl.skydrive&response_type=code&display=windesktop&locale=en-US&state=&theme=win7
                 * Navigated: https://login.live.com/oauth20_authorize.srf?client_id=00000000480FBA5F&redirect_uri=https:%2F%2Flogin.live.com%2Foauth20_desktop.srf&scope=wl.signin wl.basic wl.skydrive&response_type=code&display=windesktop&locale=en-US&state=&theme=win7
                 * Navigating: https://account.live.com/Consent/Update?ru=https://login.live.com/oauth20_authorize.srf%3flc%3d1033%26client_id%3d00000000480FBA5F%26redirect_uri%3dhttps%253A%252F%252Flogin.live.com%252Foauth20_desktop.srf%26scope%3dwl.signin%2520wl.basic%2520wl.skydrive%26response_type%3dcode%26display%3dwindesktop%26locale%3den-US%26state%3d%26theme%3dwin7%26mkt%3dEN-US%26scft%3dCtXDfhkaGJ68YBd6T1M!t4Qm3mQ31srlcyMC3z7hrYSNVo6nVNA0HXdY4BF8JWZDQYLjyoldXbRA3zPqAmhyEUDnX9BcDwStjFHkBQ2J7I!NqdNbiwXMngTKIt4C3fDQjccXbx3RMIE41mpmvSG6saU%2524&mkt=EN-US&uiflavor=windesktop&id=279469&client_id=00000000480FBA5F&scope=wl.signin+wl.basic+wl.skydrive

                 * Navigated: https://account.live.com/Consent/Update?ru=https://login.live.com/oauth20_authorize.srf%3flc%3d1033%26client_id%3d00000000480FBA5F%26redirect_uri%3dhttps%253A%252F%252Flogin.live.com%252Foauth20_desktop.srf%26scope%3dwl.signin%2520wl.basic%2520wl.skydrive%26response_type%3dcode%26display%3dwindesktop%26locale%3den-US%26state%3d%26theme%3dwin7%26mkt%3dEN-US%26scft%3dCtXDfhkaGJ68YBd6T1M!t4Qm3mQ31srlcyMC3z7hrYSNVo6nVNA0HXdY4BF8JWZDQYLjyoldXbRA3zPqAmhyEUDnX9BcDwStjFHkBQ2J7I!NqdNbiwXMngTKIt4C3fDQjccXbx3RMIE41mpmvSG6saU%2524&mkt=EN-US&uiflavor=windesktop&id=279469&client_id=00000000480FBA5F&scope=wl.signin+wl.basic+wl.skydrive
                 * Navigating: https://account.live.com/Consent/Update?ru=https://login.live.com/oauth20_authorize.srf%3flc%3d1033%26client_id%3d00000000480FBA5F%26redirect_uri%3dhttps%253A%252F%252Flogin.live.com%252Foauth20_desktop.srf%26scope%3dwl.signin%2520wl.basic%2520wl.skydrive%26response_type%3dcode%26display%3dwindesktop%26locale%3den-US%26state%3d%26theme%3dwin7%26mkt%3dEN-US%26scft%3dCtXDfhkaGJ68YBd6T1M!t4Qm3mQ31srlcyMC3z7hrYSNVo6nVNA0HXdY4BF8JWZDQYLjyoldXbRA3zPqAmhyEUDnX9BcDwStjFHkBQ2J7I!NqdNbiwXMngTKIt4C3fDQjccXbx3RMIE41mpmvSG6saU%2524&mkt=EN-US&uiflavor=windesktop&id=279469&client_id=00000000480FBA5F&scope=wl.signin+wl.basic+wl.skydrive

                 * Navigating: https://login.live.com/oauth20_authorize.srf?lc=1033&client_id=00000000480FBA5F&redirect_uri=https:%2f%2flogin.live.com%2foauth20_desktop.srf&scope=wl.signin+wl.basic+wl.skydrive&response_type=code&display=windesktop&locale=en-US&state=&theme=win7&mkt=EN-US&scft=CtXDfhkaGJ68YBd6T1M!t4Qm3mQ31srlcyMC3z7hrYSNVo6nVNA0HXdY4BF8JWZDQYLjyoldXbRA3zPqAmhyEUDnX9BcDwStjFHkBQ2J7I!NqdNbiwXMngTKIt4C3fDQjccXbx3RMIE41mpmvSG6saU%24&res=success
                 * Navigating: https://login.live.com/oauth20_desktop.srf?code=4df80f16-2c7a-eb03-778e-f276c1dc95ee&lc=1033
                 * Navigated: https://login.live.com/oauth20_desktop.srf?code=4df80f16-2c7a-eb03-778e-f276c1dc95ee&lc=1033
                 */
                var uri = eventArgs.Uri.OriginalString;
                if (uri.Contains("code="))
                {
                    code = uri.Split(new[] { "code=" }, StringSplitOptions.None)[1];
                    code = code.Split(new[] { "&lc=" }, StringSplitOptions.None)[0];
                    Console.WriteLine("Authorized: " + code);
                    browser.Close();
                }
                else
                {
                    Console.WriteLine("Navigated: " + eventArgs.Uri);
                }
            };

            browser.Show(authUriString);
            completion.SetResult(code);

            return completion.Task;
        }
    }
}
