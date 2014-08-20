using System;
using System.Runtime.Serialization;

namespace OneDriveRestAPI.Model.Exceptions
{
    public class RetryLaterException : Exception
    {
        public RetryLaterException()
        {
        }

        public RetryLaterException(string message)
            : base(message)
        {
        }

        public RetryLaterException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected RetryLaterException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public int? RetryAfter { get; set; }
    }
}