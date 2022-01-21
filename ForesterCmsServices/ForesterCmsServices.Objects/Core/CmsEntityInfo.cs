using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.Objects.Core
{
    public class CmsEntityInfo
    {
        public string Alias { get; set; }
        public bool IsMultiLanguage { get; set; }
        public string Properties { get; set; }
        public bool IsSystem { get; set; }
        public string Name { get; set; }
        public int ObjId { get; set; }
    }
}
