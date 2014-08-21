using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OneDriveRestAPI;
using OneDriveRestAPI.Model;
using File = OneDriveRestAPI.Model.File;

namespace Samples
{
    class Program
    {
        private static void Main()
        {
            try
            {
                Run().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static async Task Run()
        {
            var options = new Options
                {
                    ClientId = "0000000040062715",
                    ClientSecret = "kJmnSnkHhIIT4dSzIAYcv7INS9rtjwRQ",
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

            // TODO: Navigate to authRequestUrl using the browser, and retrieve the Authorization Code from the response
            var authCode = "...";

            // Exchange the Authorization Code with Access/Refresh tokens
            var token = await client.GetAccessTokenAsync(authCode);

            // Get user profile
            var userProfile = await client.GetMeAsync();
            Console.WriteLine("Name: " + userProfile.Name);
            Console.WriteLine("Preferred Email: " + userProfile.Emails.Preferred);

            // Get user photo
            var userProfilePicture = await client.GetProfilePictureAsync(PictureSize.Small);
            Console.WriteLine("Avatar: " + userProfilePicture);

            // Retrieve the root folder
            var rootFolder = await client.GetAsync();
            Console.WriteLine("Root Folder: {0} (Id: {1})", rootFolder.Name, rootFolder.Id);

            // Retrieve the content of the root folder
            var folderContent = await client.GetContentsAsync(rootFolder.Id);
            foreach (var item in folderContent)
            {
                Console.WriteLine("\tItem ({0}: {1} (Id: {2})", item.Type, item.Name, item.Id);
            }


            // Initialize a new Client, this time by providing previously requested Access/Refresh tokens
            options.AccessToken = token.Access_Token;
            options.AccessToken = token.Refresh_Token;
            var client2 = new Client(options);

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
    }
}
