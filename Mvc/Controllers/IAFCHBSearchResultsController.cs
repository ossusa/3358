using SitefinityWebApp.Custom.IAFCHandBook;
using SitefinityWebApp.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Mvc.ActionFilters;

namespace SitefinityWebApp.Mvc.Controllers
{
	[ControllerToolboxItem(Name = "IAFCHBSearchResultsController", Title = "Search Results", SectionName = "Hand Book MVC Widgets")]
	public class IAFCHBSearchResultsController : Controller
	{
		private IAFCHandBookHelper handBookHelper;

		public IAFCHBSearchResultsController()
		{
			var url = System.Web.HttpContext.Current.Request.Url.Host;
			handBookHelper = new IAFCHandBookHelper(url);
		}

		public IAFCHandBookSearchedResourcesModel GetData(string searchTxt, string orderBy)
		{
			return handBookHelper.GetSearchedResourcres(searchTxt, orderBy);
		}

		[RelativeRoute("{searchText}")]
		public ActionResult Index(String searchText)
		{
			var model = GetData(searchText, "Most Recent");
			return View("SearchResults", model);
		}

		[RelativeRoute("OrderBy"), HttpPost, StandaloneResponseFilter]
		public ActionResult OrderBy(String searchText, String orderBy )
		{
			var model = GetData(searchText, orderBy);
			return PartialView("_SearchResultsDetails", model);
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

		[RelativeRoute("AddToMyHandBook"), HttpPost, StandaloneResponseFilter]
		public ActionResult AddToMyHandBook(String resourceId, String searchText, String orderBy)
		{
			var id = Guid.Parse(resourceId);

			var addToMyHandBook = handBookHelper.AddToMyHandBook(id);

			var model = GetData(searchText, orderBy);
			return PartialView("_SearchResultsDetails", model);

			
		}

		[RelativeRoute("MarkAsComplete"), HttpPost, StandaloneResponseFilter]
		public ActionResult MarkAsComplete(String resourceId)
		{
			var id = Guid.Parse(resourceId);						
			var markAsComplete = handBookHelper.MarkAsComplete(id);
			return Json(markAsComplete);
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