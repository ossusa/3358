using ServiceStack.Logging;
using SitefinityWebApp.Custom.IAFCHandBook;
using SitefinityWebApp.Mvc.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Mvc.ActionFilters;
using Telerik.Sitefinity.Services;

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
			AddMetaTags(model);
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
			AddMetaTags(model);
			return View("ResourceDetails", model);
		}
		public void AddMetaTags(IAFCHandBookResourceModel model)
		{
			var page = (Page)SystemManager.CurrentHttpContext.CurrentHandler;
			var meta = new HtmlMeta();
			meta.Attributes.Add("property", "og:type");
			meta.Content = "video";
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
			meta.Content = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.TrimEnd('/');
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
	}
}