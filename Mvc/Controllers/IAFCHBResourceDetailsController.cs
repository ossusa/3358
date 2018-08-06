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
			var likes = AddLikeForResource(id, "Resource").ToString();

			return Json(likes);
		}


		[RelativeRoute("AddDislike"), HttpPost]
		public ActionResult AddDislike(String resourceId)
		{
			var id = Guid.Parse(resourceId);
			var dislikes = AddDislikeForResource(id, "Resourse").ToString();

			return Json(dislikes);
		}

		[RelativeRoute("AddToMyHandBook"), HttpPost]
		public ActionResult AddToMyHandBook(String resourceId)
		{
			var id = Guid.Parse(resourceId);
			Boolean addToMyHandBook = true;

			return Json(addToMyHandBook);
		}

		[RelativeRoute("AddComment"), HttpPost]
		public ActionResult AddComment(String commentTxt, String resourceId)
		{
			var id = Guid.Parse(resourceId);
			string CommentTxt = commentTxt;

			return Json(CommentTxt);
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