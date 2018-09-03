using SitefinityWebApp.Custom.IAFCHandBook;
using SitefinityWebApp.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;

namespace SitefinityWebApp.Mvc.Controllers
{
	[ControllerToolboxItem(Name = "IAFCHBMyHandBook", Title = "My HandBook", SectionName = "Hand Book MVC Widgets")]
	public class IAFCHBMyHandBookController : Controller
	{
		private IAFCHandBookHelper handBookHelper;
		public IAFCHBMyHandBookController()
		{
			var url = System.Web.HttpContext.Current.Request.Url.Host;
			handBookHelper = new IAFCHandBookHelper(url);
		}
	
		public IAFCHandBookMyHandBookModel GetData()
		{
			IAFCHandBookMyHandBookModel model = handBookHelper.GetMyHandBook();
			return model;
		}

		public IAFCHandBookMyHandBookModel GetSharedHandBook(String userId)
		{
			IAFCHandBookMyHandBookModel model = handBookHelper.GetMyHandBook(userId);
			return model;
		}

		[RelativeRoute("{userid?}")]
		public ActionResult Index(String userid)
		{
			IAFCHandBookMyHandBookModel model = new IAFCHandBookMyHandBookModel();
			if (userid == null)
			{
				model = GetData();
			}
			else
			{
				model = GetSharedHandBook(userid);
			}


			return View("MyHandBook",model);
		}

		[RelativeRoute("Share"), HttpPost]
		public ActionResult Share(string url)
		{
			String sharedUrl = handBookHelper.GenerateSharedUrl(url);			
			return Json(sharedUrl);
		}
	}
}