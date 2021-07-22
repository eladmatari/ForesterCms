using Common.Utils.Standard;
using ForesterCmsServices.Objects.Core;
using ForesterCmsServices.UI.General;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.Models
{
    public class PageModel
    {
        public List<string> Meta { get; internal set; }
        internal Dictionary<string, ViewPlaceHolder> PlaceHolders { get; set; }
        public List<BranchObject> BranchObjects { get; internal set; }
        public ViewPlaceHolder GetPlaceHolder(string key)
        {
            ViewPlaceHolder ph;
            if (key != null && PlaceHolders.TryGetValue(key.ToLower(), out ph))
                return ph;

            return null;
        }

        public List<ViewPlaceHolder> GetPlaceHolders()
        {
            return PlaceHolders.Values.ToList();
        }

        private readonly object _resourcesDictLockObj = new object();
        private Dictionary<string, object> _resourcesDict = new Dictionary<string, object>();
        private List<string> _scripts = new List<string>();
        private List<string> _css = new List<string>();
        private List<string> _webpackScripts = new List<string>();
        private List<string> _webpackCss = new List<string>();
        public Dictionary<string, object> ClientData { get; private set; } = new Dictionary<string, object>();
        public List<BranchObject> PageObjects { get; internal set; }

        public List<string> _htmlToHead = new List<string>();

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

        public void AddToHead(string html)
        {
            _htmlToHead.Add(html);
        }

        public string GetHeadHtml()
        {
            return string.Join("\n", _htmlToHead);
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
