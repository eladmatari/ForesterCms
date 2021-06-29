using Ganss.XSS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Common.Utils.Standard
{
    public static class RequestHelper
    {
        public const string REGEX_PHONE = "^\\+?\\d{2}[- ]?\\d{3}[- ]?\\d{5}$";
        public const string REGEX_PHONE_CELLPHONE = "^0[1-9][0-9]{7,8}$";
        public const string REGEX_CELLPHONE = "^0[1-9][0-9]{8}$";
        public const string REGEX_EMAIL = "^[a-zA-Z0-9.!#$%&’*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\\.[a-zA-Z0-9-]+)*$";
        public const string REGEX_SQL_DATE_FORMAT = "(^[0-9]{4}-[0-9]{2}-[0-9]{2}$)|(^[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}$)|(^[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}.[0-9]{1,3}$)";

        public static int GetInt(string s, int? defaultValue = null)
        {
            int i;
            if (int.TryParse(s, out i))
                return i;

            if (defaultValue != null)
                return (int)defaultValue;

            return default(int);
        }

        public static decimal GetDecimal(string s, decimal? defaultValue = null)
        {
            decimal i;
            if (decimal.TryParse(s, out i))
                return i;

            if (defaultValue != null)
                return (decimal)defaultValue;

            return default(decimal);
        }

        public static string SecureFreeTextQueryParameter(object i)
        {
            if (i == null)
                return "null";

            return i.ToString();
        }

        public static string SecureFreeTextQueryParameter(string s)
        {
            if (s == null)
                return "null";

            return "'" + SecureSqlParameter(s) + "'";
        }

        public static string SecureSqlParameter(string s)
        {
            if (s == null)
                return null;

            return s.Replace("'", "''");
        }

        public static string ValidateSqlName(string s)
        {
            if (!string.IsNullOrWhiteSpace(s) && !Regex.IsMatch(s, "^[a-zA-Z0-9_]+$"))
                throw new Exception("invalid sql name: " + HttpUtility.HtmlEncode(s));

            return s;
        }

        public static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email ?? "", REGEX_EMAIL);
        }

        public static bool IsValidPhone(string phone)
        {
            return Regex.IsMatch(phone ?? "", REGEX_PHONE);
        }

        public static bool IsValidCellphone(string phone)
        {
            return Regex.IsMatch(phone ?? "", REGEX_PHONE_CELLPHONE);
        }

        public static bool IsValidNumber(string s)
        {
            return Regex.IsMatch(s ?? "", "^[0-9]+$");
        }

        public static string SanitizeHtml(string s)
        {
            var sanitizer = new HtmlSanitizer();

            return sanitizer.Sanitize(s);
        }

        public static string StripHtml(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return "";
            }

            s = Regex.Replace(s, @"<script[^<\/]+<\/script>", string.Empty);
            s = Regex.Replace(s, @"<style[^<\/]+<\/style>", string.Empty);
            s = Regex.Replace(s, @"<(.|\n)*?>", string.Empty);
            s = s.Replace("&nbsp;", " ");

            return SanitizeHtml(s);
        }

        public static string SecureRequestParam(string param, int maxLength = 4000)
        {
            if (!string.IsNullOrWhiteSpace(param))
            {
                if (param.Length > maxLength)
                    return null;

                return SecureSqlParameter(StripHtml(param));
            }

            return param;
        }

        public static string SecureQueryParam(string param, int maxLength = 200)
        {
            param = SecureRequestParam(param, maxLength);
            return HttpUtility.UrlEncode(param);
        }
    }
}
