namespace OneDriveRestAPI.Model
{
    public class Address
    {
        /// <summary>
        /// The street address, or null if one is not specified. 
        /// </summary>
        public string Street { get; set; }
        /// <summary>
        /// The second line of the street address, or null if one is not specified.
        /// </summary>
        public string Street_2 { get; set; }
        /// <summary>
        /// The city of the address, or null if one is not specified. 
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// The state of the address, or null if one is not specified. 
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// The postal code of the address, or null if one is not specified. 
        /// </summary>
        public string Postal_Code { get; set; }
        /// <summary>
        /// The region of the address, or null if one is not specified. 
        /// </summary>
        public string Region { get; set; }

    }
}