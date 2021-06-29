using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utils
{
    public static class HttpContextHelper
    {
        private static IHttpContextAccessor m_httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            m_httpContextAccessor = httpContextAccessor;
        }

        public static HttpContext Current
        {
            get
            {
                if (m_httpContextAccessor != null)
                    return m_httpContextAccessor.HttpContext;

                return null;
            }
        }

        private static T GetOrAddItem<T>(this HttpContext context, string baseKey, Func<T> method, params object[] keys)
        {
            if (context == null)
                return method();

            string key = baseKey + "$" + string.Join("|", keys);

            object val = context.Items[key];
            if (val == null)
                val = context.Items[key] = method();

            return (T)val;
        }

        public static T GetOrAddItem<T>(string baseKey, Func<T> method, params object[] keys)
        {
            return Current.GetOrAddItem<T>("ItemsHelper.Outer." + baseKey, method, keys);
        }

        private static string _serverIp;
        private static readonly object _serverIpLockObj = new object();

        public static string GetServerIp()
        {
            if (_serverIp == null)
            {
                lock (_serverIpLockObj)
                {
                    if (_serverIp == null)
                    {
                        var ipsTask = Dns.GetHostAddressesAsync(Dns.GetHostName());
                        ipsTask.Wait();
                        var ips = ipsTask.Result;

                        _serverIp = ips.LastOrDefault()?.ToString();
                    }
                }
            }

            return _serverIp;
        }
    }
}
