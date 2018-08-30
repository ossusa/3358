using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitefinityWebApp.Mvc.Models
{
	public class IAFCHandBookAccount
	{
		public IAFCHandBookAccount()
		{
			FollowedCategories = new List<IAFCHandBookTopicCategoryModel>();
		}
		public Guid UserId { get; set; }
		public bool WeeklyUpdates { get; set; }
		public bool MonthlyUpdates { get; set; }
		public List<IAFCHandBookTopicCategoryModel> FollowedCategories { get; set; }

	}
}