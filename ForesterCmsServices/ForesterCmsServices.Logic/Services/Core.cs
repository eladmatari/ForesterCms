using Common.Utils.Standard;
using ForesterCmsServices.Objects.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.Logic.Services
{
    public class Core
    {
        private const string EntityInfosCacheKey = "Core.GetEntityInfosCached";

        public void EntityInfosRemoveCache()
        {
            CacheHelper.Remove(EntityInfosCacheKey);
        }

        public List<CmsEntityInfo> GetEntityInfosCached()
        {
            return CacheHelper.GetOrAdd(EntityInfosCacheKey, () =>
            {
                return GetEntityInfos();
            }, 60 * 30, true);
        }

        public List<CmsEntityInfo> GetEntityInfos()
        {
            var table = DBHelper.Database.ExecuteDataTable("select * from cms_entity_info");

            var results = new List<CmsEntityInfo>();

            foreach (DataRow row in table.Rows)
            {
                var entityInfo = new CmsEntityInfo();

                entityInfo.ObjId = row.Field<int>("id");
                entityInfo.Name = row.Field<string>("name");
                entityInfo.Alias = row.Field<string>("alias");
                entityInfo.IsMultiLanguage = row.Field<ulong>("multilang") == 1;
                entityInfo.Properties = row.Field<string>("properties");
                entityInfo.IsSystem = row.Field<ulong>("system") == 1;

                results.Add(entityInfo);
            }

            return results;
        }

        public DisplayPage GetDisplayPage(int entityInfoId, int objId, int branchId)
        {
            throw new NotImplementedException();
        }

        public List<BranchObject> GetObjects(int boneid, int lcid)
        {
            return new List<BranchObject>();
            // TODO: implement 
            DataSet ds = null;// Data.Core.GetObjects(boneid, lcid);

            var ob = new List<BranchObject>();

            foreach (DataRow item in ds.Tables[0].Rows)
            {
                ob.Add(new BranchObject()
                {
                    BranchId = Convert.ToInt32(item["branchId"]),
                    LCID = Convert.ToInt32(item["lcid"]),
                    EntityInfoId = Convert.ToInt32(item["eid"]),
                    ObjId = Convert.ToInt32(item["objid"]),
                    Sort = Convert.ToInt32(item["sort"]),
                });
            }

            return ob;
        }

        private DataTable GetTable(string alias)
        {
            var ei = GetEntityInfosCached().FirstOrDefault(i => i.Alias == alias);
            if (ei == null)
                throw new Exception($"table \"{alias}\" wasn't found");

            return GetTable(ei.ObjId, "cms_" + ei.Alias);
        }

        private DataTable GetTable(int tableId, string tableName)
        {
            return DBHelper.Database.ExecuteDataTable($@"SELECT 
    t.*,
    o.eid,
	o.name,
    o.status,
    o.createdate,
    o.updatedate,
    o.sort
FROM
	{tableName} t
	join
    cms_object o
    on
    t.id = o.objid and t.lcid = o.lcid and o.eid = 2");
        }

        public List<CmsBranch> GetBranches()
        {
            var table = GetTable("branch");
            //            var table = DBHelper.Database.ExecuteDataTable($@"SELECT 
            //    b.*,
            //    o.eid,
            //	o.name,
            //    o.status,
            //    o.createdate,
            //    o.updatedate,
            //    o.sort
            //FROM
            //	cms_branch b
            //	join
            //    cms_object o
            //    on
            //    b.id = o.objid and b.lcid = o.lcid and o.eid = 2");

            var results = new List<CmsBranch>();

            foreach (DataRow row in table.Rows)
            {
                var item = new CmsBranch();

                item.SetBaseData(row);
                item.ObjId = row.Field<int>("id");
                item.LCID = row.Field<int>("lcid");
                item.Alias = row.Field<string>("alias");
                item.IsSystem = row.Field<UInt64>("system") == 1;
                item.ParentId = row.Field<int?>("parentId");

                results.Add(item);
            }

            return results;
        }

        public List<CmsLanguage> GetLanguages()
        {
            var table = DBHelper.Database.ExecuteDataTable("select * from cms_language");

            var results = new List<CmsLanguage>();

            foreach (DataRow row in table.Rows)
            {
                var item = new CmsLanguage();

                item.ObjId = row.Field<int>("id");
                item.Name = row.Field<string>("name");
                item.Alias = row.Field<string>("alias");
            }

            return results;
        }

        public List<CmsComponent> GetPageControlsByBoneId(int boneId, int lcid)
        {
            // TODO: implement
            throw new NotImplementedException();
            //var ds = Data.Core.GetPageControls(-1, boneId, lcid);

            //return BuildPageControls(ds);
        }
    }
}
