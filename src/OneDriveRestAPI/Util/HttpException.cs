using System;
using System.Runtime.Serialization;

namespace OneDriveRestAPI.Util
{
    public class HttpException : System.Web.HttpException
    {
        public HttpException(string message) : base(message)
        {
        }

        public HttpException()
        {
        }

        public HttpException(string message, int hr) : base(message, hr)
        {
        }

        public HttpException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public HttpException(int httpCode, string message, Exception innerException) : base(httpCode, message, innerException)
        {
        }

        public HttpException(int httpCode, string message, int hr) : base(httpCode, message, hr)
        {
        }

        public HttpException(int httpCode, string message) : base(httpCode, message)
        {
        }

        protected HttpException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public int? Attempts { get; set; }
    }
}