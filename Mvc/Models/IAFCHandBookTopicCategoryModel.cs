using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitefinityWebApp.Mvc.Models
{
	public class IAFCHandBookTopicCategoryModel
	{
		public IAFCHandBookTopicCategoryModel()
		{
			ChildCategories = new List<IAFCHandBookTopicCategoryModel>();
			TotalDuration = new TimeSpan();
		}
		public Guid Id { get; set; }
		public string CategoryTitle { get; set; }
		public string CategoryDescription { get; set; }
		public string CategoryUrl { get; set; }
		public string ParentCategoryTitle { get; set; }
		public string ParentCategoryUrl { get; set; }
		public int ResourcesAmount { get; set; }
		public string ResourcesTotalDuration { get; set; }
		public string TopicCategoryImageUrl { get; set; }
		public TimeSpan TotalDuration { get; set; }
		public string MyHandBookCategoryUrl { get; set; }
		public string MyHandBookParentCategoryUrl { get; set; }
		public int MyHandBookCompletedResources { get; set; }
		public int MyHandBookInCompletedResources { get; set; }
		public string MyHandBookResourcesTotalDuration { get; set; }
		public List<IAFCHandBookTopicCategoryModel> ChildCategories { get; set; }

	}
}