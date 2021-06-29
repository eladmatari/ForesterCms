using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Common.Utils
{
    public static class UrlHelper
    {
        public static string CreateAlias(string name)
        {
            if (name == null)
                return null;

            string alias = name.Trim().ToLower();

            alias = Regex.Replace(alias, "[^A-Za-z0-9א-ת]", " ");

            while (alias.Contains("  "))
                alias = alias.Replace("  ", " ");

            alias = alias.Replace(" ", "-");

            return alias;
        }

        public static string ToAbsolute(string s)
        {
            if (s.StartsWith("~"))
                return HttpContextHelper.Current.Request.PathBase + s.Substring(1);

            return s;
        }
    }
}
