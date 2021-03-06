using System.Collections.Specialized;

namespace HotCoreUtils.Http
{
    public class HttpFormResponse
    {
        public HttpFormResponse()
        {
            Headers = new NameValueCollection();
            Cookies = new NameValueCollection();
        }

        public string Response
        {
            get;
            set;
        }

        public NameValueCollection Headers
        {
            get;
            set;
        }

        public NameValueCollection Cookies
        {
            get;
            set;
        }
    }
}