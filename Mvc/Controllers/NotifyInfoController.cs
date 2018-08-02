using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceStack;

namespace SitefinityWebApp.Mvc.Controllers
{
    public class NotifyInfoController : Controller
    {
        // GET: NotifyInfo
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WarningInfo()
        {
            var data = ViewData["warning"];
            return Content("Message:{0}".Fmt(data));
        }
    }
}