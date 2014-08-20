namespace OneDriveRestAPI.Model
{
    public class Audio : File
    {
        public const string AudioType = "audio";

        /// <summary>
        /// A URL of a picture that represents the video.
        /// </summary>
        public string Picture { get; set; }

        /// <summary>
        /// The duration, in milliseconds, of the video run time.
        /// </summary>
        public long Duration { get; set; }

        /// <summary>
        /// The audio's title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The audio's artist name.
        /// </summary>
        public string Artist { get; set; }

        /// <summary>
        /// The audio's album name.
        /// </summary>
        public string Album { get; set; }

        /// <summary>
        /// The artist name of the audio's album.
        /// </summary>
        public string Album_Artist { get; set; }

        /// <summary>
        /// The audio's genre.
        /// </summary>
        public string Genre { get; set; }
    }
}