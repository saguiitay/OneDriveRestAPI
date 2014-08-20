namespace OneDriveRestAPI.Model
{
    public class Emails
    {
        /// <summary>
        /// The user's preferred email address, or null if one is not specified. 
        /// </summary>
        public string Preferred { get; set; }
        /// <summary>
        /// The email address that is associated with the account.
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// The user's personal email address, or null if one is not specified. 
        /// </summary>
        public string Personal { get; set; }
        /// <summary>
        /// The user's business email address, or null if one is not specified. 
        /// </summary>
        public string Business { get; set; }
        /// <summary>
        /// The user's "alternate" email address, or null if one is not specified.
        /// </summary>
        public string Other { get; set; }
    }
}