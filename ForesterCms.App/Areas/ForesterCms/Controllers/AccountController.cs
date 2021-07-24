using Common.Utils.Logging;
using ForesterCmsServices.UI.Cms.Base;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForesterCms.App.Areas.ForesterCms.Controllers
{
    [Area("ForesterCms")]
    public class AccountController : BaseCmsController
    {
        public IActionResult Login()
        {


            return View();
        }
    }
}
