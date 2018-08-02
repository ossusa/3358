using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SitefinityWebApp.Api.Models
{
	/// <summary>
	/// Wrapper for list of events and metadata on the list
	/// @TODO: Implement paging, consider adding taxonomy summary.
	/// </summary>
	[DataContract]
	public class EventsViewModel
	{
		[DataMember(Name = "events")]
		public IEnumerable<ApiEvent> Events { get; set; }

		[DataMember(Name = "pageNumber")]
		public int PageNumber { get; set; }

		[DataMember(Name = "pageCount")]
		public int PageCount { get; set; }

		[DataMember(Name = "totalCount")]
		public int TotalCount { get; set; }


		public EventsViewModel(IEnumerable<ApiEvent> events)
		{
			Events = events;
			PageNumber = 1;
			PageCount = Events.Count();
			TotalCount = PageCount;
		}
	}
}