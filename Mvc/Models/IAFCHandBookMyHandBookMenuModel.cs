using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitefinityWebApp.Mvc.Models
{
	public class IAFCHandBookMyHandBookMenuModel
	{
		public IAFCHandBookMyHandBookMenuModel()
		{
			Menu = new List<IAFCHandBookMyHandBookMenuItemModel>();
		}
		public List<IAFCHandBookMyHandBookMenuItemModel> Menu { get; set; }
		public bool IsUserAuthorized {get;set;}
	}
}