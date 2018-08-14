using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitefinityWebApp.Mvc.Models
{
	public class IAFCHandBookMyHandBookCategoryModel
	{
		public IAFCHandBookMyHandBookCategoryModel()
		{
			MyHandBookCategories = new List<IAFCHandBookTopicCategoryModel>();
		}
		public Guid Id { get; set; }
		public int CompleteResources { get; set; }
		public int IncompletedResources { get; set; }
		List<IAFCHandBookTopicCategoryModel > MyHandBookCategories { get; set; }


	}
}