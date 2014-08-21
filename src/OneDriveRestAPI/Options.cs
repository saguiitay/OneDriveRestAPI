namespace OneDriveRestAPI
{
    public class Options
    {
        public bool PrettyJson { get; set; }
        public bool AutoRefreshTokens { get; set; }

        public float? ReadRequestsPerSecond { get; set; }
        public float? WriteRequestsPerSecond { get; set; }
        

        public string ClientId { get; set; }
        public string ClientSecret{ get; set; }
        public string CallbackUrl{ get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}