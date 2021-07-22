using Common.Utils.Standard;
using ForesterCms.App.Infrastructure;
using ForesterCmsServices.UI;
using ForesterCmsServices.UI.Resources;
using ForesterCmsServices.UI.Routing;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForesterCms.App.Controllers
{
    public class GeneralController : Controller
    {
        public IActionResult Error404()
        {
            if (Request.Path.Value.Length == 1)
                return View();

            if (Config.Environment == EnvironmentType.Local)
                return NotFound();

            return Redirect($"/DigitalSite/ErrorPages/error404.htm?aspxerrorpath={Request.RelativeUrl()}");
        }

        public IActionResult ListPh(string fileName)
        {
            var routeData = RouterData.GetRouteParams(fileName);

            return RedirectToAction(routeData.Action, routeData.Controller, new { listph = "1" });
        }

        public IActionResult Preview(int branchId, int lcid, int entityInfoId, int objId)
        {
            string url = Router.GetUrl(branchId, lcid, entityInfoId, objId);
            if (url == null)
                return Json("url not found");

            CookiesManager.IsPreview = true;

            return Redirect("~/" + url);
        }

        public IActionResult Webpack(string type)
        {
            if (Config.Environment != EnvironmentType.Local)
                return Json(false);

            switch (type?.ToLower() ?? "")
            {
                case "stop":
                    WebpackHelper.Instance.Stop();
                    break;
                case "start":
                    WebpackHelper.Instance.StartWatch();
                    break;
                default:
                    return Json(false);
            }

            return Json(true);
        }
    }
}
