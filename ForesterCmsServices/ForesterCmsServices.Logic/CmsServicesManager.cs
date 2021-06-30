using ForesterCmsServices.Logic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.Logic
{
    public static class CmsServicesManager
    {
        static CmsServicesManager()
        {
            Core = new Core();
        }
        public static Core Core { get; private set; }
    }
}
