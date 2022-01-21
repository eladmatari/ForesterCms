using Common.Utils.Standard;
using ForesterCmsServices.Cache.Base;
using ForesterCmsServices.Logic;
using ForesterCmsServices.Objects.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.Cache.Core
{
    public class EntityInfosCache : BaseCacheCustom<EntityInfosCache>
    {
        public EntityInfosCache()
        {
            SetOnRefresh(() =>
            {
                CmsServicesManager.Core.EntityInfosRemoveCache();
            });
        }

        public override string EntityTypeAlias { get { return "entity_info"; } }

        public List<CmsEntityInfo> Items { get; private set; }
        public Dictionary<int, CmsEntityInfo> ItemsByObjIdDict { get; private set; }
        public Dictionary<string, CmsEntityInfo> ItemsByAliasDict { get; private set; }

        protected override void Init()
        {
            Items = CmsServicesManager.Core.GetEntityInfos();
            ItemsByObjIdDict = Items.ToDictionary(i => i.ObjId);
            ItemsByAliasDict = Items.ToDictionary(i => i.Alias);
        }

        public CmsEntityInfo GetItem(int id)
        {
            CmsEntityInfo ei;
            ItemsByObjIdDict.TryGetValue(id, out ei);
            return ei;
        }

        public CmsEntityInfo GetItem(string alias)
        {
            CmsEntityInfo ei;
            ItemsByAliasDict.TryGetValue(alias, out ei);
            return ei;
        }
    }
}
