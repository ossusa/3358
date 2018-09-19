using SitefinityWebApp.Custom.IAFCHandBook;
using SitefinityWebApp.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Mvc.ActionFilters;
using Telerik.Sitefinity.Services;

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
			AddMetaTags(model, userid);
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
			AddMetaTags(model, userid);
			return View("MyHandBookResourceDetails", model);
		}

		public void AddMetaTags(IAFCHandBookResourceModel model, string userid)
		{
			var page = (Page)SystemManager.CurrentHttpContext.CurrentHandler;
			var meta = new HtmlMeta();
			meta.Attributes.Add("property", "og:type");
			meta.Content = "article";
			page.Header.Controls.Add(meta);

			meta = new HtmlMeta();
			meta.Attributes.Add("property", "og:title");
			meta.Content = model.ResourceDetails.ResourceTitle;
			page.Header.Controls.Add(meta);

			meta = new HtmlMeta();
			meta.Attributes.Add("property", "og:description");
			meta.Content = model.ResourceDetails.ResourceSummary;
			page.Header.Controls.Add(meta);

			meta = new HtmlMeta();
			meta.Attributes.Add("property", "og:url");
			if (userid == null)
			{
				meta.Content = handBookHelper.GenerateSharedUrl(System.Web.HttpContext.Current.Request.Url.AbsoluteUri.TrimEnd('/'));
			}
			else
			{
				System.Web.HttpContext.Current.Request.Url.AbsoluteUri.TrimEnd('/');
			}
			page.Header.Controls.Add(meta);

			meta = new HtmlMeta();
			meta.Attributes.Add("property", "og:site_name");
			meta.Content = @"Chief's Administrative Rapid Information Tool";
			page.Header.Controls.Add(meta);

			meta = new HtmlMeta();
			meta.Attributes.Add("property", "og:image");
			if (model.ResourceDetails.ImageUrl != null && model.ResourceDetails.ImageUrl != String.Empty)
			{
				meta.Content = model.ResourceDetails.ImageUrl;
			}
			else
			{
				meta.Content = model.ResourceDetails.ImageSvgUrl;
			}
			page.Header.Controls.Add(meta);

			meta = new HtmlMeta();
			meta.Attributes.Add("property", "og:video");
			meta.Content = model.ResourceDetails.VideoEmbedCode;
			page.Header.Controls.Add(meta);

			meta = new HtmlMeta();
			meta.Attributes.Add("property", "og:video:height");
			meta.Content = "385";
			page.Header.Controls.Add(meta);

			meta = new HtmlMeta();
			meta.Attributes.Add("property", "og:video:width");
			meta.Content = "640";
			page.Header.Controls.Add(meta);
	 
		    meta = new HtmlMeta();
			meta.Name = "description";
			meta.Content = @"Chief's A-RIT Administrative Rapid Information Tool";
			page.Header.Controls.Add(meta);

			meta = new HtmlMeta();
			meta.Name = "twitter:card";
			meta.Attributes.Add("value", "summary");
			page.Header.Controls.Add(meta);

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