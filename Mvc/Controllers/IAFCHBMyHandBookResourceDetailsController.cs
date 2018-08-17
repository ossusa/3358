using SitefinityWebApp.Custom.IAFCHandBook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;

namespace SitefinityWebApp.Mvc.Controllers
{
	[ControllerToolboxItem(Name = "IAFCHBMyHandBookResourceDetails", Title = "My HandBook Resource Details", SectionName = "Hand Book MVC Widgets")]
	public class IAFCHBMyHandBookResourceDetailsController: Controller
	{
		private IAFCHandBookHelper handBookHelper = new IAFCHandBookHelper();

		public IAFCHBMyHandBookResourceDetailsController()
		{

		}
		
		[RelativeRoute("{name}")]
		public ActionResult Index(String name)
		{
			return View("MyHandBookResourceDetail");
		}
	}
}