using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitefinityWebApp.Mvc.Models
{
	public class IAFCHandBookResourceDetailsModel
	{
		public IAFCHandBookResourceDetailsModel()
		{
			Duration = new TimeSpan();
			Category = new IAFCHandBookTopicCategoryModel();
		}
		public Guid id { get; set; }
		public string ResourceTitle { get; set; }
		public string ResourceSummary { get; set; }
		public string ResourceDescription { get; set; }
		public IAFCHandBookTopicCategoryModel Category { get; set; }
		public string ResourceType { get; set; }
		public TimeSpan Duration { get; set; }
		public string DurationStr { get; set; }
		public string ImageUrl { get; set; }
		public string ImageSvgUrl { get; set; }
		public string ImagePlaceholderUrl { get; set; }
		public string VideoEmbedCode { get; set; }		
		public bool IsResourceHasMoreThen1Category { get; set; }
	}
}