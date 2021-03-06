﻿using SitefinityWebApp.Mvc.Models;
using Telerik.Sitefinity.Mvc;
using System.Web.Mvc;
using SitefinityWebApp.Custom.IAFCHandBook;

namespace SitefinityWebApp.Mvc.Controllers
{
	[ControllerToolboxItem(Name = "IAFCHBFeaturedResources", Title = "Featured Resources", SectionName = "Hand Book MVC Widgets")]
	public class IAFCHBFeaturedResourcesController: Controller
	{
		private IAFCHandBookHelper handBookHelper;

		public IAFCHBFeaturedResourcesController()
		{
			var url = System.Web.HttpContext.Current.Request.Url.Host;
			handBookHelper = new IAFCHandBookHelper(url);
		}

		public IAFCHandBookResourceModel GetData()
		{
			var model = handBookHelper.GetFeaturedResourcesData();
			return model;
		}


		// GET: FeaturedResources
		public ActionResult Index()
		{
			var model = GetData();
			return View("FeaturedResources", model);
		}
	}
}