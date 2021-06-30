using ForesterCmsServices.Cache.Base;
using ForesterCmsServices.Objects.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.Cache.Core
{
    public class BranchesPropertiesCache : BaseCacheList<BranchesPropertiesCache, CmsBranchProperties>
    {
        public override string EntityTypeAlias { get { return "branchproperties"; } }
        private Dictionary<int, List<CmsBranchProperties>> _dictByBranchId;

        protected override List<CmsBranchProperties> GetItemsAllFromDB()
        {
            throw new NotImplementedException();
        }

        protected override void OnLoaded()
        {
            _dictByBranchId = Items.GroupBy(i => i.BranchId).ToDictionary(i => i.Key, i => i.ToList());
        }

        public CmsBranchProperties GetItemByBranchId(int branchId, int lcid)
        {
            List<CmsBranchProperties> bps;
            if (_dictByBranchId.TryGetValue(branchId, out bps))
                return bps.FirstOrDefault(i => i.LCID == lcid);

            return null;
        }
    }
}
