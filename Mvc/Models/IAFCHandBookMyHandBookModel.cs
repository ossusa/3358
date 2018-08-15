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
			MyHandBookResurces = new List<IAFCHandBookMyHandBookResourceModelModel>();
		}

		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public List<IAFCHandBookMyHandBookResourceModelModel> MyHandBookResurces { get; set; }

	}
}