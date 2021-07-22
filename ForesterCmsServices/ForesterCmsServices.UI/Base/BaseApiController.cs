using Common.Utils.Standard;
using ForesterCmsServices.Cache;
using ForesterCmsServices.UI.Models;
using ForesterCmsServices.UI.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.Base
{
    public class BaseApiController : Controller
    {
        public RouterData RouterData { get; private set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var routerData = new RouterData();
            routerData.BranchId = RequestHelper.GetInt(Request.Query["boneid"].FirstOrDefault());
            routerData.Language = CacheManager.Languages.GetItem(RequestHelper.GetInt(Request.Query["lcid"].FirstOrDefault()));
            routerData.ObjId = RequestHelper.GetInt(Request.Query["objid"].FirstOrDefault());
            routerData.EntityInfoId = RequestHelper.GetInt(Request.Query["nsid"].FirstOrDefault());

            Router.Data = RouterData = routerData;

            base.OnActionExecuting(context);
        }

        protected DatasourceResult Datasource(string key, object value)
        {
            return new DatasourceResult(key, value);
        }
    }
}
