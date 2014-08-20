# OneDriveRestAPI

## About

OneDriveRestAPI is a C# client for the Microsoft OneDrive service through its RESTful API.

Microsoft has released the [Live SDK](http://msdn.microsoft.com/en-US/live/ff621310 "Microsoft Live SDK"), a set of C# libraries for interacting with OneDrive, which is hard to extend and debug, as it relies on async HttpWebRequest/HttpWebResponse.

OneDriveRestAPI is based on [John Hoerr](https://twitter.com/johnhoerr)'s [SkyNet](https://github.com/jhoerr/SkyNet) project.

## Contact

You can contact me on twitter [@saguiitay](https://twitter.com/saguiitay).

## NuGet

OneDriveRestAPI is available as a [NuGet package](https://www.nuget.org/packages/OneDriveRestAPI)

## Release Notes

+ **1.0.1**   Added [NuGet Package](https://www.nuget.org/packages/OneDriveRestAPI).
+ **1.0.0**   Initial release.

## Usage

```csharp
const string clientId = "";
const string clientSecret = "";
const string callbackUrl = "";
            
// Initialize a new Client (without an Access/Refresh tokens
var client = new Client(clientId, clientSecret, callbackUrl);

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
var client2 = new Client(clientId, clientSecret, callbackUrl, token.Access_Token, token.Refresh_Token);

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

```


