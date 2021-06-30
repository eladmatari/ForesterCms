using Common.Utils;
using Common.Utils.Standard;
using Microsoft.AspNetCore.Rewrite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.Resources
{
    public class ResourceRewriteRule : IRule
    {
        private static string[] _extensions = new string[]
        {
            ".js",
            ".css",
           // ".map"
        };
        private static readonly Dictionary<string, string> _resourcesPaths = new Dictionary<string, string>();
        private static readonly object _resourcesPathsLockObj = new object();

        public void ApplyRule(RewriteContext context)
        {
            string path = context.HttpContext.Request.Path.Value;
            if (!_extensions.Any(i => context.HttpContext.Request.Path.Value.EndsWith(i, StringComparison.OrdinalIgnoreCase)))
                return;

            string newPath = null;
            if (!_resourcesPaths.TryGetValue(path, out newPath))
            {
                if (path.IndexOf("/static/", StringComparison.OrdinalIgnoreCase) == -1)
                    return;

                lock (_resourcesPathsLockObj)
                {
                    if (!_resourcesPaths.TryGetValue(path, out newPath))
                    {
                        string extension = "." + string.Join(".", path.Split('.').Skip(1));
                        newPath = path.Substring(0, path.Length - extension.Length) + "_" + Config.CreateDate.Ticks + extension;
                        string filePath = Path.Combine(DiHelper.Environment.WebRootPath, newPath.TrimStart('/').Replace('/', '\\'));
                        if (File.Exists(filePath))
                            _resourcesPaths[path] = newPath;
                    }
                }
            }

            if (Config.Environment != EnvironmentType.Prod)
                HttpContextHelper.Current.Response.Headers.Add("RewritelUrl", newPath);

            context.HttpContext.Request.Path = new Microsoft.AspNetCore.Http.PathString(newPath);
            context.Result = RuleResult.SkipRemainingRules;
        }
    }

    public static class ResourceRewriteExtensions
    {
        public static RewriteOptions AddStaticRewrite(this RewriteOptions options)
        {
            options.Add(new ResourceRewriteRule());
            return options;
        }
    }
}
