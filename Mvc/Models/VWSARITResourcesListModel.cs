using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitefinityWebApp.Mvc.Models
{
	public class VWSARITResourcesListModel
	{
		public VWSARITResourcesListModel()
		{

		}
		public Guid Id { get; set; }
		public String ResourceTitle { get; set; }
		public DateTime CreatedDate { get; set; }
	}
}