using Common.Utils.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.Objects
{
    public static class CmsConfig
    {
        public static bool IsCms { get { return Config.GetAppSettings("ForesterCms.IsCms") == "1"; } }
        public static bool IsSite { get { return Config.GetAppSettings("ForesterCms.IsSite") == "1"; } }
    }
}
