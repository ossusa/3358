﻿using SitefinityWebApp.Custom.IAFCHandBook;
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
		
		public IAFCHandBookResourceModel GetData(string name)
		{

			return handBookHelper.GetMyHnadbookResourceDetailsUI(name);

		}
		[RelativeRoute("{name?}")]
		public ActionResult Index(String name)
		{
			var model = GetData(name);
			if (model == null)
			{
				return Redirect(handBookHelper.PageNotFoundUrl());
			}
			return View("MyHandBookResourceDetails", model);
		}

		[RelativeRoute("{name},{categoryName?}")]
		public ActionResult Index(string name, string categoryName)
		{			
			var model = handBookHelper.GetMyHnadbookResourceDetailsUI(name, categoryName);
			if (model == null)
			{
				return Redirect(handBookHelper.PageNotFoundUrl());
			}
			return View("MyHandBookResourceDetails", model);
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

		[RelativeRoute("Share"), HttpPost]
		public ActionResult Share()
		{
			Boolean share = true;
			return Json(share);
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