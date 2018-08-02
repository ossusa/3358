using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using MatrixGroup.Ams.MatrixMaxx;
using MatrixGroup.Ams.MatrixMaxx.Linq;
using MatrixGroup.Ams.MatrixMaxx.Provider;
using MatrixGroup.Ams.Provider;
using Telerik.Sitefinity.Mvc;
using MatrixGroup.Sitefinity.Utilities;

namespace SitefinityWebApp.Mvc.Controllers
{
	/// <summary>
	/// 
	/// </summary>
	[ControllerToolboxItem(Name = "MaxxEvents", Title = "Maxx Events", SectionName = "MatrixMaxxToolboxSection")]
	public class MaxxEventsController : Controller
	{
		public string MaxxCategoryId { get; set; }
		public int MeetingTypeId { get; set; }
		public string MaxxCalendarName { get; set; }
		public int Take { get; set; }
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public ActionResult Index()
		{
			try
			{
				var provider = MatrixGroup.Ams.Provider.Ams.Providers["MaxxAmsProvider"] as MatrixGroup.Ams.MatrixMaxx.Provider.MaxxAmsProvider;
				var filterTaxon = Taxonomy.GetTaxonFromUrl(System.Web.HttpContext.Current);
				if (filterTaxon != null && String.IsNullOrEmpty(MaxxCategoryId))
					MaxxCategoryId = filterTaxon.Description;

				var events = provider.GetUpcomingMeetings().AsQueryable();//provider.Query<Meeting>().Where(m => m.CalendarItem.StartDate >= DateTime.Today);

				if (!string.IsNullOrEmpty(MaxxCategoryId))
					events = provider.GetUpcomingMeetingsByCategoryId(MaxxCategoryId).AsQueryable();

				if (MeetingTypeId != default(int))
					events = events.Where(m => m.TypeId == MeetingTypeId);

				if (!string.IsNullOrEmpty(MaxxCalendarName))
					events = events.Where(m => m.CalendarItem != null && m.CalendarItem.Calendars != null && m.CalendarItem.Calendars.Any(c => c.Name == MaxxCalendarName));
					
				var results = events.OrderBy(m => m.CalendarItem.StartDate).Take(Take).ToList();

				return View(results);

			}
			catch
			{
				return null;
			}
		}
	}
}