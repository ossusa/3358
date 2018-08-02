using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Sitefinity.Services.Search.Data;

namespace SitefinityWebApp.Mvc.Models
{
    public class ContentListResultsViewModel
    {
        public List<ContentListSearchResult> Results { get; set; }
        public ContentListSearchCriteria Criteria { get; set; }
        public Pagination Pagination { get; set; }
    }
}