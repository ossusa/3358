using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Events.Model;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Model;
using Telerik.OpenAccess;
using System.Runtime.Serialization;

namespace SitefinityWebApp.Api.Models
{
	[DataContract]
	public class ApiEvent
	{
		#region properties
		
		[DataMember(Name="id")]
		public Guid Id { get; set; }

		[DataMember(Name = "urlName")]
		public string UrlName { get; set; }

		[DataMember(Name = "title")]
		public string Title { get; set; }

		[DataMember(Name = "url")]
		public string Url { get; set; }

		[DataMember(Name = "isAllDay")]
		public bool IsAllDay { get; set; }

		[DataMember(Name = "eventStart")]
		public DateTime EventStart { get; set; }

		[DataMember(Name = "eventEnd")]
		public DateTime? EventEnd { get; set; }

		[DataMember(Name = "summary")]
		public string Summary { get; set; }

		[DataMember(Name = "location")]
		public EventLocation Location { get; set; }

		[DataMember(Name = "categories")]
		public List<string> Categories { get; set; }

		[DataMember(Name = "tags")]
		public List<string> Tags { get; set; }

		#endregion

		#region contructor
		
		public ApiEvent(Event sfEvent)
		{
			Id = sfEvent.Id;
			UrlName = sfEvent.UrlName;
			Title = sfEvent.Title;
			Url = SystemManager.GetContentLocationService().GetItemDefaultLocation(sfEvent).ItemAbsoluteUrl;
			IsAllDay = sfEvent.AllDayEvent;
			EventStart = sfEvent.EventStart;
			EventEnd = sfEvent.EventEnd;
			Summary = sfEvent.Summary;
			Location = new EventLocation()
			{
				Street = sfEvent.Street,
				City = sfEvent.City,
				Country = sfEvent.Country,
				State = sfEvent.State
			};

			Categories = new List<string>();
			var manager = TaxonomyManager.GetManager();
			var categoryIds = sfEvent.GetValue("Category") as TrackedList<Guid>;
			foreach (var id in categoryIds)
			{
				Categories.Add(manager.GetTaxon(id).Name);
			}

			Tags = new List<string>();
			var tagIds = sfEvent.GetValue("Tags") as TrackedList<Guid>;
			foreach (var id in tagIds)
			{
				Tags.Add(manager.GetTaxon(id).Name);
			}
		}
		
		#endregion
	}
}