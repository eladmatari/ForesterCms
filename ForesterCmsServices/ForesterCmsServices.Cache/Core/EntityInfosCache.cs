using ForesterCmsServices.Cache.Base;
using ForesterCmsServices.Objects.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.Cache.Core
{
    public class EntityInfosCache : BaseCacheList<EntityInfosCache, CmsEntityInfo>
    {
        private Dictionary<string, CmsEntityInfo> _aliasEntityInfos = null;

        public override string EntityTypeAlias { get { return "entityinfo"; } }

        protected override List<CmsEntityInfo> GetItemsAllFromDB()
        {
            throw new NotImplementedException();
        }

        protected override void OnLoaded()
        {
            _aliasEntityInfos = Items.GroupBy(i => (i.Alias ?? "").Trim().ToLower()).ToDictionary(i => i.Key, i => i.First());
        }

        public CmsEntityInfo GetItem(string alias)
        {
            CmsEntityInfo entityInfo;
            _aliasEntityInfos.TryGetValue(alias ?? "", out entityInfo);
            return entityInfo;
        }
    }
}
