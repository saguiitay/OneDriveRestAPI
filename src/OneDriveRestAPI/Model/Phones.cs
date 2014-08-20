namespace OneDriveRestAPI.Model
{
    public class Phones
    {
        /// <summary>
        /// The user's personal phone number, or null if one is not specified. 
        /// </summary>
        public string Personal { get; set; }
        /// <summary>
        /// The user's business phone number, or null if one is not specified.
        /// </summary>
        public string Business { get; set; }
        /// <summary>
        /// The user's mobile phone number, or null if one is not specified. 
        /// </summary>
        public string Mobile { get; set; }
    }
}