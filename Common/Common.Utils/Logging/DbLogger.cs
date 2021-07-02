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
        public static Action<LogInfo, string> SendSmsMethod;
        public static Action<LogInfo, string> SendEmailMethod;
        public static Action<LogInfo> InsertLogMethod;
        public static Func<List<SystemAlertTarget>> GetSystemAlertTargets;
        private const string REMOVED_FROM_LOG_MESSAGE = "unlogged";
        public static Func<string> TryGetPhoneMethod;
        public static readonly byte SourceId = (byte)RequestHelper.GetInt(Config.GetAppSettings("DbLogger.SourceId"));

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
                        return GetSystemAlertTargets?.Invoke().Where(i => i.IsActive).ToList();
                    }, 300, false);

                    if (targets != null)
                    {
                        foreach (var logsAlert in logsAlerts)
                        {
                            foreach (var target in targets.Where(i => string.IsNullOrWhiteSpace(i.Type)))
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
                        log.PhoneNumber = GetPhoneFromData(log.Response);
                    else
                        log.PhoneNumber = GetPhoneFromData(log.Request);
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
                SendEmailMethod?.Invoke(log, target.Email);
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
                SendSmsMethod?.Invoke(log, target.Phone);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "error alerting sms: " + target.Phone);
            }
        }

        private static string GetPhoneFromData(string request)
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

        private static void InsertGeneralLog(LogInfo log)
        {
            InsertLogMethod.Invoke(log);

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
