using SitefinityWebApp.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Telerik.Sitefinity.Blogs.Model;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Libraries.Model;
using Telerik.Sitefinity.Lists.Model;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Modules.Blogs;
using Telerik.Sitefinity.Modules.Events;
using Telerik.Sitefinity.Modules.Lists;
using Telerik.Sitefinity.Modules.News;
using Telerik.Sitefinity.News.Model;
using Telerik.Sitefinity.RelatedData;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Taxonomies.Model;

namespace SitefinityWebApp.Api.Controller
{
	public class EventsController : ApiController
	{

        /**
         * Get all events.
         */
		public EventsViewModel GetAll()
		{
			var manager = EventsManager.GetManager();
			var events =
				manager.GetEvents()
				.Where(n => n.Status == ContentLifecycleStatus.Live && n.Visible == true)
				.Select(n => new ApiEvent(n));

			var model = new EventsViewModel(events);

			return model;
		}

        /**
         * Get all future events.
         */
        public EventsViewModel GetFuture()
        {
            var manager = EventsManager.GetManager();
            var events =
                manager.GetEvents()
                .Where(n => n.Status == ContentLifecycleStatus.Live && n.Visible == true && n.EventStart > DateTime.UtcNow)
                .Select(n => new ApiEvent(n));

            var model = new EventsViewModel(events);

            return model;
        }

	}
}