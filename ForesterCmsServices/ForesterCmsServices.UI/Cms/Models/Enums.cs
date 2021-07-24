using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.Cms.Models
{
    [Flags]
    public enum CmsAuthType
    {
        None = 0,
        View = 1,
        Edit = 2
    }
}
