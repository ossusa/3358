using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitefinityWebApp.Mvc.Models
{
	public class IAFCHandBookMyHandBookModel
	{
		public IAFCHandBookMyHandBookModel()
		{
			MyResources = new List<IAFCHandBookResourceModel>();
			MyCompletedResources = new List<IAFCHandBookResourceModel>();
			MayHandBookCategories = new List<IAFCHandBookTopicCategoryModel>();
			TotalDuration = new TimeSpan();
		}

		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public int CompletedResourcesAmount { get; set;}
		public int IncompletedResourcesAmount { get; set; }
		public TimeSpan TotalDuration { get; set; } 
		public List<IAFCHandBookResourceModel> MyResources { get; set; }
		public List<IAFCHandBookResourceModel> MyCompletedResources { get; set; }
		public List<IAFCHandBookTopicCategoryModel> MayHandBookCategories { get; set; }
	}
}