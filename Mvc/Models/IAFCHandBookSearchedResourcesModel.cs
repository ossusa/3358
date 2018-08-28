using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitefinityWebApp.Mvc.Models
{
	public class IAFCHandBookSearchedResourcesModel
	{
		public IAFCHandBookSearchedResourcesModel()
		{
			Resources = new List<IAFCHandBookResourceModel>();			

			OrderBy = new List<IAFCHandBookTopicOrderBy>();
		}
		
		public List<IAFCHandBookResourceModel> Resources { get; set; }		
		public List<IAFCHandBookTopicOrderBy> OrderBy { get; set; }
		public string SearchText { get; set; }
		
	}
}