﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitefinityWebApp.Mvc.Models
{
	public class IAFCHandBookMyHandBookResourceModelModel
	{
		public IAFCHandBookMyHandBookResourceModelModel()
		{
			Category = new IAFCHandBookTopicCategoryModel();
			MyCompletedResources = new List<IAFCHandBookResourceModel>();
			MyResources = new List<IAFCHandBookResourceModel>();
			TotalDuration = new TimeSpan();
			MyChildHandBookResources = new List<IAFCHandBookMyHandBookResourceModelModel>();
			MoreCategories = new List<IAFCHandBookTopicCategoryModel>();
		}
		public int CompletedResourcesAmount { get; set; }
		public int IncompletedResourcesAmount { get; set; }
		public TimeSpan TotalDuration { get; set; }
		public IAFCHandBookTopicCategoryModel Category { get; set; }
		public List<IAFCHandBookTopicCategoryModel> MoreCategories { get; set; }
		public List<IAFCHandBookResourceModel> MyResources { get; set; }
		public List<IAFCHandBookResourceModel> MyCompletedResources { get; set; }	
		public List<IAFCHandBookMyHandBookResourceModelModel> MyChildHandBookResources { get; set; }
		public List<IAFCHandBookTopicOrderBy> OrderBy { get; set; }
		public Guid UserId { get; set; }
		public Guid SharedUserId { get; set; }
		public string SharedUser { get; set; }	
	}
}