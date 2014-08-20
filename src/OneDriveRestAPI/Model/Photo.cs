using System.Collections.Generic;

namespace OneDriveRestAPI.Model
{

    public class Photo : File
    {
        public const string PhotoType = "photo";

        /// <summary>
        /// The height, in pixels, of the photo.
        /// </summary>
        public long Height { get; set; }

        /// <summary>
        /// The width, in pixels, of the photo.
        /// </summary>
        public long Width { get; set; }

        /// <summary>
        ///   Info about various sizes of the photo.
        /// </summary>
        public List<Image> Images { get; set; }

        /// <summary>
        /// The date, in ISO 8601 format, on which the photo was taken, or null if no date is specified.
        /// </summary>
        public string When_Taken { get; set; }

        /// <summary>
        /// The location where the photo was taken. The location object is not available for shared photos.
        /// </summary>
        public Location Location { get; set; }
    }
}