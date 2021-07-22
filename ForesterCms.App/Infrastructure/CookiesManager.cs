using Common.Utils;
using Common.Utils.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForesterCms.App.Infrastructure
{
    public class CookiesManager
    {
        private static void SetCookie(string name, string val, int days, bool httpOnly = false, string domain = null)
        {
            // set cookie on response
            HttpContextHelper.Current.Response.Cookies.Delete(name);

            var cookie = new Microsoft.AspNetCore.Http.CookieOptions()
            {
                Secure = HttpContextHelper.Current.Request.IsHttps,
                SameSite = HttpContextHelper.Current.Request.IsHttps ? Microsoft.AspNetCore.Http.SameSiteMode.None : Microsoft.AspNetCore.Http.SameSiteMode.Lax,
                Expires = DateTime.Now.AddDays(days),
                HttpOnly = httpOnly
            };

            if (domain != null)
                cookie.Domain = domain;

            HttpContextHelper.Current.Response.Cookies.Append(name, val, cookie);
        }

        private static void RemoveCookie(string name)
        {
            HttpContextHelper.Current.Response.Cookies.Delete(name);
        }

        private static string GetCookie(string name)
        {
            if (HttpContextHelper.Current.Request.Cookies[name] != null)
                return HttpContextHelper.Current.Request.Cookies[name];

            return null;
        }

        public static bool IsPreview
        {
            get
            {
                return GetCookie("preview") == "1" || HttpContextHelper.Current.Request.Query["preview"] == "1";
            }
            set
            {
                if (Config.Environment != EnvironmentType.Prod)
                {
                    if (value)
                        SetCookie("preview", "1", 365);
                    else
                        RemoveCookie("preview");
                }
            }
        }




        public static bool? IsMobile
        {
            get
            {
                string mobile = GetCookie("mobile");

                if (mobile == "1")
                    return true;

                if (mobile == "0")
                    return false;

                return null;
            }
            set
            {
                if (value == true)
                    SetCookie("mobile", "1", 365);
                else if (value == false)
                    SetCookie("mobile", "0", 365);
                else
                    RemoveCookie("mobile");
            }
        }
    }
}
