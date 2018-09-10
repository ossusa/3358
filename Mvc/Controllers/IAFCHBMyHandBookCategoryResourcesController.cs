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
using Telerik.Sitefinity.Mvc.ActionFilters;

namespace SitefinityWebApp.Mvc.Controllers
{
	[ControllerToolboxItem(Name = "IAFCHBMyHandBookCategoryResources", Title = "My HandBook Category Resources", SectionName = "Hand Book MVC Widgets")]
	public class IAFCHBMyHandBookCategoryResourcesController : Controller
	{
		[Category("General")]
		public String CategoryName { get; set; }

		public IAFCHBMyHandBookCategoryResourcesController()
		{
			var url = System.Web.HttpContext.Current.Request.Url.Host;
			handBookHelper = new IAFCHandBookHelper(url);
		}

		private ILog log = LogManager.GetLogger(typeof(IAFCHBMyHandBookCategoryResourcesController));
		private IAFCHandBookHelper handBookHelper;

		[RelativeRoute("{userid?}")]
		public ActionResult Index(String userId)
		{
			IAFCHandBookMyHandBookResourceModelModel model = new IAFCHandBookMyHandBookResourceModelModel();
			model = handBookHelper.GetMyHandBookCategoryResourcesByName(CategoryName, userId);

			if (model == null)
			{
				return Redirect(handBookHelper.PageNotFoundUrl());
			}

			return View("MyHandBookCategoryResources", model);
		}
		

		[RelativeRoute("GetResources"), HttpPost, StandaloneResponseFilter]
		public ActionResult GetResources(String categoryId)
		{

			var categoryGuid = Guid.Parse(categoryId);
			var model = handBookHelper.GetMyHandBookCategoryResourcesList(categoryGuid);


			var view = PartialView("_MyHandBookCategoryResourcesDetails", model);
			return view;
		}

		[RelativeRoute("AddLike"), HttpPost]
		public ActionResult AddLike(String resourceId, bool isAdding)
		{
			var id = Guid.Parse(resourceId);
			var likes = AddLikeForResource(id, isAdding).ToString();

			return Json(likes);
		}
		[RelativeRoute("AddDislike"), HttpPost]
		public ActionResult AddDislike(String resourceId, bool isAdding)
		{
			var id = Guid.Parse(resourceId);
			var dislikes = AddDislikeForResource(id, isAdding).ToString();

			return Json(dislikes);
		}

		[RelativeRoute("MarkAsComplete"), HttpPost, StandaloneResponseFilter]
		public ActionResult MarkAsComplete(String resourceId, String categoryId)
		{
			var id = Guid.Parse(resourceId);
			var categoryGuid = Guid.Parse(categoryId);
			var model = new IAFCHandBookMyHandBookResourceModelModel();			
			var markAsComplete = handBookHelper.MarkAsComplete(id);
			model = handBookHelper.GetCategoryResources(categoryGuid, true, null);
			
			var view = PartialView("_MyHandBookCategoryResourcesDetails", model);
			return view;
		}

		[RelativeRoute("Remove"), HttpPost, StandaloneResponseFilter]
		public ActionResult Remove(String resourceId, String categoryId)
		{
			var id = Guid.Parse(resourceId);
			var categoryGuid = Guid.Parse(categoryId);
			var model = new IAFCHandBookMyHandBookResourceModelModel();
			var markAsComplete = handBookHelper.RemoveResource(id);
			model = handBookHelper.GetCategoryResources(categoryGuid, true, null);			
			var view = PartialView("_MyHandBookCategoryResourcesDetails", model);
			return view;
		}

		
		[RelativeRoute("AddSharedToMyHandBook"), HttpPost]
		public ActionResult AddSharedToMyHandBook(String resourceId)
		{
			var id = Guid.Parse(resourceId);						
			var addedToMyHandBool = handBookHelper.AddToMyHandBook(id);
						
			return Json(addedToMyHandBool);
		}

		[RelativeRoute("RemoveCompleted"), HttpPost, StandaloneResponseFilter]
		public ActionResult RemoveCompleted(String resourceId, String categoryId, String userId)
		{
			var id = Guid.Parse(resourceId);
			var categoryGuid = Guid.Parse(categoryId);
			var model = new IAFCHandBookMyHandBookResourceModelModel();			
			var markAsComplete = handBookHelper.RemoveResource(id, "MyCompletedResources");
			
			model = handBookHelper.GetCategoryResources(categoryGuid, true, null);
			var view = PartialView("_MyHandBookCategoryResourcesDetails", model);
			return view;
		}

		[RelativeRoute("OrderBy"), HttpPost, StandaloneResponseFilter]
		public ActionResult OrderBy(String orderBy, String categoryId, String userId, String sharedUserID)
		{

			var categoryGuid = Guid.Parse(categoryId);
			var model = new IAFCHandBookMyHandBookResourceModelModel();
			if (sharedUserID == Guid.Empty.ToString())
			{
				model = handBookHelper.GetCategoryResources(categoryGuid, true, null, orderBy);
			}
			else
			{
				model = handBookHelper.GetCategoryResources(categoryGuid, true, sharedUserID, orderBy);
			}
			var view = PartialView("_MyHandBookCategoryResourcesDetails", model);
			return view;
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


		#region Likes
		public int AddLikeForResource(Guid resourceID, bool isAdding)
		{
			var currentLikes = handBookHelper.AddLikeForResource(resourceID, "Resource", isAdding);
			return currentLikes;
		}

		public int AddDislikeForResource(Guid resourceID, bool isAdding)
		{
			var currentDislikes = handBookHelper.AddDislikeForResource(resourceID, "Resource", isAdding);
			return currentDislikes;
		}
		#endregion Likes
	}
}
