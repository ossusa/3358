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
	[ControllerToolboxItem(Name = "IAFCHBResourcesPerCategory", Title = "Resources Per Category", SectionName = "Hand Book MVC Widgets")]
	public class IAFCHBResourcesPerCategoryController : Controller
    {
		[Category("General")]
		public String CategoryName { get; set; }


		private ILog log = LogManager.GetLogger(typeof(IAFCHBResourcesPerCategoryController));

		public ResourcesOrderBy OrderBy { get; set; }

		public IAFCHBResourcesPerCategoryController()
		{
			
			OrderBy = ResourcesOrderBy.MostPopular;
		}


		private IAFCHandBookHelper handBookHelper = new IAFCHandBookHelper();

		public IAFCHandBookResourcesPerCatergoryModel GetData(ResourcesOrderBy orderBy)
		{

			return handBookHelper.GetResourcesPerCategory(CategoryName, orderBy);
			
		}

		public ActionResult Index()
        {
			log.Info("Get Resouces Per Category Index Start");
			var model = GetData(OrderBy);
			var view = View("ResourcesPerCategory", model);
			log.Info("Get Resouces Per Category Index End");
			return view;
		}

	

		[HttpPost]	
		public ActionResult GetOrderedResources(int orderBy)
		{
			log.Info("Get Resouces Per Category Start");
			var newOrderBy = (ResourcesOrderBy)Enum.ToObject(typeof(ResourcesOrderBy), orderBy);
			var model = GetData(newOrderBy);
			log.Info("Get Resouces Per Category End");
			return View("_ResourcesPerCategoryDetails", model.Resources);
			

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

		[HttpPost]
		public ActionResult AddToMyHandBook(String resourceId)
		{
			var id = Guid.Parse(resourceId);
			Boolean addToMyHandBook = true;

			return Json(addToMyHandBook);
		}

		[HttpPost]
		public ActionResult AddAllToMyHandBook(String categoryId)
		{
			var id = Guid.Parse(categoryId);
			Boolean addAllToMyHandBook = true;
			return Json(addAllToMyHandBook);
		}

		[HttpPost]
		public ActionResult FollowCategory(String categoryId)
		{
			var id = Guid.Parse(categoryId);
			Boolean followCategory = true;
			return Json(followCategory);
		}

		#region Likes
		public int AddLikeForResource(Guid resourceID)
		{
			var currentLikes = handBookHelper.AddLikeForResource(resourceID);
			return currentLikes;
		}

		public int AddDislikeForResource(Guid resourceID)
		{
			var currentDislikes = handBookHelper.AddDislikeForResource(resourceID);
			return currentDislikes;
		}
		#endregion AddLikes
	}
}