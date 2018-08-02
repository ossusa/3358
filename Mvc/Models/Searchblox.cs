using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitefinityWebApp.Mvc.Models
{
	public class Searchblox
	{
		public string Query { get; set; }
		public int Page { get; set; }
		public List<SearchbloxResult> Results { get; set; }
		public int NumResults { get; set; }
		public int PageStart { get; set; }
		public int PageEnd { get; set; }
		public int NumPages { get; set; }
		public string PageingUrl { get; set; }
		public string ErrorMessage { get; set; }

		public bool ShowErrors { get; set; }
		public bool ShowNumResults { get; set; }
		public bool ShowSearchBox { get; set; }

		public Searchblox()
		{
			this.Page = 1;
			this.Results = new List<SearchbloxResult>();
		}
	}


}