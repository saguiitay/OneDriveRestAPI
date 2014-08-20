using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;

namespace OneDriveRestAPI.Util
{
    public interface IRequest
    {
        string BaseAddress { get; }
        string Resource { get;  }
        
        HttpMethod Method { get; }
        HttpContent Content { get; }
        HttpRequestHeaders Headers { get; }
        
        Uri BuildUri();
    }

    public class Request : IRequest
    {
        private readonly NameValueCollection _query = new NameValueCollection();

        public HttpMethod Method { get; set; }
        public string BaseAddress { get; set; }
        public string Resource { get; set; }

        public HttpContent Content { get; set; }
        public HttpRequestHeaders Headers { get; set; }

        public Request()
        {
            _query["pretty"] = "false";
        }

        public Uri BuildUri()
        {
            var uriBuilder = new UriBuilder(BaseAddress);
            if (string.IsNullOrEmpty(uriBuilder.Path))
                uriBuilder.Path = Resource;
            else
            {
                if (uriBuilder.Path.EndsWith("/"))
                    uriBuilder.Path += Resource.TrimStart('/');
                else
                    uriBuilder.Path += Resource;
            }
            uriBuilder.Query = _query.ToQueryString(false);

            return uriBuilder.Uri;
        }


        public void AddParameter(string key, string value)
        {
            _query[key] = value;
        }

        public void AddHeader(string name, string value)
        {
            Headers.Add(name, value);
        }
    }
}