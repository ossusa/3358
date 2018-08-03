using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitefinityWebApp.Mvc.Models
{
	public class IAFCHandBookTopicCategoryModel
	{
		public Guid Id { get; set;}
		public string CategoryTitle { get; set; }		
		public string CategoryDescription { get; set; }
		public string CategoryUrl { get; set; }

		public string ParentCategoryTitle { get; set; }
		public string ParentCategoryUrl { get; set; }

		public int ResourcesAmount { get; set; }
		public string ResourcesTotalDuration { get; set; }				
		public string TopicCategoryImageUrl { get; set; }


	}
}