using System;
using System.Runtime.Serialization;

namespace OneDriveRestAPI.Model.Exceptions
{
    public class NotEnoughQuotaException : Exception
    {
        public NotEnoughQuotaException()
        {
        }

        public NotEnoughQuotaException(string message)
            : base(message)
        {
        }

        public NotEnoughQuotaException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected NotEnoughQuotaException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}