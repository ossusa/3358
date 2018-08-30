using ServiceStack.Logging;
using SitefinityWebApp.Custom.IAFCHandBook;
using SitefinityWebApp.Mvc.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Mvc.ActionFilters;

namespace SitefinityWebApp.Mvc.Controllers
{
	[ControllerToolboxItem(Name = "IAFCHBResourcesPerCategory", Title = "Resources Per Category", SectionName = "Hand Book MVC Widgets")]
	public class IAFCHBResourcesPerCategoryController : Controller
    {
		[Category("General")]
		public String CategoryName { get; set; }

		private ILog log = LogManager.GetLogger(typeof(IAFCHBResourcesPerCategoryController));
		

		public IAFCHBResourcesPerCategoryController()
		{
						
		}

		private IAFCHandBookHelper handBookHelper = new IAFCHandBookHelper();

		public IAFCHandBookResourcesPerCatergoryModel GetData(String orderBy)
		{			
			return handBookHelper.GetResourcesPerCategory(CategoryName, orderBy);			
		}

				
		[RelativeRoute("{orderby?}")]
		public ActionResult GetOrderedResources(string orderby)
		{
			string orderByItem = "MostPopular";
			if (orderby !=null &&(
				orderby == "MostPopular" ||
				orderby == "MostRecent" ||
				orderby == "AlphabeticalAZ" ||
				orderby == "AlphabeticalZA"))
			{

				orderByItem = orderby;
			}
			
			var model = GetData(orderByItem);
			var view = View("ResourcesPerCategory", model);
			return view;
		}




		[RelativeRoute("AddLike"), HttpPost]
		public ActionResult AddLike(String resourceId)
		{
			var id = Guid.Parse(resourceId);
			var likes = AddLikeForResource(id).ToString();

			return Json(likes);
		}


		[RelativeRoute("AddDislike"), HttpPost]
		public ActionResult AddDislike(String resourceId)
		{
			var id = Guid.Parse(resourceId);
			var dislikes = AddDislikeForResource(id).ToString();

			return Json(dislikes);
		}

		[RelativeRoute("AddToMyHandBook"), HttpPost]
		public ActionResult AddToMyHandBook(String resourceId)
		{

			var id = Guid.Parse(resourceId);

			var addToMyHandBook = handBookHelper.AddToMyHandBook(id);

			return Json(addToMyHandBook);
		}

		[RelativeRoute("AddAllToMyHandBook"), HttpPost, StandaloneResponseFilter]
		public ActionResult AddAllToMyHandBook(String categoryId, string orderby)
		{
			var id = Guid.Parse(categoryId);
			handBookHelper.AddAllToMyHandBook( id);

			string orderByItem = "MostPopular";
			if (orderby != null && (
				orderby == "MostPopular" ||
				orderby == "MostRecent" ||
				orderby == "AlphabeticalAZ" ||
				orderby == "AlphabeticalZA"))
			{

				orderByItem = orderby;
			}

			var model = GetData(orderByItem);
			return PartialView("_ResourcesPerCategoryDetails", model.Resources);
			
		}
	

		[RelativeRoute("FollowCategory"), HttpPost]
		public ActionResult FollowCategory(String categoryId)
		{
			var id = Guid.Parse(categoryId);
			var followCategory = handBookHelper.FollowCategory(id);			
			return Json(followCategory);
		}

		#region Likes
		public int AddLikeForResource(Guid resourceID)
		{
			var currentLikes = handBookHelper.AddLikeForResource(resourceID, "Resource");
			return currentLikes;
		}

		public int AddDislikeForResource(Guid resourceID)
		{
			var currentDislikes = handBookHelper.AddDislikeForResource(resourceID, "Resource");
			return currentDislikes;
		}
		#endregion AddLikes
	}
}