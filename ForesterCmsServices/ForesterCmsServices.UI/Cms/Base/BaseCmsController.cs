using ForesterCmsServices.UI.Cms.Models;
using ForesterCmsServices.UI.Cms.Routing;
using ForesterCmsServices.UI.Models;
using ForesterCmsServices.UI.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.Cms.Base
{
    public class BaseCmsController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var cmsAuthAttr = ((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo
                .GetCustomAttributes(typeof(CmsAuthAttribute), false)
                .Select(i => (CmsAuthAttribute)i)
                .FirstOrDefault();

            if (cmsAuthAttr == null)
            {
                cmsAuthAttr = this.GetType().GetCustomAttributes(typeof(CmsAuthAttribute), false)
                .Select(i => (CmsAuthAttribute)i)
                .FirstOrDefault();
            }

            if (cmsAuthAttr != null)
            {
                if (CmsSessionManager.User == null)
                {
                    context.Result = Redirect("~/forestercms/account/login/");
                    return;
                }
            }

            CmsRouter.Data = new CmsRouterData()
            {
                PageModel = new CmsPageModel()
            };

            base.OnActionExecuting(context);
        }

        public CmsPageModel PageModel
        {
            get
            {
                return CmsRouter.Data.PageModel;
            }
        }
    }
}
