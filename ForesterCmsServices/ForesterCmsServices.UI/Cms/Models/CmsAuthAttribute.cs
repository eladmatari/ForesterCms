using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.Cms.Models
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class CmsAuthAttribute : Attribute
    {
        public CmsAuthType Type { get; set; }

        public CmsAuthAttribute(CmsAuthType type)
        {
            Type = type;
        }

        public CmsAuthAttribute()
        {

        }
    }
}
