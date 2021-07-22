using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.Objects.Core
{
    public class LanguageResource : BaseCmsEntity
    {
        public string Key { set; get; }
        public string Text { get; set; }
        public CmsImage Image { get; set; }
    }
}
