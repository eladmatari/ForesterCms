using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.Routing
{
    public class VirtualRoute
    {
        private string _routeMatchingRegex;
        public readonly string BaseRoute;
        public readonly VirtualRouteParam[] Parameters;

        public VirtualRoute(string baseRoute, params VirtualRouteParam[] parameters)
        {
            if (baseRoute == null)
                throw new ArgumentNullException(nameof(baseRoute));

            BaseRoute = baseRoute;
            Parameters = parameters;
            _routeMatchingRegex = "^/[^/]+/" + BaseRoute.Trim(' ', '\\', '/').Replace("\\", "/") + "/";
        }

        public bool IsMatch(string relativePath)
        {
            if (!relativePath.EndsWith("\\"))
                relativePath = relativePath + "\\";

            return Regex.IsMatch(relativePath, _routeMatchingRegex, RegexOptions.IgnoreCase);
        }

        public NameValueCollection GetParametersValues(string relativePath)
        {
            if (!relativePath.EndsWith("/"))
                relativePath = relativePath + "/";

            var parametersValues = new NameValueCollection();
            var match = Regex.Match(relativePath, _routeMatchingRegex, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                var path = relativePath.Substring(match.Value.Length);
                var parametersValuesArr = path.Split('/');

                if (parametersValuesArr.Length - 1 <= Parameters.Length)
                {
                    for (int i = 0; i < Parameters.Length; i++)
                    {
                        var parameter = Parameters[i];

                        if (parametersValuesArr.Length > i)
                            parametersValues[parameter.Alias] = parametersValuesArr[i];
                        else
                            parametersValues[parameter.Alias] = parameter.DefaultValue;
                    }

                    return parametersValues;
                }
            }


            return null;
        }

        public string GetRoute(string relativePath)
        {
            if (!relativePath.EndsWith("/"))
                relativePath = relativePath + "/";

            var match = Regex.Match(relativePath, _routeMatchingRegex, RegexOptions.IgnoreCase);

            return match.Value;
        }
    }
}
