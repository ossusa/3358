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

		}

		private ILog log = LogManager.GetLogger(typeof(IAFCHBMyHandBookCategoryResourcesController));
		private IAFCHandBookHelper handBookHelper = new IAFCHandBookHelper();


		public ActionResult Index()
		{
			IAFCHandBookMyHandBookResourceModelModel model = new IAFCHandBookMyHandBookResourceModelModel();
			model = handBookHelper.GetMyHandBookCategoryResourcesByName(CategoryName);
			return View("MyHandBookCategoryResources", model);
		}


		[HttpPost, StandaloneResponseFilter]
		public ActionResult GetResources(String categoryId)
		{

			var categoryGuid = Guid.Parse(categoryId);

			var model = handBookHelper.GetMyHandBookCategoryResourcesList(categoryGuid);


			var view = PartialView("_MyHandBookCategoryResourcesDetails", model);
			return view;
		}

		[HttpPost]
		public ActionResult AddLike(String resourceId)
		{
			var id = Guid.Parse(resourceId);
			var likes = AddLikeForResource(id).ToString();

			return Json(likes);
		}
		[HttpPost]
		public ActionResult AddDislike(String resourceId)
		{
			var id = Guid.Parse(resourceId);
			var dislikes = AddDislikeForResource(id).ToString();

			return Json(dislikes);
		}

		[HttpPost, StandaloneResponseFilter]
		public ActionResult MarkAsComplete(String operation, String resourceId, String categoryId)
		{
			var id = Guid.Parse(resourceId);
			var categoryGuid = Guid.Parse(categoryId);
			var model = new IAFCHandBookMyHandBookResourceModelModel();
			if (operation.Equals("Remove"))
			{
				var markAsComplete = handBookHelper.RemoveResource(id);
				
			}
			else
			{
				var markAsComplete = handBookHelper.MarkAsComplete(id);
				
			}
			model = handBookHelper.GetCategoryResources(categoryGuid);
			var view = PartialView("_MyHandBookCategoryResourcesDetails", model);
			return view;
		}

		[HttpPost, StandaloneResponseFilter]
		public ActionResult RemoveCompleted(String resourceId, String categoryId)
		{
			var id = Guid.Parse(resourceId);
			var categoryGuid = Guid.Parse(categoryId);
			var model = new IAFCHandBookMyHandBookResourceModelModel();			
			var markAsComplete = handBookHelper.RemoveResource(id, "MyCompletedResources");
			
			model = handBookHelper.GetCategoryResources(categoryGuid);
			var view = PartialView("_MyHandBookCategoryResourcesDetails", model);
			return view;
		}

		[HttpPost, StandaloneResponseFilter]
		public ActionResult OrderBy(String orderBy, String categoryId)
		{
			
			var categoryGuid = Guid.Parse(categoryId);
			var model = new IAFCHandBookMyHandBookResourceModelModel();
			

			model = handBookHelper.GetCategoryResources(categoryGuid, "Marked as Commplete", orderBy);
			var view = PartialView("_MyHandBookCategoryResourcesDetails", model);
			return view;
		}


		[HttpPost]
		public ActionResult Edit(String operation)
		{
			Boolean edit = true;
			if (operation.Equals("Cancel"))
			{
				edit = false;
			}
			return Json(edit);
		}

		[HttpPost]
		public ActionResult Share()
		{
			Boolean share = true;
			return Json(share);
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
