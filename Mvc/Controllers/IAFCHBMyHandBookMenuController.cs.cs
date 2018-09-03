using SitefinityWebApp.Custom.IAFCHandBook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;

namespace SitefinityWebApp.Mvc.Controllers
{
	[ControllerToolboxItem(Name = "IAFCHBMyHandBookMenu", Title = "My HandBook Menu", SectionName = "Hand Book MVC Widgets")]
	public class IAFCHBMyHandBookMenuController : Controller
	{
		private IAFCHandBookHelper handBookHelper;
		public IAFCHBMyHandBookMenuController()
		{
			var url = System.Web.HttpContext.Current.Request.Url.Host;
			handBookHelper = new IAFCHandBookHelper(url);
		}
		

		[RelativeRoute("{relativeUrl?}")]
		public ActionResult Index()
		{
			var url = System.Web.HttpContext.Current.Request.Url;
			var urlPath = url.AbsoluteUri;
			var model = handBookHelper.GetMenu(urlPath);
			return View("MyHandBookMenu",model);
		}
	}
}