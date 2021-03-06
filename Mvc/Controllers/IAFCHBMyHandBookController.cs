﻿using SitefinityWebApp.Custom.IAFCHandBook;
using SitefinityWebApp.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Services;

namespace SitefinityWebApp.Mvc.Controllers
{
	[ControllerToolboxItem(Name = "IAFCHBMyHandBook", Title = "My HandBook", SectionName = "Hand Book MVC Widgets")]
	public class IAFCHBMyHandBookController : Controller
	{
		private IAFCHandBookHelper handBookHelper;
		public IAFCHBMyHandBookController()
		{
			var url = System.Web.HttpContext.Current.Request.Url.Host;
			handBookHelper = new IAFCHandBookHelper(url);
		}
	
		public IAFCHandBookMyHandBookModel GetData()
		{
			IAFCHandBookMyHandBookModel model = handBookHelper.GetMyHandBook();
			return model;
		}

		public IAFCHandBookMyHandBookModel GetSharedHandBook(String userId)
		{
			IAFCHandBookMyHandBookModel model = handBookHelper.GetMyHandBook(userId);
			return model;
		}

		[RelativeRoute("{userid?}")]
		public ActionResult Index(String userid)
		{

			IAFCHandBookMyHandBookModel model = new IAFCHandBookMyHandBookModel();
			
			if (userid == null)
			{
				model = GetData();
			}
			else
			{
				model = GetSharedHandBook(userid);
			}

			if (model == null)
			{
				return Redirect(handBookHelper.PageNotFoundUrl());
			}

			AddMetaTags(model, userid);
			return View("MyHandBook",model);
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
			meta.Content = @"Chief's A-RIT Administrative Rapid Information Tool";
			page.Header.Controls.Add(meta);


			meta = new HtmlMeta();
			meta.Attributes.Add("property", "og:description");
			meta.Content = @"Chief's A-RIT Administrative Rapid Information Tool Description";
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

		[RelativeRoute("Share"), HttpPost]
		public ActionResult Share(string url)
		{
			String sharedUrl = handBookHelper.GenerateSharedUrl(url);			
			return Json(sharedUrl);
		}
	}
}