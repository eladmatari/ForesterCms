using Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.Routing.Cms
{
    public static class CmsRouter
    {
        public static CmsRouterData Data
        {
            get
            {
                return HttpContextHelper.Current.Items["CmsRouter.RouterData"] as CmsRouterData;
            }
            internal set
            {
                HttpContextHelper.Current.Items["CmsRouter.RouterData"] = value;
            }
        }
    }
}
