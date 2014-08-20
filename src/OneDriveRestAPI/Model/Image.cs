namespace OneDriveRestAPI.Model
{
    public class Image
    {
        /// <summary>
        ///   The height, in pixels, of this image of this particular size.
        /// </summary>
        public long Height { get; set; }

        /// <summary>
        ///   The width, in pixels, of this image of this particular size.
        /// </summary>
        public long Width { get; set; }

        /// <summary>
        ///   A URL of the source file of this image of this particular size.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        ///   The type of this image of this particular size. Valid values are:
        ///   full (max size 2048 x 2048 pixels)
        ///   normal (max size 800 x 800 pixels)
        ///   album (max size 200 x 200 pixels)
        ///   small (max size 100 x 100 pixels)
        /// </summary>
        public string Type { get; set; }
    }
}