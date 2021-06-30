using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.Objects.Cache
{
    public class CacheRefreshEventArgs : EventArgs
    {
        public string EntityInfoName { get; set; }

        public int? BranchId { get; set; }

        public int? EntityInfoId { get; set; }

        public int? ObjId { get; set; }

        public int? LCID { get; set; }

        public int ReTryCounter { get; set; }
    }
}
