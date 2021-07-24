using Common.Utils;
using ForesterCmsServices.UI.Cms.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.Cms
{
    public static class CmsSessionManager
    {
        private const string SESSION_KEY_USER = "Cms.User";

        public static CmsUserSessionData User
        {
            get
            {
                //if (HttpContextHelper.Current.Session.GetData<CmsUserSessionData>(SESSION_KEY_USER) == null)
                //    HttpContextHelper.Current.Session.SetData(SESSION_KEY_USER, new CmsUserSessionData());

                return HttpContextHelper.Current.Session.GetData<CmsUserSessionData>(SESSION_KEY_USER);
            }
            set
            {
                HttpContextHelper.Current.Session.SetData(SESSION_KEY_USER, value);
            }
        }
    }
}
