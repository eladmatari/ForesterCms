using ForesterCmsServices.Cache;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForesterCms.App.Areas.ForesterCms.Controllers.Api
{
    public class CoreApiController : Controller
    {
        public IActionResult Index()
        {
            var branches = CacheManager.Branches.Items;

            return Json(branches);
        }
    }
}
