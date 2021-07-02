using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Common.Utils.Standard
{
    public static class Extensions
    {
        public static string GetValue(this XNode xd, string xpath)
        {
            var element = xd.XPathSelectElement(xpath);
            if (element != null)
                return element.Value;

            return null;
        }

        public static DateTime? GetValue(this XNode xd, string xpath, string dateTimeFormat)
        {
            string val = xd.GetValue(xpath);

            if (!string.IsNullOrWhiteSpace(val))
            {
                DateTime dateVal;
                if (DateTime.TryParseExact(val, dateTimeFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out dateVal))
                    return dateVal;
            }

            return null;
        }

        public static T GetValue<T>(this XNode xd, string xpath)
        {
            string val = xd.GetValue(xpath);
            if (!string.IsNullOrWhiteSpace(val))
            {
                try
                {
                    var type = typeof(T);

                    switch (type.Name)
                    {
                        case "String":
                            return (T)(object)val;
                        default:
                            return (T)Convert.ChangeType(val, type);
                    }

                }
                catch (Exception ex)
                {
                    Config.Logger?.Error(ex);
                }
            }

            return default(T);
        }

        public static string GetValue(this XmlNode xd, string xpath)
        {
            var element = xd.SelectSingleNode(xpath);
            if (element != null)
                return element.Value;

            return null;
        }

        public static DateTime? GetValue(this XmlNode xd, string xpath, string dateTimeFormat)
        {
            string val = xd.GetValue(xpath);

            if (!string.IsNullOrWhiteSpace(val))
            {
                DateTime dateVal;
                if (DateTime.TryParseExact(val, dateTimeFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out dateVal))
                    return dateVal;
            }

            return null;
        }

        public static T GetValue<T>(this XmlNode xd, string xpath)
        {
            string val = xd.GetValue(xpath);
            if (!string.IsNullOrWhiteSpace(val))
            {
                try
                {
                    var type = typeof(T);

                    switch (type.Name)
                    {
                        case "String":
                            return (T)(object)val;
                        default:
                            return (T)Convert.ChangeType(val, type);
                    }

                }
                catch (Exception ex)
                {
                    Config.Logger?.Error(ex);
                }
            }

            return default(T);
        }

        public static string GetInnerText(this XmlNode xd, string xpath)
        {
            var element = xd.SelectSingleNode(xpath);
            if (element != null)
                return element.InnerText;

            return null;
        }

        public static DateTime? GetInnerText(this XmlNode xd, string xpath, string dateTimeFormat)
        {
            string val = xd.GetInnerText(xpath);

            if (!string.IsNullOrWhiteSpace(val))
            {
                DateTime dateVal;
                if (DateTime.TryParseExact(val, dateTimeFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out dateVal))
                    return dateVal;
            }

            return null;
        }

        public static T GetInnerText<T>(this XmlNode xd, string xpath)
        {
            string val = xd.GetInnerText(xpath);
            if (!string.IsNullOrWhiteSpace(val))
            {
                try
                {
                    var type = typeof(T);

                    switch (type.Name)
                    {
                        case "String":
                            return (T)(object)val;
                        default:
                            return (T)Convert.ChangeType(val, type);
                    }

                }
                catch (Exception ex)
                {
                    Config.Logger?.Error(ex);
                }
            }

            return default(T);
        }

        public static string GetHebMonth(this DateTime date)
        {
            switch (date.Month)
            {
                case 1:
                    return "ינואר";
                case 2:
                    return "פברואר";
                case 3:
                    return "מרס";
                case 4:
                    return "אפריל";
                case 5:
                    return "מאי";
                case 6:
                    return "יוני";
                case 7:
                    return "יולי";
                case 8:
                    return "אוגוסט";
                case 9:
                    return "ספטמבר";
                case 10:
                    return "אוקטובר";
                case 11:
                    return "נובמבר";
                case 12:
                    return "דצמבר";
            }

            return null;
        }

        public static string GetHebDay(this DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return "ראשון";
                case DayOfWeek.Monday:
                    return "שני";
                case DayOfWeek.Tuesday:
                    return "שלישי";
                case DayOfWeek.Wednesday:
                    return "רביעי";
                case DayOfWeek.Thursday:
                    return "חמישי";
                case DayOfWeek.Friday:
                    return "שישי";
                case DayOfWeek.Saturday:
                    return "שבת";
            }

            return null;
        }

        public static string FormatHebDate(this DateTime date)
        {
            return $"יום {date.GetHebDay()} {date.ToString("dd/MM/yyyy")}";
        }

        public static string ToMoney(this decimal d)
        {
            return d.ToString("₪ #,##0.##");
        }

        public static string ToMoney(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return s;

            return "₪ " + s;
        }

        public static string ToNegativeNumber(this string s)
        {
            if (string.IsNullOrWhiteSpace(s) || s == "0")
                return s;

            return "-" + s;
        }

        public static DataSet ExecuteDataSetText(this SqlServerHelper db, string text)
        {
            return db.ExecuteDataset(CommandType.Text, text);
        }

        public static void ExecuteNonQueryText(this SqlServerHelper db, string text)
        {
            db.ExecuteNonQuery(CommandType.Text, text);
        }

        public static object ExecuteScalarText(this SqlServerHelper db, string text)
        {
            return db.ExecuteScalar(CommandType.Text, text);
        }

        public static void AddParameter(this DbCommand cmd, string name, object value)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = "@" + name;
            parameter.Value = value;
            cmd.Parameters.Add(parameter);
        }

        public static XElement ToXElement(this XmlElement element)
        {
            return XElement.Parse(element.OuterXml);
        }

        public static XDocument ToXDocument(this XmlElement element)
        {
            return XDocument.Parse(element.OuterXml);
        }

        public static XmlElement ToXmlElement(this XElement element)
        {
            return ToXmlElement(element, new XmlDocument());
        }

        public static XmlElement ToXmlElement(this XElement element, XmlDocument doc)
        {
            doc.LoadXml(element.ToString());
            return doc.DocumentElement;
        }

        public static XmlDocument ToXmlDocument(this XDocument element)
        {
            var doc = new XmlDocument();
            doc.LoadXml(element.ToString());
            return doc;
        }

        public static string GetDescendantValue(this XDocument doc, string descendantName)
        {
            var descendant = doc.Descendants(descendantName).FirstOrDefault();
            if (descendant != null)
                return descendant.Value;

            return null;
        }

        public static Dictionary<string, string> ToDictionary(this NameValueCollection nameValueCollection)
        {
            var dict = new Dictionary<string, string>();

            foreach (var key in nameValueCollection.AllKeys)
            {
                dict[key] = nameValueCollection[key];
            }

            return dict;
        }

        public static string ToQueryString(this NameValueCollection nvc)
        {
            var sb = new StringBuilder();

            foreach (string key in nvc.Keys)
            {
                if (string.IsNullOrEmpty(key))
                    continue;

                string[] values = nvc.GetValues(key);
                if (values == null) continue;

                foreach (string value in values)
                {
                    if (sb.Length > 0)
                        sb.Append("&");

                    sb.AppendFormat("{0}={1}", Uri.EscapeDataString(key), Uri.EscapeDataString(value ?? ""));
                }
            }

            return sb.ToString();
        }

        public static string ToQueryStringRaw(this NameValueCollection nvc)
        {
            var sb = new StringBuilder();

            foreach (string key in nvc.Keys)
            {
                foreach (string value in nvc.GetValues(key))
                {
                    if (sb.Length > 0)
                        sb.Append("&");

                    sb.AppendFormat("{0}={1}", key, value);
                }
            }

            return sb.ToString();
        }

        public static IEnumerable<T> Reversed<T>(this IEnumerable<T> list)
        {
            var newList = list.ToList();
            newList.Reverse();
            return newList;
        }
    }
}
