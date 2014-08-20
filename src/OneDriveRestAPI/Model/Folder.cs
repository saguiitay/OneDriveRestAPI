namespace OneDriveRestAPI.Model
{
    public class Folder : File
    {
        public const string Root = "/me/skydrive";
        public const string FolderType = "folder";

        /// <summary>
        /// The total number of items in the folder.
        /// </summary>
        public long Count { get; set; }
    }
}