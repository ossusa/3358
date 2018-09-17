using SitefinityWebApp.Custom.IAFCHandBook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;

namespace SitefinityWebApp.Mvc.Controllers
{
	[ControllerToolboxItem(Name = "VWSARITResourcesList", Title = "VWSARITResources", SectionName = "Hand Book MVC Widgets")]
	public class VWSARITResourcesListController : Controller
	{
		private IAFCHandBookHelper handBookHelper;
		public VWSARITResourcesListController()
		{
			var url = System.Web.HttpContext.Current.Request.Url.Host;
			handBookHelper = new IAFCHandBookHelper(url);
		}

		public ActionResult Index(String userid)
		{
			var model = handBookHelper.GetVWSARITResources();
			return View("Index", model);
		}
	}
}