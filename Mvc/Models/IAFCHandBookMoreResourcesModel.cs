using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitefinityWebApp.Mvc.Models
{
	public class IAFCHandBookMoreResourcesModel
	{
		public IAFCHandBookMoreResourcesModel()
		{
			Likes = new IAFCHandBookLikesModel();
			ResourceDetails = new IAFCHandBookResourceDetailsModel();
		}

		public Guid Id { get; set; }

		public string ResourceUrl { get; set; }

		public IAFCHandBookLikesModel Likes { get; set; }

		public IAFCHandBookResourceDetailsModel ResourceDetails { get; set; }
	}
}