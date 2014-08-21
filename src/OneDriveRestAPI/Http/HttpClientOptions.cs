using System;

namespace OneDriveRestAPI.Http
{
    public class HttpClientOptions
    {
        public bool AllowAutoRedirect { get; set; }

        public bool AddTokenToRequests { get; set; }
        public Func<string> TokenRetriever { get; set; }


        public double? ReadRequestsPerSecond { get; set; }
        public double? WriteRequestsPerSecond { get; set; }

    }
}