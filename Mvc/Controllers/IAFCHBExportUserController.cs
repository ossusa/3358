using SitefinityWebApp.Custom.IAFCHandBook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;

namespace SitefinityWebApp.Mvc.Controllers
{
	[ControllerToolboxItem(Name = "IAFCHBExportUser", Title = "Export Users", SectionName = "Hand Book MVC Widgets")]
	public class IAFCHBExportUserController : Controller
	{
		private IAFCHandBookHelper handBookHelper;
		public IAFCHBExportUserController()
		{
			var url = System.Web.HttpContext.Current.Request.Url.Host;
			handBookHelper = new IAFCHandBookHelper(url);
		}

		public ActionResult Index(String userid)
		{
			return View("Index");
		}
		
		public FileContentResult DownloadCSV()
		{
			string filename = "Users_"+DateTime.UtcNow.ToString("yyyyMMddhhmmss")+".csv";

			string csv = "User edit date, Helix ID, Ind ID, First Name, Last Name, Email, Date, Grp ID, Opt -in to XYZ subtopic(for each topic" + System.Environment.NewLine;
			csv = csv + handBookHelper.GetUsers();
			return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv", filename);
		}
	}
}