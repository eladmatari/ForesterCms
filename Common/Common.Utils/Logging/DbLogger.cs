using Common.Utils.Logging.Objects;
using Common.Utils.Standard;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Common.Utils.Logging
{
    public class DbLogger : BaseProcess
    {
        public readonly static DbLogger Instance = new DbLogger();
        private static Action<string, string, string> _sendSmsMethod;
        private const string REMOVED_FROM_LOG_MESSAGE = "unlogged";
        public static Func<string> TryGetPhoneMethod;
        public static readonly byte SourceId = (byte)RequestHelper.GetInt(Config.GetAppSettings("DbLogger.SourceId"));
        public static readonly string LogProcedure = Config.GetAppSettings("DbLogger.Procedure") ?? "srv_GeneralLogs_Insert";

        public static void Init(Action<string, string, string> sendSms)
        {
            _sendSmsMethod = sendSms;
        }

        private DbLogger()
        {
            Interval = 5000;
        }

        public static void AddLog(Exception ex, string service, string method, string step, object request, object response, bool isAlert = false, DateTime startDate = default(DateTime), DateTime endDate = default(DateTime))
        {
            AddLog(ex, service, method, step, request, response, null, isAlert, startDate, endDate);
        }

        public static void AddLog(Exception ex, string service, string method, string step, object request, object response, string phone, bool isAlert = false, DateTime startDate = default(DateTime), DateTime endDate = default(DateTime))
        {
            string server = HttpContextHelper.GetServerIp();
            string clientIp = null;
            string phoneNumber = null;
            string url = null;
            string traceIdentifier = null;

            if (HttpContextHelper.Current != null && HttpContextHelper.Current.Request != null)
            {
                clientIp = HttpContextHelper.Current.Connection?.RemoteIpAddress.ToString();
                url = $"{HttpContextHelper.Current.Request.Scheme}://{HttpContextHelper.Current.Request.Host}{HttpContextHelper.Current.Request.PathBase}{HttpContextHelper.Current.Request.Path}{HttpContextHelper.Current.Request.QueryString}";
                traceIdentifier = HttpContextHelper.Current.TraceIdentifier;

                if (request == null && response == null)
                {
                    string requestBody = null;

                    if (HttpContextHelper.Current.Request.Body.CanRead && HttpContextHelper.Current.Request.Body.CanSeek)
                    {
                        HttpContextHelper.Current.Request.Body.Position = 0;
                        using (var stream = new StreamReader(HttpContextHelper.Current.Request.Body, Encoding.UTF8, true, 1024, true))
                        {
                            var readTask = stream.ReadToEndAsync();
                            readTask.Wait(1000);
                            requestBody = readTask.Result;
                        }
                    }

                    if (IsAppLog)
                    {
                        request = requestBody;
                    }
                    else
                    {
                        var referer = HttpContextHelper.Current.Request.GetTypedHeaders().Referer;

                        request = new
                        {
                            url,
                            requestBody,
                            Host = HttpContextHelper.Current.Request.Host.Host,
                            Scheme = HttpContextHelper.Current.Request.Scheme,
                            UrlReferer = referer != null ? referer.AbsoluteUri : null,
                            UserAgent = HttpContextHelper.Current.Request.Headers["User-Agent"],
                            ClientIP = HttpContextHelper.Current.Connection.RemoteIpAddress.ToString()
                        };
                    }

                    if (HttpContextHelper.Current.Response.Body.CanRead && HttpContextHelper.Current.Response.Body.CanSeek)
                    {
                        HttpContextHelper.Current.Response.Body.Position = 0;
                        using (var stream = new StreamReader(HttpContextHelper.Current.Response.Body, Encoding.UTF8, true, 1024, true))
                        {
                            response = stream.ReadToEnd();
                        }
                    }
                }

                if (phoneNumber == null)
                {
                    if (TryGetPhoneMethod != null)
                    {
                        phoneNumber = TryGetPhoneMethod();
                    }
                    else if (HttpContextHelper.Current.User.Identity.IsAuthenticated)
                    {
                        phoneNumber = HttpContextHelper.Current.User.Claims.FirstOrDefault(i => i.Type == "Phone")?.Value;
                    }
                }
            }

            Instance.AddLog(new LogInfo()
            {
                StartDate = startDate == default(DateTime) ? DateTime.Now : startDate,
                EndDate = endDate == default(DateTime) ? DateTime.Now : endDate,
                Exception = ex != null ? ex.ToString() : null,
                Service = service,
                Method = method,
                Step = step,
                Url = url,
                Request = ObjectToString(request),
                Response = ObjectToString(response),
                PhoneNumber = phoneNumber,
                Server = server,
                ClientIp = clientIp,
                IsAlert = isAlert,
                MoreInfo = traceIdentifier
            });
        }

        private static string ObjectToString(object obj)
        {
            if (obj == null)
                return null;

            if (obj is string)
                return (string)obj;

            return JsonConvert.SerializeObject(obj);
        }

        private readonly object _logsLock = new object();
        protected List<LogInfo> Logs;

        protected void AddLog(LogInfo log)
        {
            lock (_logsLock)
            {
                if (Logs == null)
                    Logs = new List<LogInfo>();

                Logs.Add(log);
            }
        }

        protected override void Execute()
        {
            if (Logs == null || Logs.Count == 0)
                return;

            List<LogInfo> logsToInsert = null;

            lock (_logsLock)
            {
                logsToInsert = Logs;
                Logs = new List<LogInfo>();
            }

            PrepareLogs(logsToInsert);

            try
            {
                InsertGeneralLogs(logsToInsert);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            try
            {
                var logsAlerts = logsToInsert.Where(i => i.IsAlert).ToList();

                if (logsAlerts.Count > 0)
                {
                    var targets = CacheHelper.GetOrAdd("SystemAlertTargets", () =>
                    {
                        return GetSystemAlertTargets();
                    }, 300, false);

                    foreach (var logsAlert in logsAlerts)
                    {
                        foreach (var target in targets.Where(i => string.IsNullOrWhiteSpace(i.Type)))
                        {
                            AlertSms(logsAlert, target);
                            AlertEmail(logsAlert, target);
                        }

                        if (logsAlert.Request != null && logsAlert.Request.IndexOf("IT_IBM-WCE_EstablishIdentity", StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            foreach (var target in targets.Where(i => i.Type == "ibm"))
                            {
                                AlertSms(logsAlert, target);
                                AlertEmail(logsAlert, target);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            lock (_logsLock)
            {
                Logs.AddRange(logsToInsert.Where(i => !i.IsInserted));
            }
        }

        private void PrepareLogs(List<LogInfo> logsToInsert)
        {
            foreach (var log in logsToInsert)
            {
                if (string.IsNullOrWhiteSpace(log.PhoneNumber))
                {
                    if (log.Url?.EndsWith("/api/auth/login", StringComparison.OrdinalIgnoreCase) == true)
                        log.PhoneNumber = GetPhoneFromXml(log.Response);
                    else
                        log.PhoneNumber = GetPhoneFromXml(log.Request);
                }

                if (!log.IsAlert)
                    log.IsAlert = IsAlert(log.Response);

                if (string.IsNullOrWhiteSpace(log.Step))
                {
                    log.Step = GetCommand(log.Request);
                    if (log.Step == "IT_GetInvoicePDF")
                        log.Response = REMOVED_FROM_LOG_MESSAGE;
                }

                if (log.Response != null)
                {
                    if (log.Response.StartsWith("{\"fileContents\":"))
                        log.Response = REMOVED_FROM_LOG_MESSAGE;
                    else if (log.Response.Length >= 100000)
                        log.Response = REMOVED_FROM_LOG_MESSAGE;
                }

            }
        }

        private static void AlertEmail(LogInfo log, SystemAlertTarget target)
        {
            if (string.IsNullOrWhiteSpace(target.Email))
                return;

            try
            {
                string subject = string.Format("התקבלה שגיאה עבור {0} תאריך {1}", log.PhoneNumber, log.StartDate.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                var htmlBody = new StringBuilder();

                htmlBody.Append("<style>table, td { direction:ltr;text-align:left;vertical-align:top; }</style>");
                htmlBody.Append("<table>");
                htmlBody.AppendFormat("<tr><td>PhoneNumber:</td><td style=\"padding-right: 10px;\">{0}</td></tr>", log.PhoneNumber);
                htmlBody.AppendFormat("<tr><td>Exception:</td><td style=\"padding-right: 10px;\">{0}</td></tr>", log.Exception);
                htmlBody.AppendFormat("<tr><td>Request:</td><td style=\"padding-right: 10px;\">{0}</td></tr>", System.Web.HttpUtility.HtmlEncode(log.Request));
                htmlBody.AppendFormat("<tr><td>Response:</td><td style=\"padding-right: 10px;\">{0}</td></tr>", System.Web.HttpUtility.HtmlEncode(log.Response));
                htmlBody.AppendFormat("<tr><td>Server:</td><td style=\"padding-right: 10px;\">{0}</td></tr>", log.Server);
                htmlBody.AppendFormat("<tr><td>Method:</td><td style=\"padding-right: 10px;\">{0}</td></tr>", log.Method);
                htmlBody.AppendFormat("<tr><td>Service:</td><td style=\"padding-right: 10px;\">{0}</td></tr>", log.Service);
                htmlBody.AppendFormat("<tr><td>Step:</td><td style=\"padding-right: 10px;\">{0}</td></tr>", log.Step);
                htmlBody.Append("</table>");

                using (var smtp = new SmtpClient())
                {
                    smtp.Host = Config.MailServer;
                    using (var mailMessage = new MailMessage())
                    {
                        mailMessage.Subject = subject;
                        mailMessage.To.Add(target.Email);

                        mailMessage.From = new MailAddress("postmaster@pelephone.co.il");
                        mailMessage.IsBodyHtml = true;
                        mailMessage.Body = htmlBody.ToString();

                        smtp.Send(mailMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "error alerting email: " + target.Email);
            }
        }

        private static void AlertSms(LogInfo log, SystemAlertTarget target)
        {
            if (string.IsNullOrWhiteSpace(target.Phone) || log.Step == "Send_Sms")
                return;

            try
            {
                string message = string.Format("התקבלה שגיאה עבור {0}, תאריך {1}", log.Step, log.StartDate.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                if (!string.IsNullOrWhiteSpace(log.PhoneNumber))
                    message += string.Format(", מנוי {0}", log.PhoneNumber);

                _sendSmsMethod?.Invoke("PeleErrors", target.Phone, message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "error alerting sms: " + target.Phone);
            }
        }

        private static string GetPhoneFromXml(string request)
        {
            if (request != null)
            {
                var match = Regex.Match(request, "(^0[0-9]{9,9}$)|([^0-9]0[0-9]{9,9}[^0-9])", RegexOptions.Multiline);
                if (match.Success)
                    return Regex.Replace(match.Value, "[^0-9]", "");

                match = Regex.Match(request, "(^972[0-9]{9,9}$)|([^0-9]972[0-9]{9,9}[^0-9])", RegexOptions.Multiline);
                if (match.Success)
                    return "0" + Regex.Replace(match.Value, "[^0-9]", "").Substring(3);
            }

            return null;
        }

        private static string GetCommand(string request)
        {
            if (request != null)
            {
                var match = Regex.Match(request, "<command>[^<]+<\\/command>", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                if (match.Success)
                    return RequestHelper.StripHtml(match.Value);
            }

            return null;
        }

        private static bool IsAlert(string response)
        {
            if (response != null)
            {
                try
                {
                    var xd = new XmlDocument();
                    xd.LoadXml(response);

                    var statusCode = xd.SelectSingleNode("//STATUS_CODE");
                    if (statusCode != null)
                    {
                        if (statusCode.InnerText.Trim().Equals("SERVICE_UNAVAILABLE", StringComparison.OrdinalIgnoreCase))
                            return true;

                        if (statusCode.InnerText.Trim().Equals("UNAUTHORIZED_SERVICE", StringComparison.OrdinalIgnoreCase))
                            return true;
                    }
                }
                catch
                {
                    // not xml response
                }

                if (response.Contains("System.UnauthorizedAccessException"))
                    return true;
            }

            return false;
        }

        private static bool IsAppLog
        {
            get
            {
                return Config.GetAppSettings("Connection_DigitalLog") != null;
            }
        }

        private static void InsertGeneralLog(LogInfo log)
        {
            // app logs
            if (IsAppLog)
            {
                DBHelper.LogDatabase.ExecuteNonQuery(LogProcedure,
                    log.StartDate,
                    log.EndDate,
                    RequestHelper.SecureSqlParameter(log.Exception) ?? "",
                    RequestHelper.SecureSqlParameter(log.Service) ?? "",
                    RequestHelper.SecureSqlParameter(log.Method) ?? "",
                    RequestHelper.SecureSqlParameter(log.Step) ?? "",
                    RequestHelper.SecureSqlParameter(log.Url) ?? "",
                    RequestHelper.SecureSqlParameter(log.Request) ?? "",
                    RequestHelper.SecureSqlParameter(log.Response) ?? "",
                    RequestHelper.SecureSqlParameter(log.PhoneNumber) ?? "",
                    RequestHelper.SecureSqlParameter(log.Server) ?? "",
                    RequestHelper.SecureSqlParameter(log.ClientIp) ?? "",
                    RequestHelper.SecureSqlParameter(log.MoreInfo) ?? ""
                );
            }
            else // site logs
            {
                DBHelper.Database.ExecuteNonQuery("site_GeneralLogs_Insert2",
                    log.StartDate,
                    RequestHelper.SecureSqlParameter(log.Exception) ?? "",
                    RequestHelper.SecureSqlParameter(log.Service) ?? "",
                    RequestHelper.SecureSqlParameter(log.Method) ?? "",
                    RequestHelper.SecureSqlParameter(log.Step) ?? "",
                    RequestHelper.SecureSqlParameter(log.Request) ?? "",
                    RequestHelper.SecureSqlParameter(log.Response) ?? "",
                    RequestHelper.SecureSqlParameter(log.PhoneNumber) ?? "",
                    RequestHelper.SecureSqlParameter(log.Server) ?? "",
                    SourceId
                );
            }

            log.IsInserted = true;
        }

        private static void InsertGeneralLogs(List<LogInfo> logs)
        {
            foreach (var log in logs)
            {
                try
                {
                    InsertGeneralLog(log);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
        }

        private static List<SystemAlertTarget> GetSystemAlertTargets()
        {
            var ds = DBHelper.Database.ExecuteDataset("site_SystemAlertTargets_Get");

            var items = new List<SystemAlertTarget>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                var resultItem = new SystemAlertTarget();

                resultItem.Id = row.Field<int>("Id");
                resultItem.Name = row.Field<string>("Name");
                resultItem.Email = row.Field<string>("Email");
                resultItem.Phone = row.Field<string>("Phone");
                resultItem.IsActive = row.Field<bool>("IsActive");
                resultItem.Type = row.Field<string>("Type");

                items.Add(resultItem);
            }

            return items;
        }

        public override void Start()
        {
            Logger.Debug("DbLogger start");
            base.Start();
        }

        public override void Stop()
        {
            Logger.Debug("DbLogger stop");
            base.Stop();
            Execute();
        }
    }
}
