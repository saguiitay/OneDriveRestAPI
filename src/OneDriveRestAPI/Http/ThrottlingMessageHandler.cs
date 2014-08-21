using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using OneDriveRestAPI.Util;

namespace OneDriveRestAPI.Http
{
    public class ThrottlingMessageHandler : DelegatingHandler
    {
        private readonly TimeSpanSemaphore _readTimeSpanSemaphore;
        private readonly TimeSpanSemaphore _writeTimeSpanSemaphore;

        public ThrottlingMessageHandler(TimeSpanSemaphore readTimeSpanSemaphore, TimeSpanSemaphore writeTimeSpanSemaphore)
            : this(readTimeSpanSemaphore, writeTimeSpanSemaphore, null)
        { }

        public ThrottlingMessageHandler(TimeSpanSemaphore readTimeSpanSemaphore, TimeSpanSemaphore writeTimeSpanSemaphore, HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            _readTimeSpanSemaphore = readTimeSpanSemaphore;
            _writeTimeSpanSemaphore = writeTimeSpanSemaphore;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Get ||
                request.Method == HttpMethod.Head ||
                request.Method == HttpMethod.Options)
            {
                if (_readTimeSpanSemaphore != null)
                    return _readTimeSpanSemaphore.RunAsync(base.SendAsync, request, cancellationToken);

                return base.SendAsync(request, cancellationToken);
            }

            if (_writeTimeSpanSemaphore != null)
                return _writeTimeSpanSemaphore.RunAsync(base.SendAsync, request, cancellationToken);
            return base.SendAsync(request, cancellationToken);
        }
    }
}