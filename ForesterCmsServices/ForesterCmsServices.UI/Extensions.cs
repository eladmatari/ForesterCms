using Common.Utils.Standard;
using ForesterCmsServices.Objects.Core;
using ForesterCmsServices.UI.Models;
using ForesterCmsServices.UI.Resources;
using ForesterCmsServices.UI.Routing;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI
{
    public static class Extensions
    {
        public static string GetValue(this RouteValueDictionary dict, string key)
        {
            object val;
            if (dict.TryGetValue(key, out val))
                return (string)val;

            return null;
        }

        public static T GetValue<T>(this RouteValueDictionary dict, string key)
        {
            object val;
            if (dict.TryGetValue(key, out val))
                return (T)val;

            return default(T);
        }

        public static async Task<IHtmlContent> AppPlaceHolder(this IHtmlHelper htmlHelper, string key)
        {
            return await htmlHelper.PartialAsync("~/Views/Cms/_AppPlaceHolder.cshtml", "Main");
        }

        public static HtmlString BundleScripts(this IHtmlHelper htmlHelper, string key, params string[] scriptsPaths)
        {
            if (scriptsPaths.Length == 0)
                return null;

            string filePath = ResourceGroupHelper.Scripts.GetOrAddBundle(key, scriptsPaths);

            return new HtmlString($"<script src=\"{filePath}\"></script>");
        }

        public static HtmlString BundleCss(this IHtmlHelper htmlHelper, string key, params string[] cssPaths)
        {
            if (cssPaths.Length == 0)
                return null;

            string filePath = ResourceGroupHelper.Css.GetOrAddBundle(key, cssPaths);

            return new HtmlString($"<link href=\"{filePath}\" rel=\"stylesheet\" />");
        }

        public static HtmlString BundleModulesScripts(this IHtmlHelper htmlHelper)
        {
            var scripts = Router.Data.PageModel.GetScripts();
            if (scripts.Length == 0)
                return null;

            string hash = Regex.Replace(CryptHelper.GetHash(string.Join("|", scripts)), "[^A-Za-z0-9]", "-");

            return htmlHelper.BundleScripts($"modules-{hash}", scripts);
        }

        public static HtmlString BundleModulesCss(this IHtmlHelper htmlHelper)
        {
            var css = Router.Data.PageModel.GetCss();
            if (css.Length == 0)
                return null;

            string hash = Regex.Replace(CryptHelper.GetHash(string.Join("|", css)), "[^A-Za-z0-9]", "-");

            return htmlHelper.BundleCss($"modules-{hash}", css);
        }

        public static HtmlString GetWebpackScripts(this IHtmlHelper htmlHelper, params string[] dependencies)
        {
            var scripts = Router.Data.PageModel.GetWebpackScripts();
            string folder = ResourceGroupHelper.IsDebugModeEnabled == true ? "dev" : "prod";
            string version = $"v={WebpackHelper.Instance.VersionUniqueId}";
            var sb = new StringBuilder();

            foreach (var dependency in dependencies)
            {
                sb.AppendLine($"<script src=\"webpack/{folder}/{dependency}.js?{version}\"></script>");
            }

            var dict = new Dictionary<string, object>();

            foreach (var script in scripts.Select(i => i.Trim().ToLower()))
            {
                if (dict.ContainsKey(script))
                    continue;

                dict[script] = null;
                sb.AppendLine($"<script src=\"webpack/{folder}/{script}.js?{version}\"></script>");
            }

            return new HtmlString(sb.ToString());
        }

        public static HtmlString GetWebpackCss(this IHtmlHelper htmlHelper, params string[] dependencies)
        {
            var cssItems = Router.Data.PageModel.GetWebpackCss();
            string folder = ResourceGroupHelper.IsDebugModeEnabled == true ? "dev" : "prod";
            string version = $"v={WebpackHelper.Instance.VersionUniqueId}";
            var sb = new StringBuilder();

            foreach (var dependency in dependencies)
            {
                sb.AppendLine($"<link href=\"webpack/{folder}/{dependency}.css?{version}\" type=\"text/css\" rel=\"stylesheet\"/>");
            }

            var dict = new Dictionary<string, object>();

            foreach (var css in cssItems.Select(i => i.Trim().ToLower()))
            {
                if (dict.ContainsKey(css))
                    continue;

                dict[css] = null;
                sb.AppendLine($"<link href=\"webpack/{folder}/{css}.css?{version}\" type=\"text/css\" rel=\"stylesheet\"/>");
            }

            return new HtmlString(sb.ToString());
        }

        public static string AbsoluteUrl(this HttpRequest request)
        {
            return $"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}{request.QueryString}";
        }

        public static string AbsoluteUrlNoQueryString(this HttpRequest request)
        {
            return $"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}";
        }

        public static string AbsoluteUrlNoQueryString(this Uri uri)
        {
            if (uri.IsDefaultPort)
                return $"{uri.Scheme}://{uri.Host}{uri.AbsolutePath}";

            else return $"{uri.Scheme}://{uri.Host}:{uri.Port}{uri.AbsolutePath}";
        }

        public static string RelativeUrl(this HttpRequest request)
        {
            return $"{request.PathBase}{request.Path}{request.QueryString}";
        }

        public static string RelativeUrlNoQueryString(this HttpRequest request)
        {
            return $"{request.PathBase}{request.Path}";
        }

        //public static async Task<IHtmlContent> PrintImage(this IHtmlHelper htmlHelper, Image image, bool isLazyLoad = false, bool isFullUrl = false)
        //{
        //    return await htmlHelper.PartialAsync("~/Views/Cms/_Image.cshtml", image, new ViewDataDictionary(htmlHelper.ViewData)
        //    {
        //        {
        //            "IsFullUrl", isFullUrl
        //        },
        //        {
        //            "IsLazyLoad", isLazyLoad
        //        }
        //    });
        //}

        //public static async Task<IHtmlContent> PrintImageLazy(this IHtmlHelper htmlHelper, Image image)
        //{
        //    return await htmlHelper.PrintImage(image, isLazyLoad: true);
        //}

        public static string GetUnit(this IHtmlHelper htmlHelper, string unit, string defaultValue = null)
        {
            if (string.IsNullOrWhiteSpace(unit))
                return defaultValue;

            int unitInt;
            if (int.TryParse(unit, out unitInt))
                return unitInt + "px";

            return unit;
        }

        //public static string GetUploadedFileUrl(this IHtmlHelper htmlHelper, string fileUrl)
        //{
        //    if (string.IsNullOrWhiteSpace(fileUrl))
        //        return null;

        //    if (fileUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        //        return fileUrl;

        //    return "uploadedfiles/" + fileUrl;
        //}

        //public static async Task<IHtmlContent> ImageLinkAsync(this IHtmlHelper htmlHelper, ImageLinkModel imageLinkModel)
        //{
        //    return await htmlHelper.PartialAsync("~/Views/Cms/_Imagelink.cshtml", imageLinkModel);
        //}

        public static string GetValue(this BaseCmsEntity.MetaObject metaObj, MetaTemplate.TemplateType type)
        {
            switch (type)
            {
                case MetaTemplate.TemplateType.Title:
                    return metaObj.Title;
                case MetaTemplate.TemplateType.Keywords:
                    return metaObj.Keywords;
                case MetaTemplate.TemplateType.Description:
                    return metaObj.Description;
                case MetaTemplate.TemplateType.Robots:
                    return metaObj.Robots;
            }

            return null;
        }
    }
}
