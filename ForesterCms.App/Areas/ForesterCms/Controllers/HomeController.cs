using Common.Utils.Logging;
using ForesterCmsServices.UI.Base.Cms;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForesterCms.App.Areas.ForesterCms.Controllers
{
    [Area("ForesterCms")]
    public class HomeController : BaseCmsController
    {
        public IActionResult Index()
        {


            return View();
        }
    }
}
