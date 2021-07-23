using Common.Utils.Standard;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.Base
{
    public abstract class BasePageModel
    {
        private readonly object _resourcesDictLockObj = new object();
        private Dictionary<string, object> _resourcesDict = new Dictionary<string, object>();
        private List<string> _scripts = new List<string>();
        private List<string> _css = new List<string>();
        private List<string> _webpackScripts = new List<string>();
        private List<string> _webpackCss = new List<string>();
        public Dictionary<string, object> ClientData { get; private set; } = new Dictionary<string, object>();

        private void AddResources(string[] resourcePaths, List<string> targetList)
        {
            lock (_resourcesDictLockObj)
            {
                foreach (string resourcePath in resourcePaths)
                {
                    string path = resourcePath.ToLower();

                    if (!_resourcesDict.ContainsKey(path))
                    {
                        _resourcesDict[path] = null;
                        targetList.Add(path);
                    }
                }
            }
        }

        public void AddScripts(params string[] scriptsPaths)
        {
            AddResources(scriptsPaths, _scripts);
        }

        public string[] GetScripts()
        {
            return _scripts.ToArray();
        }

        public void AddCss(params string[] cssPaths)
        {
            AddResources(cssPaths, _css);
        }

        public string[] GetCss()
        {
            return _css.ToArray();
        }

        public void AddWebpackScripts(params string[] scriptsPaths)
        {
            lock (_resourcesDictLockObj)
            {
                _webpackScripts.AddRange(scriptsPaths);
            }
        }

        public string[] GetWebpackScripts()
        {
            return _webpackScripts.ToArray();
        }

        public void AddWebpackCss(params string[] cssPaths)
        {
            lock (_resourcesDictLockObj)
            {
                _webpackCss.AddRange(cssPaths);
            }
        }

        public string[] GetWebpackCss()
        {
            return _webpackCss.ToArray();
        }

        public HtmlString GetClientDataScript()
        {
            var sb = new StringBuilder();

            sb.Append("<script>");

            foreach (string key in ClientData.Keys)
            {
                sb.Append($"datasource['{key}']={JsonHelper.Serialize(ClientData[key])};");
            }

            sb.Append("</script>");

            return new HtmlString(sb.ToString());
        }
    }
}
