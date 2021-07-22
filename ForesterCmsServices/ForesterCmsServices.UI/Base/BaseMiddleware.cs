using Common.Utils.Logging;
using Common.Utils.Standard;
using ForesterCmsServices.UI.Resources;
using ForesterCmsServices.UI.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.Base
{
    public class BaseMiddleware
    {
        private static Action<Exception> _onError;
        public static Action<Exception> OnError
        {
            get
            {
                return _onError;
            }
            set
            {
                if (_onError != null)
                    throw new InvalidOperationException("OnError is already set");

                _onError = value;
            }
        }

        private readonly RequestDelegate _next;

        public BaseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(Microsoft.AspNetCore.Http.HttpContext context)
        {
            if (TryRedirect(context))
                return;

            // is file
            if (Regex.IsMatch(context.Request.Path, "/.+\\..+$"))
            {
                await _next(context);
                return;
            }

            if (!IsTrailingSlash(context))
                return;

            StringValues debugValues;
            if (context.Request.Query.TryGetValue("debug", out debugValues))
            {
                bool isDebug = debugValues[0] == "1";
                ResourceGroupHelper.SetDebugMode(isDebug);
            }
            else if (Config.Environment == EnvironmentType.Local && ResourceGroupHelper.IsDebugModeEnabled == null)
            {
                ResourceGroupHelper.SetDebugMode(true);
            }

            var startDate = DateTime.Now;
            var sw = Stopwatch.StartNew();
            Exception exception = null;
            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                try
                {
                    context.Response.Body = responseBody;
                    await _next(context);
                    SessionHelper.SaveSessionItems();
                }
                catch (Exception ex)
                {
                    exception = ex;
                    if (OnError != null)
                        OnError(ex);
                    else
                        Logger.Error(ex);

                    if (Config.Environment == EnvironmentType.Local)
                        throw;
                    else
                    {
                        if (Router.IsCmsPage)
                        {
                            // TODO: error redirect
                            //context.Response.Redirect($"/ErrorPages/error404.htm?aspxerrorpath={context.Request.RelativeUrl()}");
                        }
                    }
                }
                finally
                {
                    try
                    {
                        sw.Stop();

                        var endDate = startDate.Add(sw.Elapsed);
                        //DbLogger.AddLog(exception, "API", null, null, null, null, false, startDate, endDate);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        throw;
                    }

                    try
                    {
                        responseBody.Position = 0;
                        await responseBody.CopyToAsync(originalBodyStream);
                        context.Response.Body = originalBodyStream;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        throw;
                    }
                }
            }
        }

        private bool TryRedirect(Microsoft.AspNetCore.Http.HttpContext context)
        {
            //TODO: redirects
            //string toUrl = UrlRedirectsCache.Instance.GetRedirect(context);

            //if (!string.IsNullOrWhiteSpace(toUrl))
            //{
            //    context.Response.Redirect(toUrl, true);
            //    return true;
            //}

            return false;
        }

        private bool IsTrailingSlash(Microsoft.AspNetCore.Http.HttpContext context)
        {
            if (!context.Request.Path.Value.EndsWith('/'))
            {
                string toUrl = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.PathBase}{context.Request.Path}/{context.Request.QueryString}";

                context.Response.Redirect(toUrl, true);
                return false;
            }

            return true;
        }
    }
}
