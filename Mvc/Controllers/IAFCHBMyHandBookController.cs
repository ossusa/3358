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
	public class IAFCHBMyHandBookController:Controller
	{
		private IAFCHandBookHelper handBookHelper = new IAFCHandBookHelper();
		public IAFCHandBookMyHandBookModel GetData()
		{
			IAFCHandBookMyHandBookModel model = handBookHelper.GetMyHandBook();
			return model;
		}
		public ActionResult Index()
		{
			IAFCHandBookMyHandBookModel model = GetData();

			return View("MyHandBook",model);
		}
	}
}