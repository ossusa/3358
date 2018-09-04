using ServiceStack.Logging;
using SitefinityWebApp.Custom.IAFCHandBook;
using SitefinityWebApp.Mvc.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;

namespace SitefinityWebApp.Mvc.Controllers
{
	[ControllerToolboxItem(Name = "IAFCHBRCategoryResourcesList", Title = "Category Resources List", SectionName = "Hand Book MVC Widgets")]
	public class IAFCHBRCategoryResourcesListController: Controller
	{
		[Category("General")]
		public String CategoryName { get; set; }

		private ILog log = LogManager.GetLogger(typeof(IAFCHBResourcesPerCategoryController));
		private IAFCHandBookHelper handBookHelper;
		public IAFCHBRCategoryResourcesListController()
		{
			var url = System.Web.HttpContext.Current.Request.Url.Host;
			handBookHelper = new IAFCHandBookHelper(url);
		}

		public IAFCHandBookCategoryResourcesModel GetData()
		{
			var model = handBookHelper.CategoryListResources(CategoryName);
			return model;
		}

		
		public ActionResult Index()
		{
			var model = GetData();
			return  View("CategoryResourcesList", model);			
		}

	}
}