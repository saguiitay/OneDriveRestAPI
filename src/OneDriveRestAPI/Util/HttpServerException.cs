using System;
using System.Runtime.Serialization;

namespace OneDriveRestAPI.Util
{
    public class HttpServerException : System.Web.HttpException
    {
        public HttpServerException(string message) : base(message)
        {
        }

        public HttpServerException()
        {
        }

        public HttpServerException(string message, int hr) : base(message, hr)
        {
        }

        public HttpServerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public HttpServerException(int httpCode, string message, Exception innerException) : base(httpCode, message, innerException)
        {
        }

        public HttpServerException(int httpCode, string message, int hr) : base(httpCode, message, hr)
        {
        }

        public HttpServerException(int httpCode, string message) : base(httpCode, message)
        {
        }

        protected HttpServerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public int? Attempts { get; set; }
    }
}