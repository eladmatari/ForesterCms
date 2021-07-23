using Common.Utils.Logging;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForesterCms.App.Areas.ForesterCms.Controllers
{
    [Area("ForesterCms")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            Logger.Info("Hello");

            return View();
        }
    }
}
