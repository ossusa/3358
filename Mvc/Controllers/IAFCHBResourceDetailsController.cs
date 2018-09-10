using ServiceStack.Logging;
using SitefinityWebApp.Custom.IAFCHandBook;
using SitefinityWebApp.Mvc.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Mvc.ActionFilters;

namespace SitefinityWebApp.Mvc.Controllers
{
	[ControllerToolboxItem(Name = "IAFCHBResourceDetails", Title = "Resource Details", SectionName = "Hand Book MVC Widgets")]
	public class IAFCHBResourceDetailsController:Controller
	{
		private const string commentResource = "Comment";
		private const string resourceResource = "Resource";

		[Category("General")]
		public Guid ResourceID { get; set; }
		private IAFCHandBookHelper handBookHelper;

		public IAFCHBResourceDetailsController()
		{
			var url = System.Web.HttpContext.Current.Request.Url.Host;
			handBookHelper = new IAFCHandBookHelper(url);
		}

		public IAFCHandBookResourceModel GetData(string name)
		{
			return handBookHelper.GetResourceDetails(name);
		}

		[RelativeRoute("{name},{categoryName?}")]
		public ActionResult GetResourceDetails(string name, string categoryName)
		{
			
			var model = handBookHelper.GetResourceDetailsUI(name, categoryName);
			if (model== null)
			{
				return Redirect(handBookHelper.PageNotFoundUrl());
			}
			return View("ResourceDetails", model);			
		}

		[RelativeRoute("{name?}")]
		public ActionResult GetResourceDetails(string name)
		{
			var model = GetData(name);
			if (model == null)
			{
				return Redirect(handBookHelper.PageNotFoundUrl());
			}
			return View("ResourceDetails", model);
		}

		[RelativeRoute("AddLike"), HttpPost]
		public ActionResult AddLike(String resourceId, bool isAdding)
		{
			var id = Guid.Parse(resourceId);
			var likes = AddLikeForResource(id, resourceResource, isAdding).ToString();

			return Json(likes);
		}


		[RelativeRoute("AddDislike"), HttpPost]
		public ActionResult AddDislike(String resourceId, bool isAdding)
		{
			var id = Guid.Parse(resourceId);
			var dislikes = AddDislikeForResource(id, resourceResource, isAdding).ToString();

			return Json(dislikes);
		}

		[RelativeRoute("AddToMyHandBook"), HttpPost]
		public ActionResult AddToMyHandBook(String resourceId)
		{
			var id = Guid.Parse(resourceId);
			handBookHelper.AddToMyHandBook(id);
			Boolean addToMyHandBook = true;

			return Json(addToMyHandBook);
		}

		[RelativeRoute("AddComment"), HttpPost]
		[StandaloneResponseFilter]
		public ActionResult AddComment(String commentTxt, String resourceId)
		{
			var id = Guid.Parse(resourceId);
			string CommentTxt = commentTxt;

			handBookHelper.CreateNewCommentForResource(id, commentTxt);
			var model = handBookHelper.GetResourceComments(id);
			return PartialView("_IAFCHBComments", model);						
		}
	

		[RelativeRoute("AddCommentLike"), HttpPost]
		public ActionResult AddCommentLike(String resourceId, bool isAdding)
		{
			var id = Guid.Parse(resourceId);
			var likes = AddLikeForResource(id, commentResource, isAdding).ToString();

			return Json(likes);
		}


		[RelativeRoute("AddCommentDislike"), HttpPost]
		public ActionResult AddCommentDislike(String resourceId, bool isAdding)
		{
			var id = Guid.Parse(resourceId);
			var dislikes = AddDislikeForResource(id, commentResource, isAdding).ToString();

			return Json(dislikes);
		}

		[RelativeRoute("PressReplyCommentBtn"), HttpPost]
		[StandaloneResponseFilter]
		public ActionResult PressReplyCommentBtn(String commentId)
		{
			var id = Guid.Parse(commentId);
			return PartialView("_IAFCHBReplyCommentsInput", id);
		}

		
		[RelativeRoute("AddReplyComment"), HttpPost]
		[StandaloneResponseFilter]
		public ActionResult AddReplyComment(String commentTxt, String commentId)
		{
			var id = Guid.Parse(commentId);
			handBookHelper.CreateNewCommentForResource(id, commentTxt, commentResource);
			var model = handBookHelper.GetResourceComments(id, commentResource);
			return PartialView("_IAFCHBComments", model);

		}
		#region Likes
		public int AddLikeForResource(Guid resourceID, string resourceType, bool isAdding)
		{
			var currentLikes = handBookHelper.AddLikeForResource(resourceID, resourceType, isAdding);
			return currentLikes;
		}

		public int AddDislikeForResource(Guid resourceID, string resourceType, bool isAdding)
		{
			var currentDislikes = handBookHelper.AddDislikeForResource(resourceID, resourceType, isAdding);
			return currentDislikes;
		}
		#endregion AddLikes




	}
}