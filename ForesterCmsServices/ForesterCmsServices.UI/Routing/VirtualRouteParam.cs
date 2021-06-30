using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.Routing
{
    public class VirtualRouteParam
    {
        public readonly string Alias;

        public readonly string DefaultValue;

        public VirtualRouteParam(string alias, string defaultValue = null)
        {
            Alias = alias;
            DefaultValue = defaultValue;
        }
    }
}
