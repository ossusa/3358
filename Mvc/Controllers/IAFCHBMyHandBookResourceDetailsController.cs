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
	[ControllerToolboxItem(Name = "IAFCHBMyHandBookResourceDetails", Title = "My HandBook Resource Details", SectionName = "Hand Book MVC Widgets")]
	public class IAFCHBMyHandBookResourceDetailsController: Controller
	{
		private IAFCHandBookHelper handBookHelper;

		public IAFCHBMyHandBookResourceDetailsController()
		{
			var url = System.Web.HttpContext.Current.Request.Url.Host;
			handBookHelper = new IAFCHandBookHelper(url);
		}

		private const string commentResource = "Comment";
		private const string resourceResource = "Resource";
		
		public IAFCHandBookResourceModel GetData(string name, string userId)
		{

			return handBookHelper.GetMyHnadbookResourceDetailsUI(name, null, userId);

		}
		[RelativeRoute("{name?}/{userid?}")]
		public ActionResult Index(String name, string userid)
		{
			var model = GetData(name, userid );
			if (model == null)
			{
				return Redirect(handBookHelper.PageNotFoundUrl());
			}
			return View("MyHandBookResourceDetails", model);
		}

		[RelativeRoute("{name},{categoryName?}/{userid?}")]
		public ActionResult Index(string name, string categoryName, string userid)
		{			
			var model = handBookHelper.GetMyHnadbookResourceDetailsUI(name, categoryName, userid);
			if (model == null)
			{
				return Redirect(handBookHelper.PageNotFoundUrl());
			}
			
			return View("MyHandBookResourceDetails", model);
		}

		[RelativeRoute("AddLike"), HttpPost]	
		public ActionResult AddLike(String resourceId, int likeAddAmount, int dislikeAddAmount)
		{
			var id = Guid.Parse(resourceId);

			var likes = handBookHelper.AddLikeForResourceUI(id, resourceResource, likeAddAmount, dislikeAddAmount);

			return Json(likes);
		}

		[RelativeRoute("AddDislike"), HttpPost]
		public ActionResult AddDislike(String resourceId, int likeAddAmount, int dislikeAddAmount)
		{
			var id = Guid.Parse(resourceId);
			var likes = handBookHelper.AddLikeForResourceUI(id, resourceResource, likeAddAmount, dislikeAddAmount).ToString();

			return Json(likes);
		}

		[RelativeRoute("MarkAsComplete"), HttpPost]
		public ActionResult MarkAsComplete(String resourceId)
		{
			var id = Guid.Parse(resourceId);
			handBookHelper.MarkAsComplete(id);
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
		public ActionResult AddCommentLike(String resourceId, int likeAddAmount, int dislikeAddAmount)
		{
			var id = Guid.Parse(resourceId);

			var likes = handBookHelper.AddLikeForResourceUI(id, commentResource, likeAddAmount, dislikeAddAmount);

			return Json(likes);
		}

		[RelativeRoute("AddCommentDislike"), HttpPost]		
		public ActionResult AddCommentDislike(String resourceId, int likeAddAmount, int dislikeAddAmount)
		{
			var id = Guid.Parse(resourceId);
			var likes = handBookHelper.AddLikeForResourceUI(id, commentResource, likeAddAmount, dislikeAddAmount).ToString();

			return Json(likes);
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

		[RelativeRoute("Share"), HttpPost]
		public ActionResult Share(String url)
		{
			String sharedUrl = handBookHelper.GenerateSharedUrl(url);
			return Json(sharedUrl);
		}

	}
}