using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using MatrixGroup.Ams.MatrixMaxx.Provider;
using Telerik.Web.UI;
using Telerik.Sitefinity.Web.UI;
using MatrixGroup.Sitefinity.Config.AppSettings;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Services;
using System.Collections.Specialized;

namespace SitefinityWebApp.Custom
{
	public partial class MaxxDirectory : System.Web.UI.UserControl
	{

		protected void Page_Load(object sender, EventArgs e)
		{
			directorySearchSubmit.Click += new EventHandler(directorySearchSubmit_Click);
		}

		void directorySearchSubmit_Click(object sender, EventArgs e)
		{
			var urlString = "http://" + HttpContext.Current.Request.Url.Host + AppSettingsUtility.GetValue<String>("Url.MaxxDirectorySearch");

			var searchUrl = new Uri(urlString);
			
			var parameters = HttpUtility.ParseQueryString(searchUrl.Query);
			
			parameters.Add(new NameValueCollection()
			                 	{
			                 		{"anyName", anyName.Text}
			                 	});
			Response.Redirect(searchUrl.GetLeftPart(UriPartial.Path) + "?" + parameters);
		}
	}
}