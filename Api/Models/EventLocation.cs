using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SitefinityWebApp.Api.Models
{
	[DataContract]
	public class EventLocation
	{
		[DataMember(Name = "street")]
		public string Street { get; set; }

		[DataMember(Name = "city")]
		public string City { get; set; }

		[DataMember(Name = "country")]
		public string Country { get; set; }

		[DataMember(Name = "state")]
		public string State { get; set; }
	}
}