using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitefinityWebApp.Mvc.Models
{
	public class IAFCHandBookCategoryResourcesModel
	{
		public IAFCHandBookCategoryResourcesModel()
		{
			ParenCategory = new IAFCHandBookTopicCategoryModel();
			ChildCategories = new List<IAFCHandBookTopicCategoryModel>();
		}

		public IAFCHandBookTopicCategoryModel ParenCategory { get; set; }
		public List<IAFCHandBookTopicCategoryModel> ChildCategories { get; set; }
	}
}