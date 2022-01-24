using ForesterCmsServices.Cache;
using ForesterCmsServices.Objects.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForesterCms.App.Areas.ForesterCms.Controllers.Api
{
    public class CoreApiController : Controller
    {
        [HttpGet]
        public IActionResult Branches()
        {
            var branches = CacheManager.Branches.Items;

            return Json(branches);
        }

        [HttpPost]
        public IActionResult AddNewBranch([FromBody] CmsBranch branch)
        {
            return Json(branch);
        }
    }
}
