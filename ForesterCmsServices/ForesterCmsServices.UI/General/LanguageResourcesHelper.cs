using ForesterCmsServices.Cache;
using ForesterCmsServices.Objects.Core;
using ForesterCmsServices.UI.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.General
{
    public static class LanguageResourcesHelper
    {
        public static LanguageResource GetResource(string key, string @namespace = null, int? lcid = null)
        {
            return GetResource(key, false, @namespace, lcid);
        }

        public static LanguageResource GetResource(string key, bool isFullNameSpace = false, string @namespace = null, int? lcid = null)
        {
            if (lcid == null)
                lcid = Router.Data?.LCID ?? 0;

            return CacheManager.LanguageResources.GetItem(lcid.Value, key, isFullNameSpace, @namespace);
        }

        public static string GetText(string key, string @namespace = null, int? lcid = null)
        {
            return GetText(key, false, @namespace, lcid);
        }

        public static string GetText(string key, bool isFullNameSpace = false, string @namespace = null, int? lcid = null)
        {
            if (lcid == null)
                lcid = Router.Data?.LCID ?? 0;

            var resource = CacheManager.LanguageResources.GetItem(lcid.Value, key, isFullNameSpace, @namespace);
            if (resource != null)
            {
                if (!string.IsNullOrWhiteSpace(resource.Text))
                    return resource.Text;

                return resource.Name;
            }

            return null;
        }

        public static CmsImage GetImage(string key, string @namespace = null, int? lcid = null)
        {
            return GetImage(key, false, @namespace, lcid);
        }

        public static CmsImage GetImage(string key, bool isFullNameSpace = false, string @namespace = null, int? lcid = null)
        {
            if (lcid == null)
                lcid = Router.Data?.LCID ?? 0;

            return CacheManager.LanguageResources.GetItem(lcid.Value, key, isFullNameSpace, @namespace)?.Image;
        }
    }
}
