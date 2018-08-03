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

			

		public IAFCHandBookResourceModel GetData(Guid resourceId)
		{

			return handBookHelper.GetResourceDetails(resourceId);

		}

		[RelativeRoute("{id}")]
		public ActionResult GetResourceDetails(string id)
		{
			var resourceId = Guid.Parse(id);
			var model = GetData(resourceId);
			return View("ResourceDetails", model);
			
		}

		




	}
}