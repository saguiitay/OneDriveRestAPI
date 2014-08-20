namespace OneDriveRestAPI.Model
{
    public class User
    {
        /// <summary>
        /// The user's ID.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The user's full name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The user's first name.
        /// </summary>
        public string First_Name { get; set; }
        /// <summary>
        /// The user's last name.
        /// </summary>
        public string Last_Name{ get; set; }
        /// <summary>
        /// The URL of the user's profile page.
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// The day of the user's birth date, or null if no birth date is specified.
        /// </summary>
        public int? Birth_Day { get; set; }
        /// <summary>
        /// The month of the user's birth date, or null if no birth date is specified.
        /// </summary>
        public int? Birth_Month { get; set; }
        /// <summary>
        /// The year of the user's birth date, or null if no birth date is specified.
        /// </summary>
        public int? Birth_Year { get; set; }
        /// <summary>
        /// An array that contains the user's work info.
        /// </summary>
        public Work[] Work { get; set; }
        /// <summary>
        /// The user's email addresses.
        /// </summary>
        public Emails Emails { get; set; }
        /// <summary>
        /// The user's postal addresses.
        /// </summary>
        public Addresses Addresses { get; set; }
        /// <summary>
        /// The user's phone numbers.
        /// </summary>
        public Phones Phones { get; set; }
        /// <summary>
        /// The user's locale code.
        /// </summary>
        public string Locale { get; set; }
        /// <summary>
        /// The time, in ISO 8601 format, at which the user last updated the object.
        /// </summary>
        public string Updated_Time { get; set; }
    }
}