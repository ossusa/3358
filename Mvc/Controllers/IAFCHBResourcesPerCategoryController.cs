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
		private IAFCHandBookHelper handBookHelper;

		public IAFCHBResourcesPerCategoryController()
		{
			var url = System.Web.HttpContext.Current.Request.Url.Host;
			handBookHelper = new IAFCHandBookHelper(url);

			
		}
		
		public IAFCHandBookResourcesPerCatergoryModel GetData(String orderBy)
		{			
			return handBookHelper.GetResourcesPerCategory(CategoryName, orderBy);			
		}

				
		[RelativeRoute("{orderby?}")]
		public ActionResult GetOrderedResources(string orderby)
		{
			string orderByItem = "Most Popular";
			if (orderby !=null &&(
				orderby == "Most Popular" ||
				orderby == "Most Recent" ||
				orderby == "Alphabetical AZ" ||
				orderby == "Alphabetical ZA"))
			{

				orderByItem = orderby;
			}
			
			var model = GetData(orderByItem);
			if (model == null)
			{
				return Redirect(handBookHelper.PageNotFoundUrl());
			}
			var view = View("ResourcesPerCategory", model);
			return view;
		}




		[RelativeRoute("AddLike"), HttpPost]		
		public ActionResult AddLike(String resourceId, int likeAddAmount, int dislikeAddAmount)
		{
			var id = Guid.Parse(resourceId);
			var likes = handBookHelper.AddLikeForResourceUI(id, "Resource", likeAddAmount, dislikeAddAmount);

			return Json(likes);
		}


		[RelativeRoute("AddDislike"), HttpPost]
		public ActionResult AddDislike(String resourceId, int likeAddAmount, int dislikeAddAmount)
		{
			var id = Guid.Parse(resourceId);
			var likes = handBookHelper.AddLikeForResourceUI(id, "Resource", likeAddAmount, dislikeAddAmount).ToString();

			return Json(likes);
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

			string orderByItem = "Most Popular";
			if (orderby != null && (
				orderby == "Most Popular" ||
				orderby == "Most Recent" ||
				orderby == "Alphabetical AZ" ||
				orderby == "Alphabetical ZA"))
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
		
	}
}