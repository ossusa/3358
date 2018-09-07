using ServiceStack.Logging;
using SitefinityWebApp.Custom.IAFCHandBook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;

namespace SitefinityWebApp.Mvc.Controllers
{
	[ControllerToolboxItem(Name = "IAFCHBRederict", Title = "Rederict to not found", SectionName = "Hand Book MVC Widgets")]
	public class IAFCHBRederictController : Controller
	{
		private ILog log = LogManager.GetLogger(typeof(IAFCHBResourcesPerCategoryController));
		private IAFCHandBookHelper handBookHelper;
		public IAFCHBRederictController()
		{
			var url = System.Web.HttpContext.Current.Request.Url.Host;
			handBookHelper = new IAFCHandBookHelper(url);
		}

		

		[RelativeRoute("{url?}")]
		public ActionResult Index(string url)
		{
			
			if (url != null)
			{
				return Redirect(handBookHelper.PageNotFoundUrl());
			}
			return null;
		}
	}
}