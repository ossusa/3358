using System.Collections.Generic;

namespace SitefinityWebApp.Mvc.Models
{
	public class BlendedNewsListModel
	{
		public IEnumerable<NewsResult> ResultSet { get; set; }

		public Pagination Pagination { get; set; }

	}
}