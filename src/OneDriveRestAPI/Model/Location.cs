namespace OneDriveRestAPI.Model
{
    public class Location
    {
        /// <summary>
        /// The latitude portion of the location where the photo was taken, expressed as positive (north) or negative (south) degrees relative to the equator.
        /// </summary>
        public double Lattitude { get; set; }

        /// <summary>
        /// The longitude portion of the location where the photo was taken, expressed as positive (east) or negative (west) degrees relative to the Prime Meridian.
        /// </summary>
        public double Longitude { get; set; }
    }
}