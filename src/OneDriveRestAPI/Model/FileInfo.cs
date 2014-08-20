namespace OneDriveRestAPI.Model
{
    public class FileInfo
    {
        /// <summary>
        ///   The URL to use to download the file from SkyDrive. This value is not persistent. Use it immediately after making the request, and avoid caching.  This structure is not available if the file is a Office OneNote notebook.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        ///   The File object's ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///   The name of the file.
        /// </summary>
        public string Name { get; set; }
    }
}