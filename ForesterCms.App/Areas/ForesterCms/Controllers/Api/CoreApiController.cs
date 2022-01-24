using ForesterCmsServices.Cache;
using ForesterCmsServices.Logic;
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
        public IActionResult AddOrUpdateBranch([FromBody] CmsBranch branch)
        {
            var ei = CacheManager.EntityInfos.GetItem(branch.EntityInfoId);
            if (ei?.Alias != "branch")
                throw new Exception($"Invalid EntityInfoId: {branch.EntityInfoId}");

            var lang = CacheManager.Languages.GetItem(branch.LCID);
            if (lang == null)
                throw new Exception($"Invalid LCID: {branch.LCID}");

            branch = CmsServicesManager.Core.AddOrUpdateBranch(branch);
            CacheManager.Branches.Refresh();

            return Json(branch);
        }
    }
}
