using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utils.Logging.Objects
{
    public class WebApiLogInfo
    {
        public Exception Exception { get; set; }
        public NameValueCollection Headers { get; set; }
        public string Method { get; set; }
        public TimeSpan Time { get; set; }
        public object Request { get; set; }
        public string Url { get; set; }
        public CookieCollection RequestCookies { get; set; }
        public CookieCollection ResponseCookies { get; set; }
        public string ResultJson { get; set; }
        public string Phone { get; set; }
        public object Result { get; set; }
    }
}
