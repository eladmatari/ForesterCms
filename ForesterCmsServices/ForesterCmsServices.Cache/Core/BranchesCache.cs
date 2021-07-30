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
    public class BranchesCache : BaseCacheList<BranchesCache, CmsBranch>
    {
        public override string EntityTypeAlias { get { return "branch"; } }

        private Dictionary<string, CmsBranch> _branchesAliases;

        protected override List<CmsBranch> GetItemsAllFromDB()
        {
            return CmsServicesManager.Core.GetBranches();
        }

        protected override void OnLoaded()
        {
            var branchesAliases = new Dictionary<string, CmsBranch>();

            foreach (var branch in Items)
            {
                string alias = string.Join(".", GetAncestorsAndSelf(branch.ObjId).Select(i => i.Alias));
                branchesAliases[alias] = branch;
            }

            _branchesAliases = branchesAliases;
        }

        public CmsBranch GetItem(string alias)
        {
            if (alias == null)
                return null;

            CmsBranch branch;
            _branchesAliases.TryGetValue(alias.Trim().ToLower(), out branch);
            return branch;
        }

        public List<CmsBranch> GetChildren(int branchId, int counter = 0)
        {
            if (counter > 50)
                throw new Exception("circular bone id: " + branchId);

            var children = Items.Where(i => i.ParentId == branchId).ToList();
            var subChildren = new List<CmsBranch>();

            foreach (var child in children)
            {
                subChildren.AddRange(GetChildren(child.ObjId, counter + 1));
            }

            return children.Concat(subChildren).ToList();
        }

        public List<CmsBranch> GetAncestorsAndSelf(int branchId)
        {
            var branch = GetItem(branchId);
            var resultBranches = new List<CmsBranch>();

            var counter = 0;

            while (branch != null)
            {
                counter++;
                if (counter > 50)
                    throw new Exception("Circular branch id " + branch.ObjId);

                resultBranches.Add(branch);
                if (branch.ParentId == null)
                    break;

                branch = GetItem(branch.ParentId.Value);
            }

            resultBranches.Reverse();
            return resultBranches;
        }
    }
}
