namespace OneDriveRestAPI.Model
{
    public class Work
    {
        /// <summary>
        /// Info about the user's employer.
        /// </summary>
        public Employer Employer { get; set; }
        /// <summary>
        /// Info about the user's work position.
        /// </summary>
        public Position Position { get; set; }
    }
}