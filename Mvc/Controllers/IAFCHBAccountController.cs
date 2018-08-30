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
	[ControllerToolboxItem(Name = "IAFCHBAccountController", Title = "Account", SectionName = "Hand Book MVC Widgets")]
	public class IAFCHBAccountController : Controller
	{
		private IAFCHandBookHelper handBookHelper = new IAFCHandBookHelper();
		public IAFCHandBookAccount GetData()
		{
			var model = handBookHelper.GetAccount();
			return model;
		}
		public ActionResult Index(String userid)
		{
			var model = GetData();
			return View("Account", model);
		}

		public void WeeklyUpdates(bool value)
		{
			handBookHelper.WeeklyUpdates(value);
			var v = value;
			
		}

		public void MonthlyUpdates(bool value)
		{
			handBookHelper.MonthlyUpdates(value);
			
		}

		[ HttpPost, StandaloneResponseFilter]
		public ActionResult Unfollow(Guid categoryId)
		{
			handBookHelper.Unfollow(categoryId);
			var model = GetData();
			return View("_FollowedCategories", model.FollowedCategories);

		}
	}	
	
}