using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.Routing
{
    public class VirtualRouteTranslate
    {
        private string _routeMatchingRegex;
        public readonly string BaseRoute;
        public readonly string TargetRoute;

        public VirtualRouteTranslate(string baseRoute, string targetRoute)
        {
            if (baseRoute == null)
                throw new ArgumentNullException(nameof(baseRoute));

            BaseRoute = baseRoute;
            TargetRoute = targetRoute.TrimEnd('/') + "/";
            _routeMatchingRegex = "^/[^/]+/" + BaseRoute.Trim(' ', '/') + "/";

        }

        public bool IsMatch(string relativePath)
        {
            if (!relativePath.EndsWith("/"))
                relativePath = relativePath + "/";

            return Regex.IsMatch(relativePath, _routeMatchingRegex, RegexOptions.IgnoreCase);
        }

        public string GetTranslate(string relativePath)
        {
            if (!relativePath.EndsWith("/"))
                relativePath = relativePath + "/";

            string lang = relativePath.TrimStart('/').Split('/').FirstOrDefault();
            string target = $"/{lang}/{(Regex.Replace(relativePath, _routeMatchingRegex, TargetRoute, RegexOptions.IgnoreCase))}";

            return target;
        }
    }
}
