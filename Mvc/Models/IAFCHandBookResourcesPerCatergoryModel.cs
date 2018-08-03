using SitefinityWebApp.Custom.IAFCHandBook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitefinityWebApp.Mvc.Models
{
	public class IAFCHandBookResourcesPerCatergoryModel
	{
		public IAFCHandBookTopicCategoryModel Category { get; set; }
		public List<IAFCHandBookResourceModel> Resources { get; set; }
		public List<IAFCHandBookTopicCategoryModel> MoreCategories { get; set; }

		public List<ResourcesOrderBy> OrderBy {get;set;}
		public IAFCHandBookResourcesPerCatergoryModel()
		{
			Resources = new List<IAFCHandBookResourceModel>();
			Category = new IAFCHandBookTopicCategoryModel();
			MoreCategories = new List<IAFCHandBookTopicCategoryModel>();
			OrderBy = new List<ResourcesOrderBy>();
			OrderBy.Add(ResourcesOrderBy.MostPopular);
			OrderBy.Add(ResourcesOrderBy.MostRecent);
			OrderBy.Add(ResourcesOrderBy.AlphabeticalAZ);
			OrderBy.Add(ResourcesOrderBy.AlphabeticalZA);


		}

		
	}
}