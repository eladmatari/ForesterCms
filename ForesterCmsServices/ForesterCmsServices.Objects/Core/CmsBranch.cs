using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.Objects.Core
{
    public class CmsBranch : BaseCmsEntity
    {
        public string Alias { get; set; }
        public int? ParentId { get; set; }
        public string TreeAlias { get; set; }
    }
}
