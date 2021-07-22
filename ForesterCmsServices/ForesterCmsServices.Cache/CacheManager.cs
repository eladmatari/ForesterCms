using ForesterCmsServices.Cache.Core;
using ForesterCmsServices.Objects.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.Cache
{
    public class CacheManager
    {
        public static event EventHandler<CacheRefreshEventArgs> OnCacheRefresh;

        public static LanguagesCache Languages
        {
            get
            {
                return LanguagesCache.Instance;
            }
        }

        public static EntityInfosCache EntityInfos
        {
            get
            {
                return EntityInfosCache.Instance;
            }
        }

        public static BranchesCache Branches
        {
            get
            {
                return BranchesCache.Instance;
            }
        }

        public static BranchesPropertiesCache BranchesProperties
        {
            get
            {
                return BranchesPropertiesCache.Instance;
            }
        }

        public static LanguageResourcesCache LanguageResources
        {
            get
            {
                return LanguageResourcesCache.Instance;
            }
        }
    }
}
