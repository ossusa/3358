using SitefinityWebApp.Custom.IAFCHandBook;
using SitefinityWebApp.Mvc.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;

namespace SitefinityWebApp.Mvc.Controllers
{
	[ControllerToolboxItem(Name = "IAFCHBResourceDetails", Title = "Resource Details", SectionName = "Hand Book MVC Widgets")]
	public class IAFCHBResourceDetailsController:Controller
	{
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
			var likes = AddLikeForResource(id).ToString();

			return Json(likes);
		}


		[RelativeRoute("AddDislike"), HttpPost]
		public ActionResult AddDislike(String resourceId)
		{
			var id = Guid.Parse(resourceId);
			var dislikes = AddDislikeForResource(id).ToString();

			return Json(dislikes);
		}

		#region Likes
		public int AddLikeForResource(Guid resourceID)
		{
			var currentLikes = handBookHelper.AddLikeForResource(resourceID);
			return currentLikes;
		}

		public int AddDislikeForResource(Guid resourceID)
		{
			var currentDislikes = handBookHelper.AddDislikeForResource(resourceID);
			return currentDislikes;
		}
		#endregion AddLikes




	}
}