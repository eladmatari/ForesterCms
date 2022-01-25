using Common.Utils.Logging;
using Common.Utils.Standard;
using ForesterCmsServices.Objects.Core;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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

        public void AddOrUpdateObjectData(BaseCmsEntity entity, MySqlTransaction trns = null)
        {
            var isExist = DBHelper.Database.ExecuteDataTable(@"
select 1 from `cms_object` 
where `objid` = @objId and `eid` = @eid and `lcid` = @lcid and `bid` = @bid 
LIMIT 1", (cmd) =>
            {
                cmd.Parameters.AddWithValue("@objId", entity.ObjId);
                cmd.Parameters.AddWithValue("@eid", entity.EntityInfoId);
                cmd.Parameters.AddWithValue("@lcid", entity.LCID);
                cmd.Parameters.AddWithValue("@bid", entity.BranchId);
            }, trns).Rows.Count != 0;

            if (!isExist)
            {
                DBHelper.Database.ExecuteScalar(
    @"INSERT INTO `cms_object` (`objid`, `eid`, `lcid`, `bid`, `name`, `status`, `createdate`, `updatedate`, `sort`) 
VALUES (@objid, @eid, @lcid, @bid, @name, @status, CURDATE(), CURDATE(), @sort);",
    (cmd) =>
    {
        cmd.Parameters.AddWithValue("@objid", entity.ObjId);
        cmd.Parameters.AddWithValue("@eid", entity.EntityInfoId);
        cmd.Parameters.AddWithValue("@lcid", entity.LCID);
        cmd.Parameters.AddWithValue("@bid", entity.BranchId);
        cmd.Parameters.AddWithValue("@name", entity.Name);
        cmd.Parameters.AddWithValue("@status", entity.Status);
        cmd.Parameters.AddWithValue("@sort", entity.Sort > 0 ? entity.Sort : 999);
    },
    trns);
            }
            else
            {
                DBHelper.Database.ExecuteScalar(
    @"update `cms_object` 
set
    `name` = @name,
    `status` = @status,
    `updatedate` = CURDATE(),
    `sort` = @sort
where
    `objid` = @objid
    and
    `eid` = @eid
    and
    `lcid` = @lcid
    and
    `bid` = @bid
",
    (cmd) =>
    {
        cmd.Parameters.AddWithValue("@objid", entity.ObjId);
        cmd.Parameters.AddWithValue("@eid", entity.EntityInfoId);
        cmd.Parameters.AddWithValue("@lcid", entity.LCID);
        cmd.Parameters.AddWithValue("@bid", entity.BranchId);
        cmd.Parameters.AddWithValue("@name", entity.Name);
        cmd.Parameters.AddWithValue("@status", entity.Status);
        cmd.Parameters.AddWithValue("@sort", entity.Sort > 0 ? entity.Sort : 999);
    },
    trns);
            }
        }

        public void FixBranchesSort(int? parentId, MySqlTransaction trns)
        {
            var branchesTable = DBHelper.Database.ExecuteDataTable($@"SELECT 
    `id`,
    `eid`,
    `lcid`,
    `sort`
FROM
	`cms_vw_branch`
where
    ((parentId = @parentId) or (parentId is null and @parentId is null))
order by
    `sort`
", (cmd) =>
            {
                cmd.Parameters.AddWithValue("@parentId", parentId);
            });

            for (int i = 1; i <= branchesTable.Rows.Count; i++)
            {
                var row = branchesTable.Rows[i - 1];

                DBHelper.Database.ExecuteNonQuery($@"
update
    cms_object
set
    `sort` = @sort
where
    `objid` = @objid
    and
    `eid` = @eid
    and
    `lcid` = @lcid
", (cmd) =>
                {
                    cmd.Parameters.AddWithValue("@sort", i);
                    cmd.Parameters.AddWithValue("@objid", row.Field<int>("id"));
                    cmd.Parameters.AddWithValue("@eid", row.Field<int>("eid"));
                    cmd.Parameters.AddWithValue("@lcid", row.Field<int>("lcid"));
                }, trns);
            }
        }

        public void FixBranchesSort(int? parentId)
        {
            using (var trns = DBHelper.Database.Connection.BeginTransaction())
            {
                try
                {
                    FixBranchesSort(parentId, trns);

                    trns.Commit();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    trns.Rollback();
                    throw;
                }
            }
        }

        public CmsBranch AddOrUpdateBranch(CmsBranch branch)
        {
            using (var trns = DBHelper.Database.Connection.BeginTransaction())
            {
                try
                {
                    if (branch.ObjId == 0)
                    {
                        branch.ObjId = Convert.ToInt32(DBHelper.Database.ExecuteScalar(
    @"INSERT INTO `cms_branch` (`lcid`, `alias`, `system`, `parentId`) VALUES (@lcid, @alias, b'0', @parentId);
SELECT LAST_INSERT_ID() as 'id';",
    (cmd) =>
    {
        cmd.Parameters.AddWithValue("@alias", branch.Alias);
        cmd.Parameters.AddWithValue("@lcid", branch.LCID);
        cmd.Parameters.AddWithValue("@parentId", branch.ParentId);
    },
    trns));
                        AddOrUpdateObjectData(branch, trns);
                        FixBranchesSort(branch.ParentId, trns);
                    }
                    else
                    {
                        DBHelper.Database.ExecuteNonQuery(
        @"update 
            `cms_branch`
        set
            `lcid` = @lcid,
            `alias` = @alias,
            `parentId` = @parentId
        where
            id = @objId
",
    (cmd) =>
    {
        cmd.Parameters.AddWithValue("@alias", branch.Alias);
        cmd.Parameters.AddWithValue("@lcid", branch.LCID);
        cmd.Parameters.AddWithValue("@parentId", branch.ParentId);
        cmd.Parameters.AddWithValue("@objId", branch.ObjId);
    },
    trns);

                        AddOrUpdateObjectData(branch, trns);
                    }



                    trns.Commit();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    trns.Rollback();
                    throw;
                }
            }

            return branch;
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

        public List<CmsBranch> GetBranches()
        {
            var table = DBHelper.Database.ExecuteDataTable($@"SELECT 
    *
FROM
	cms_vw_branch
");

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

                results.Add(item);
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
