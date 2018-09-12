using ServiceStack.Logging;
using SitefinityWebApp.Custom.IAFCHandBook;
using SitefinityWebApp.Mvc.Models;
using System;
using System.ComponentModel;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Mvc.ActionFilters;

namespace SitefinityWebApp.Mvc.Controllers
{
	[ControllerToolboxItem(Name = "IAFCHBMyHandBookAllResourcesPerCategory", Title = "My HandBook All Resources Per Category", SectionName = "Hand Book MVC Widgets")]
	public class IAFCHBMyHandBookAllResourcesPerCategoryController: Controller
	{
		[Category("General")]
		public String CategoryName { get; set; }

		private ILog log = LogManager.GetLogger(typeof(IAFCHBResourcesPerCategoryController));
		private IAFCHandBookHelper handBookHelper;

		public IAFCHBMyHandBookAllResourcesPerCategoryController()
		{
			var url = System.Web.HttpContext.Current.Request.Url.Host;
			handBookHelper = new IAFCHandBookHelper(url);
		}
		
		public IAFCHandBookMyHandBookModel GetData(String categoryName, String userId)
		{
			return handBookHelper.GetMyHandBookResourcesPerCategory(categoryName, userId);
		}


		[RelativeRoute("{userid?}")]
		public ActionResult Index(String userid)
		{
			var model = GetData(CategoryName, userid);
			if (model == null)
			{
				return Redirect(handBookHelper.PageNotFoundUrl());
			}

			var view = View("MyHandBookAllResourcesPerCategory", model);
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

		[RelativeRoute("MarkAsComplete"), HttpPost, StandaloneResponseFilter]
		public ActionResult MarkAsComplete(String resourceId, String categoryId, String userId)
		{
			var id = Guid.Parse(resourceId);
			var categoryGuid = Guid.Parse(categoryId);
			var model = new IAFCHandBookMyHandBookResourceModelModel();
			var markAsComplete = handBookHelper.MarkAsComplete(id);
			model = handBookHelper.GetCategoryResources(categoryGuid, false, userId);

			var view = PartialView("_MyHandBookAllResourcesPerCategoryDetails", model);
			return view;
		}

		[RelativeRoute("Remove"), HttpPost, StandaloneResponseFilter]
		public ActionResult Remove(String resourceId, String categoryId, String userId)
		{
			var id = Guid.Parse(resourceId);
			var categoryGuid = Guid.Parse(categoryId);
			var model = new IAFCHandBookMyHandBookResourceModelModel();
			var markAsComplete = handBookHelper.RemoveResource(id);
			model = handBookHelper.GetCategoryResources(categoryGuid, false, userId);

			var view = PartialView("_MyHandBookAllResourcesPerCategoryDetails", model);
			return view;
		}

		[RelativeRoute("AddSharedToMyHandBook"), HttpPost]
		public ActionResult AddSharedToMyHandBook(String resourceId)
		{
			var id = Guid.Parse(resourceId);
			var addedToMyHandBool = handBookHelper.AddToMyHandBook(id);

			return Json(addedToMyHandBool);
		}

		[RelativeRoute("Edit"), HttpPost]
		public ActionResult Edit(String operation)
		{
			Boolean edit = true;
			if (operation.Equals("Cancel"))
			{
				edit = false;
			}
			return Json(edit);
		}

		[RelativeRoute("Share"), HttpPost]
		public ActionResult Share(String url)
		{
			String sharedUrl = handBookHelper.GenerateSharedUrl(url);
			return Json(sharedUrl);
		}
	
	}
}