using Common.Utils;
using Common.Utils.Standard;
using ForesterCmsServices.Cache;
using ForesterCmsServices.Logic;
using ForesterCmsServices.Objects;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.Routing
{
    public class Router : DynamicRouteValueTransformer
    {
        public override async ValueTask<RouteValueDictionary> TransformAsync(Microsoft.AspNetCore.Http.HttpContext httpContext, RouteValueDictionary values)
        {
            values.Clear();

            if (CmsConfig.IsCms && httpContext.Request.Path.Value.StartsWith("/ForesterCms/", StringComparison.OrdinalIgnoreCase))
            {
                values["controller"] = "Home";
                values["action"] = "Index";
                values["area"] = "ForesterCms";
            }
            else if (CmsConfig.IsSite)
            {
                var data = GetRouteData(httpContext);

                if (data != null)
                {
                    Data = data;
                    values["controller"] = data.RouteParams.Controller;
                    values["action"] = data.RouteParams.Action;
                }
            }

            return values;
        }

        private static readonly List<VirtualRoute> _virtualRoutes = new List<VirtualRoute>();
        private static readonly List<VirtualRouteTranslate> _virtualRoutesTranslates = new List<VirtualRouteTranslate>();

        private static readonly Dictionary<string, Func<PreviewParams, string>> _previewDict = new Dictionary<string, Func<PreviewParams, string>>();

        public static void AddRoute(VirtualRoute virtualRoute)
        {
            _virtualRoutes.Add(virtualRoute);
        }

        public static void AddRouteTranslate(VirtualRouteTranslate virtualRouteTranslate)
        {
            _virtualRoutesTranslates.Add(virtualRouteTranslate);
        }

        private static readonly char[] _pathSplitter = new char[] { '/' };

        public static RouterData GetRouteData(Microsoft.AspNetCore.Http.HttpContext context)
        {
            var data = new RouterData();

            string urlPath = context.Request.Path.Value;

            var virtualRouteTranslate = _virtualRoutesTranslates.FirstOrDefault(i => i.IsMatch(urlPath));
            if (virtualRouteTranslate != null)
                urlPath = virtualRouteTranslate.GetTranslate(urlPath);

            var virtualRoute = _virtualRoutes.FirstOrDefault(i => i.IsMatch(urlPath));
            if (virtualRoute != null)
            {
                data.VirtualRouteParams = virtualRoute.GetParametersValues(urlPath);
                if (data.VirtualRouteParams == null)
                    return null;

                urlPath = virtualRoute.GetRoute(urlPath);
            }

            var pathArr = urlPath.Split(_pathSplitter, StringSplitOptions.RemoveEmptyEntries);

            // redirect empty url to homepage
            if (pathArr.Length == 0)
            {
                string homePageUrl = Config.GetAppSettings("ForesterCms.HomePageUrl");

                if (!string.IsNullOrEmpty(homePageUrl))
                {
                    context.Response.Redirect(context.Request.PathBase + "/" + homePageUrl.Trim('/') + "/");
                    return null;
                }
            }

            string langShortName = pathArr[0];
            var lang = CacheManager.Languages.GetItem(langShortName);

            if (lang == null)
                return null;

            data.Language = lang;

            var aliases = pathArr.Skip(1).ToList();
            if (aliases.Count == 0)
                return null;

            string lastAlias = aliases[aliases.Count - 1];

            if (lastAlias.Contains(','))
            {
                var objPath = lastAlias.Split(',');
                if (objPath.Length == 2)
                {
                    data.EntityInfo = CacheManager.EntityInfos.GetItem(objPath[0]);

                    int objId = RequestHelper.GetInt(objPath[1]);
                    if (objId > 0)
                        data.ObjId = objId;
                    //else
                    //    data.ObjAlias = RequestHelper.SecureRequestParam(objPath[1]);

                    if (data.ObjId == 0 || data.EntityInfo == null)
                        return null;

                    aliases.RemoveAt(aliases.Count - 1);
                }
            }

            data.Branch = CacheManager.Branches.GetItem(string.Concat(
                Config.GetAppSettings("ForesterCms.BaseBranchAlias"),
                ".",
                string.Join(".", aliases)
            ));

            if (data.Branch == null)
                return null;

            var branchProperties = CacheManager.BranchesProperties.GetItemByBranchId(data.BranchId, data.LCID);
            if (branchProperties == null)
                return null;

            data.BranchProperties = branchProperties;

            if (data.ObjId > 0 && data.EntityInfoId > 0)
            {
                data.DisplayPage = CmsServicesManager.Core.GetDisplayPage(data.EntityInfoId, data.ObjId, data.BranchId);
            }
            else
            {
                data.EntityInfoId = branchProperties.EntityInfoId;
                data.ObjId = branchProperties.ObjId;
                data.DisplayPage = CmsServicesManager.Core.GetDisplayPage(branchProperties.EntityInfoId, branchProperties.ObjId, data.BranchId);
            }

            if (data.DisplayPage == null)
                return null;

            if (data.RouteParams.IsOldSitePage && Config.Environment == EnvironmentType.Prod)
            {
                context.Response.Redirect("/digitalsite" + context.Request.RelativeUrl().Substring(3));
                return null;
            }

            data.IsMobile = GetOrAddMobileCookie(context);

            return data;
        }

        private static bool GetOrAddMobileCookie(Microsoft.AspNetCore.Http.HttpContext context)
        {
            string val;
            if (!context.Request.Cookies.TryGetValue("mobile", out val))
            {
                if (context.Request.Headers["User-Agent"].Any(i => i.IndexOf("Mobile", StringComparison.OrdinalIgnoreCase) > 0))
                    val = "1";
                else
                    val = "0";

                context.Response.Cookies.Append("mobile", val, new Microsoft.AspNetCore.Http.CookieOptions()
                {
                    Expires = DateTime.Now.AddDays(365),
                    HttpOnly = false,
                    Path = "/"
                });
            }

            return val == "1";
        }

        public static RouterData Data
        {
            get
            {
                return HttpContextHelper.Current.Items["AppRouter.AppRouterData"] as RouterData;
            }
            internal set
            {
                HttpContextHelper.Current.Items["AppRouter.AppRouterData"] = value;
            }
        }

        public static void SetNonCmsRouteData(Microsoft.AspNetCore.Http.HttpContext context)
        {
            var data = new RouterData();
            data.IsMobile = GetOrAddMobileCookie(context);

            Data = data;
        }

        private static string GetPreviewDictKey(string controller, string action)
        {
            return $"{controller}${action}";
        }

        public static void SetPreview(string controller, string action, Func<PreviewParams, string> getUrlMethod)
        {
            if (getUrlMethod == null)
                throw new ArgumentNullException(nameof(getUrlMethod));

            _previewDict[GetPreviewDictKey(controller, action)] = getUrlMethod;
        }

        public static string GetUrl(int boneId, int lcid)
        {
            return GetUrl(boneId, lcid, 0, 0);
        }

        public static string GetUrl(int branchId, int lcid, int entityInfoId, int objId)
        {
            var branch = CacheManager.Branches.GetItem(branchId);
            if (branch == null)
                return null;

            bool isBranchPropertyView = true;

            if (entityInfoId > 0 && objId > 0)
            {
                if (branch.TreeAlias == "Tools")
                {
                    var displayPage = CmsServicesManager.Core.GetDisplayPage(entityInfoId, objId, branchId);
                    var routeParams = RouterData.GetRouteParams(displayPage.View);

                    string key = GetPreviewDictKey(routeParams.Controller, routeParams.Action);
                    if (_previewDict.ContainsKey(key))
                        return _previewDict[key](new PreviewParams() { BranchId = branchId, LCID = lcid, EntityInfoId = entityInfoId, ObjId = objId });
                }

                isBranchPropertyView = false;
            }

            string url = string.Join("/", CacheManager.Branches.GetAncestorsAndSelf(branchId).Skip(1).Select(i => i.Alias.ToLower()));
            url = CacheManager.Languages.GetItem(lcid).Alias + "/" + url + "/";

            if (!isBranchPropertyView)
            {
                var entity = CacheManager.EntityInfos.GetItem(entityInfoId);
                url = $"{url}{entity.Alias},{objId}/";
            }

            return url;
        }

        public static bool IsCmsPage
        {
            get
            {
                return Data != null && Data.BranchId > 0;
            }
        }
    }
}
