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
using System.Web.UI.HtmlControls;
using Telerik.Sitefinity.Services;
using System.Web.UI;

namespace SitefinityWebApp.Mvc.Controllers
{
	[ControllerToolboxItem(Name = "IAFCHBMyHandBookCategoryResources", Title = "My HandBook Category Resources", SectionName = "Hand Book MVC Widgets")]
	public class IAFCHBMyHandBookCategoryResourcesController : Controller
	{
		[Category("General")]
		public String CategoryName { get; set; }

		public IAFCHBMyHandBookCategoryResourcesController()
		{
			var url = System.Web.HttpContext.Current.Request.Url.Host;
			handBookHelper = new IAFCHandBookHelper(url);
		}

		private ILog log = LogManager.GetLogger(typeof(IAFCHBMyHandBookCategoryResourcesController));
		private IAFCHandBookHelper handBookHelper;

		[RelativeRoute("{userid?}")]
		public ActionResult Index(String userId)
		{
			IAFCHandBookMyHandBookResourceModelModel model = new IAFCHandBookMyHandBookResourceModelModel();
			model = handBookHelper.GetMyHandBookCategoryResourcesByName(CategoryName, userId);

			if (model == null)
			{
				return Redirect(handBookHelper.PageNotFoundUrl());
			}
			AddMetaTags(model, userId);
			return View("MyHandBookCategoryResources", model);
		}


		public void AddMetaTags(IAFCHandBookMyHandBookResourceModelModel model, string userid)
		{
			var page = (Page)SystemManager.CurrentHttpContext.CurrentHandler;
			var meta = new HtmlMeta();
			meta.Attributes.Add("property", "og:type");
			meta.Content = "article";
			page.Header.Controls.Add(meta);

			meta = new HtmlMeta();
			meta.Attributes.Add("property", "og:title");
			meta.Content = model.Category.CategoryTitle;
			page.Header.Controls.Add(meta);

			meta = new HtmlMeta();
			meta.Attributes.Add("property", "og:description");
			meta.Content = "Chief's A-RIT Administrative Rapid Information Tool";
			page.Header.Controls.Add(meta);

			meta = new HtmlMeta();
			meta.Attributes.Add("property", "og:url");
			if (userid == null)
			{
				meta.Content = handBookHelper.GenerateSharedUrl(System.Web.HttpContext.Current.Request.Url.AbsoluteUri.TrimEnd('/'));
			}
			else
			{
				meta.Content = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.TrimEnd('/');
			}
			page.Header.Controls.Add(meta);

			meta = new HtmlMeta();
			meta.Attributes.Add("property", "og:site_name");
			meta.Content = @"Chief's Administrative Rapid Information Tool";
			page.Header.Controls.Add(meta);

			meta = new HtmlMeta();
			meta.Attributes.Add("property", "og:image");
			meta.Content = "https://dev-staging.iafc.org/images/default-source/1logos/iacfhandbook-logo.png";
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


		[RelativeRoute("GetResources"), HttpPost, StandaloneResponseFilter]
		public ActionResult GetResources(String categoryId)
		{

			var categoryGuid = Guid.Parse(categoryId);
			var model = handBookHelper.GetMyHandBookCategoryResourcesList(categoryGuid);


			var view = PartialView("_MyHandBookCategoryResourcesDetails", model);
			return view;
		}

		[RelativeRoute("AddLike"), HttpPost]
		public ActionResult AddLike(String resourceId, int likeAddAmount, int dislikeAddAmount)
		{
			var id = Guid.Parse(resourceId);

			var likes = handBookHelper.AddLikeForResourceUI(id, "Resource", likeAddAmount, dislikeAddAmount);

			return Json(likes);
		}
		[RelativeRoute("AddDislike"), HttpPost]
		public ActionResult AddDislike(String resourceId, int likeAddAmount, int dislikeAddAmount)
		{
			var id = Guid.Parse(resourceId);
			var likes = handBookHelper.AddLikeForResourceUI(id, "Resource", likeAddAmount, dislikeAddAmount).ToString();

			return Json(likes);
		}

		[RelativeRoute("MarkAsComplete"), HttpPost, StandaloneResponseFilter]
		public ActionResult MarkAsComplete(String resourceId, String categoryId)
		{
			var id = Guid.Parse(resourceId);
			var categoryGuid = Guid.Parse(categoryId);
			var model = new IAFCHandBookMyHandBookResourceModelModel();			
			var markAsComplete = handBookHelper.MarkAsComplete(id);
			model = handBookHelper.GetCategoryResources(categoryGuid, true, null);
			
			var view = PartialView("_MyHandBookCategoryResourcesDetails", model);
			return view;
		}

		[RelativeRoute("Remove"), HttpPost, StandaloneResponseFilter]
		public ActionResult Remove(String resourceId, String categoryId)
		{
			var id = Guid.Parse(resourceId);
			var categoryGuid = Guid.Parse(categoryId);
			var model = new IAFCHandBookMyHandBookResourceModelModel();
			var markAsComplete = handBookHelper.RemoveResource(id);
			model = handBookHelper.GetCategoryResources(categoryGuid, true, null);			
			var view = PartialView("_MyHandBookCategoryResourcesDetails", model);
			return view;
		}

		
		[RelativeRoute("AddSharedToMyHandBook"), HttpPost]
		public ActionResult AddSharedToMyHandBook(String resourceId)
		{
			var id = Guid.Parse(resourceId);						
			var addedToMyHandBool = handBookHelper.AddToMyHandBook(id);
						
			return Json(addedToMyHandBool);
		}

		[RelativeRoute("RemoveCompleted"), HttpPost, StandaloneResponseFilter]
		public ActionResult RemoveCompleted(String resourceId, String categoryId, String userId)
		{
			var id = Guid.Parse(resourceId);
			var categoryGuid = Guid.Parse(categoryId);
			var model = new IAFCHandBookMyHandBookResourceModelModel();			
			var markAsComplete = handBookHelper.RemoveResource(id, "MyCompletedResources");
			
			model = handBookHelper.GetCategoryResources(categoryGuid, true, null);
			var view = PartialView("_MyHandBookCategoryResourcesDetails", model);
			return view;
		}

		[RelativeRoute("OrderBy"), HttpPost, StandaloneResponseFilter]
		public ActionResult OrderBy(String orderBy, String categoryId, String userId, String sharedUserID)
		{

			var categoryGuid = Guid.Parse(categoryId);
			var model = new IAFCHandBookMyHandBookResourceModelModel();
			if (sharedUserID == Guid.Empty.ToString())
			{
				model = handBookHelper.GetCategoryResources(categoryGuid, true, null, orderBy);
			}
			else
			{
				model = handBookHelper.GetCategoryResources(categoryGuid, true, sharedUserID, orderBy);
			}
			var view = PartialView("_MyHandBookCategoryResourcesDetails", model);
			return view;
		}


		[RelativeRoute("Edit"), HttpPost]
		public ActionResult Edit(String operation)
		{
			Boolean edit = true;
			if (operation.Equals("Cancel"))
			{
				edit = false;
			}
			return Json(edit);
		}

		[RelativeRoute("Share"), HttpPost]
		public ActionResult Share(String url)
		{
			String sharedUrl = handBookHelper.GenerateSharedUrl(url);
			return Json(sharedUrl);
		}

	}
}
