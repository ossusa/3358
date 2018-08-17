using ServiceStack.Logging;
using SitefinityWebApp.Custom.IAFCHandBook;
using SitefinityWebApp.Mvc.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
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


		public IAFCHBMyHandBookAllResourcesPerCategoryController()
		{

		}

		private IAFCHandBookHelper handBookHelper = new IAFCHandBookHelper();

		public IAFCHandBookMyHandBookModel GetData(string categoryName)
		{

			return handBookHelper.GetMyHandBookResourcesPerCategory(categoryName);
		}


		
		public ActionResult Index()
		{
			var model = GetData(CategoryName);
			var view = View("MyHandBookAllResourcesPerCategory", model);
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
				model = handBookHelper.GetCategoryResources(categoryGuid, false, "Remove");
			}
			else
			{				
				var markAsComplete = handBookHelper.MarkAsComplete(id);
				model = handBookHelper.GetCategoryResources(categoryGuid, false);
			}
			
			var view =  PartialView("_MyHandBookAllResourcesPerCategoryDetails", model);			
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