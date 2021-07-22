using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.Objects.Core
{
    public class BaseCmsEntity : ICmsEntity
    {
        public BaseCmsEntity() { }

        public BaseCmsEntity(BaseCmsEntity obj)
        {
            ObjId = obj.ObjId;
            EntityInfoId = obj.EntityInfoId;
            LCID = obj.LCID;
            Name = obj.Name;
            UpdateDate = obj.UpdateDate;
            CreateDate = obj.CreateDate;
            BranchId = obj.BranchId;
            Sort = obj.Sort;
        }

        public int ObjId { set; get; }
        public int EntityInfoId { set; get; }
        public int LCID { set; get; }
        public string Name { set; get; }
        public DateTime UpdateDate { set; get; }
        public DateTime CreateDate { set; get; }
        public int BranchId { get; set; }
        public int? Sort { get; set; }
        public MetaObject Meta { get; set; }

        public class MetaObject
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string Keywords { get; set; }
            public string Robots { get; set; }
        }
    }

    public interface ICmsEntity
    {
        int EntityInfoId { get; set; }
        int ObjId { get; set; }
        int LCID { get; set; }
    }
}
