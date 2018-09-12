using SitefinityWebApp.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using SitefinityWebApp.Custom.IAFCHandBook;

namespace SitefinityWebApp.Mvc.Controllers
{

	[ControllerToolboxItem(Name = "IAFCHBRecentlyAddedResources", Title = "Recently Added Resources", SectionName = "Hand Book MVC Widgets")]
	public class IAFCHBRecentlyAddedResourcesController : Controller
    {
		
		private IAFCHandBookHelper handBookHelper;

		public IAFCHBRecentlyAddedResourcesController()
		{
			var url = System.Web.HttpContext.Current.Request.Url.Host;
			handBookHelper = new IAFCHandBookHelper(url);
		}
	
		public List<IAFCHandBookResourceModel> GetData()
		{

			var model = handBookHelper.GetRecentlyAddedResources();
			return model;
		}


		public ActionResult Index()
        {
			var model = GetData();
			return View("RecentlyAddedResources", model);
        }

		[HttpPost]
		public ActionResult AddLike(String resourceId, int likeAddAmount, int dislikeAddAmount)
		{
			var id = Guid.Parse(resourceId);			
			var likes = handBookHelper.AddLikeForResourceUI(id, "Resource", likeAddAmount, dislikeAddAmount);

			return Json(likes);
		}


		[HttpPost]
		public ActionResult AddDislike(String resourceId, int likeAddAmount, int dislikeAddAmount)
		{
			var id = Guid.Parse(resourceId);
			var likes = handBookHelper.AddLikeForResourceUI(id, "Resource", likeAddAmount, dislikeAddAmount).ToString();

			return Json(likes);
		}

		[HttpPost]
		public ActionResult AddToMyHandBook(string comment, String resourceId)
		{
			var id = Guid.Parse(resourceId);
			Boolean addToMyHandBook = handBookHelper.AddToMyHandBook(id);

			return Json(addToMyHandBook);
		}

		
	}
}