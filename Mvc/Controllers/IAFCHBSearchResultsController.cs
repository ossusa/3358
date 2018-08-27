using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;

namespace SitefinityWebApp.Mvc.Controllers
{
	[ControllerToolboxItem(Name = "IAFCHBSearchResultsController", Title = "Search Results", SectionName = "Hand Book MVC Widgets")]
	public class IAFCHBSearchResultsController : Controller
	{

		[RelativeRoute("{searchTxt}")]
		public ActionResult Index(String searchTxt)
		{
			var txt = searchTxt;
			return View("SearchResults");
		}

		
	}
}