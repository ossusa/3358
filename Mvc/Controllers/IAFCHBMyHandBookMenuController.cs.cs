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
		private IAFCHandBookHelper handBookHelper = new IAFCHandBookHelper();

		public ActionResult Index(String userid)
		{
			var model = handBookHelper.GetMenu();
			return View("MyHandBookMenu",model);
		}
	}
}