using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;

namespace SitefinityWebApp.Mvc.Controllers
{
	[ControllerToolboxItem(Name = "IAFCHBSearchController", Title = "Search", SectionName = "Hand Book MVC Widgets")]
	public class IAFCHBSearchController : Controller
	{
		public ActionResult Index(String userid)
		{
			return View("Search");
		}

		public ActionResult Search(String searchStr)
		{
			string str = searchStr;
			return Json(str);
		}
	}	
}