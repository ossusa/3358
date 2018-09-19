using ServiceStack.Logging;
using SitefinityWebApp.Custom.IAFCHandBook;
using SitefinityWebApp.Mvc.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Mvc.ActionFilters;
using Telerik.Sitefinity.Services;

namespace SitefinityWebApp.Mvc.Controllers
{
	[ControllerToolboxItem(Name = "IAFCHBMyHandBookAllResourcesPerCategory", Title = "My HandBook All Resources Per Category", SectionName = "Hand Book MVC Widgets")]
	public class IAFCHBMyHandBookAllResourcesPerCategoryController: Controller
	{
		[Category("General")]
		public String CategoryName { get; set; }

		private ILog log = LogManager.GetLogger(typeof(IAFCHBResourcesPerCategoryController));
		private IAFCHandBookHelper handBookHelper;

		public IAFCHBMyHandBookAllResourcesPerCategoryController()
		{
			var url = System.Web.HttpContext.Current.Request.Url.Host;
			handBookHelper = new IAFCHandBookHelper(url);
		}
		
		public IAFCHandBookMyHandBookModel GetData(String categoryName, String userId)
		{
			return handBookHelper.GetMyHandBookResourcesPerCategory(categoryName, userId);
		}


		[RelativeRoute("{userid?}")]
		public ActionResult Index(String userid)
		{
			var model = GetData(CategoryName, userid);
			if (model == null)
			{
				return Redirect(handBookHelper.PageNotFoundUrl());
			}
			AddMetaTags(model, userid);
			var view = View("MyHandBookAllResourcesPerCategory", model);
			return view;
		}

		public void AddMetaTags(IAFCHandBookMyHandBookModel model, string userid)
		{
			var page = (Page)SystemManager.CurrentHttpContext.CurrentHandler;
			var meta = new HtmlMeta();
			meta.Attributes.Add("property", "og:type");
			meta.Content = "article";
			page.Header.Controls.Add(meta);

			meta = new HtmlMeta();
			meta.Attributes.Add("property", "og:title");
			meta.Content = model.MyHandBookResurces.First().Category.CategoryTitle;
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
				System.Web.HttpContext.Current.Request.Url.AbsoluteUri.TrimEnd('/');
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
		public ActionResult MarkAsComplete(String resourceId, String categoryId, String userId)
		{
			var id = Guid.Parse(resourceId);
			var categoryGuid = Guid.Parse(categoryId);
			var model = new IAFCHandBookMyHandBookResourceModelModel();
			var markAsComplete = handBookHelper.MarkAsComplete(id);
			model = handBookHelper.GetCategoryResources(categoryGuid, false, userId);

			var view = PartialView("_MyHandBookAllResourcesPerCategoryDetails", model);
			return view;
		}

		[RelativeRoute("Remove"), HttpPost, StandaloneResponseFilter]
		public ActionResult Remove(String resourceId, String categoryId, String userId)
		{
			var id = Guid.Parse(resourceId);
			var categoryGuid = Guid.Parse(categoryId);
			var model = new IAFCHandBookMyHandBookResourceModelModel();
			var markAsComplete = handBookHelper.RemoveResource(id);
			model = handBookHelper.GetCategoryResources(categoryGuid, false, userId);

			var view = PartialView("_MyHandBookAllResourcesPerCategoryDetails", model);
			return view;
		}

		[RelativeRoute("AddSharedToMyHandBook"), HttpPost]
		public ActionResult AddSharedToMyHandBook(String resourceId)
		{
			var id = Guid.Parse(resourceId);
			var addedToMyHandBool = handBookHelper.AddToMyHandBook(id);

			return Json(addedToMyHandBool);
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