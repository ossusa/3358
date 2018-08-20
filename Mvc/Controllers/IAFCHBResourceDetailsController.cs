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
		private IAFCHandBookHelper handBookHelper = new IAFCHandBookHelper();

			

		public IAFCHandBookResourceModel GetData(string name)
		{

			return handBookHelper.GetResourceDetails(name);

		}

		[RelativeRoute("{name}")]
		public ActionResult GetResourceDetails(string name)
		{			
			var model = GetData(name);
			return View("ResourceDetails", model);			
		}

		[RelativeRoute("AddLike"), HttpPost]
		public ActionResult AddLike(String resourceId)
		{
			var id = Guid.Parse(resourceId);
			var likes = AddLikeForResource(id, resourceResource).ToString();

			return Json(likes);
		}


		[RelativeRoute("AddDislike"), HttpPost]
		public ActionResult AddDislike(String resourceId)
		{
			var id = Guid.Parse(resourceId);
			var dislikes = AddDislikeForResource(id, resourceResource).ToString();

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
		public ActionResult AddCommentLike(String resourceId)
		{
			var id = Guid.Parse(resourceId);
			var likes = AddLikeForResource(id, commentResource).ToString();

			return Json(likes);
		}


		[RelativeRoute("AddCommentDislike"), HttpPost]
		public ActionResult AddCommentDislike(String resourceId)
		{
			var id = Guid.Parse(resourceId);
			var dislikes = AddDislikeForResource(id, commentResource).ToString();

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
		public int AddLikeForResource(Guid resourceID, string resourceType)
		{
			var currentLikes = handBookHelper.AddLikeForResource(resourceID, resourceType);
			return currentLikes;
		}

		public int AddDislikeForResource(Guid resourceID, string resourceType)
		{
			var currentDislikes = handBookHelper.AddDislikeForResource(resourceID, resourceType);
			return currentDislikes;
		}
		#endregion AddLikes




	}
}