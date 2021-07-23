using ForesterCmsServices.UI.Models;
using ForesterCmsServices.UI.Routing;
using ForesterCmsServices.UI.Routing.Cms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.Base.Cms
{
    public class BaseCmsController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
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
