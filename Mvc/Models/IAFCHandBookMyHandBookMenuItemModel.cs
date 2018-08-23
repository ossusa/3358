using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitefinityWebApp.Mvc.Models
{
	public class IAFCHandBookMyHandBookMenuItemModel
	{
		public IAFCHandBookMyHandBookMenuItemModel()
		{
			ChildMenuItem = new List<IAFCHandBookMyHandBookMenuItemModel>();
			Visible = true;
			ChildExists = false;
		}

		public String Url { get; set; }
		public String Title { get; set; }
		public Boolean Visible { get; set; }
		public Boolean ChildExists { get; set; }
		public List<IAFCHandBookMyHandBookMenuItemModel> ChildMenuItem { get; set; }
	}
}