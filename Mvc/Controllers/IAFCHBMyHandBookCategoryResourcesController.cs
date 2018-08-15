using ServiceStack.Logging;
using SitefinityWebApp.Custom.IAFCHandBook;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using SitefinityWebApp.Mvc.Models;

namespace SitefinityWebApp.Mvc.Controllers
{
	[ControllerToolboxItem(Name = "IAFCHBMyHandBookCategoryResources", Title = "My HandBook Category Resources", SectionName = "Hand Book MVC Widgets")]
	public class IAFCHBMyHandBookCategoryResourcesController:Controller
	{
		[Category("General")]
		public String CategoryName { get; set; }

		public IAFCHBMyHandBookCategoryResourcesController()
		{

		}

		private ILog log = LogManager.GetLogger(typeof(IAFCHBMyHandBookCategoryResourcesController));
		private IAFCHandBookHelper handBookHelper = new IAFCHandBookHelper();


		


		public ActionResult Index()
		{
			IAFCHandBookMyHandBookResourceModelModel model = new IAFCHandBookMyHandBookResourceModelModel();
			return View("MyHandBookCategoryResources");			
		}




		

	}
}