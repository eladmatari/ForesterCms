using Common.Utils.Standard;
using ForesterCmsServices.Cache.Base;
using ForesterCmsServices.Objects.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.Cache.Core
{
    public class LanguageResourcesCache : BaseCacheList<LanguageResourcesCache, LanguageResource>
    {
        public override string EntityTypeAlias
        {
            get
            {
                return "languageresources";
            }
        }

        private Dictionary<string, LanguageResource> _languageResourcesDict = new Dictionary<string, LanguageResource>();

        protected override List<LanguageResource> GetItemsAllFromDB()
        {
            throw new NotImplementedException();
            //return eGenServicesManager.Core.GetLanguageResources();
        }

        protected override void OnLoaded()
        {
            foreach (var itemsGroup in Items.GroupBy(i => i.BranchId))
            {
                var bones = CacheManager.Branches.GetAncestorsAndSelf(itemsGroup.Key);
                string @namespace = string.Join(".", bones.Select(i => i.Alias));

                foreach (var itemsGroupResource in itemsGroup)
                {
                    _languageResourcesDict[$"{itemsGroupResource.LCID}.{@namespace}.{itemsGroupResource.Key}"] = itemsGroupResource;
                }
            }
        }

        public LanguageResource GetItem(int lcid, string key, string @namespace = null)
        {
            return GetItem(lcid, key, false, @namespace);
        }

        public LanguageResource GetItem(int lcid, string key, bool isFullNameSpace = false, string @namespace = null)
        {
            if (!isFullNameSpace)
            {
                string defaultNamespace = Config.GetAppSettings("ForesterCms.LanguageResourcesDefaultNamespace");

                if (string.IsNullOrEmpty(@namespace))
                    @namespace = defaultNamespace;
                else
                    @namespace = $"{defaultNamespace}.{@namespace}";
            }

            string fullKey = $"{lcid}.{@namespace}.{key}".ToLower();
            LanguageResource res;
            if (_languageResourcesDict.TryGetValue(fullKey, out res))
                return res;

            return null;
        }
    }
}
