namespace OneDriveRestAPI.Model
{
    public class Addresses
    {
        /// <summary>
        /// The user's personal postal address
        /// </summary>
        public Address Personal { get; set; }
        /// <summary>
        /// The user's business postal address. 
        /// </summary>
        public Address Business { get; set; }
    }
}