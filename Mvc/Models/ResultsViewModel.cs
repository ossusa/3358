using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Sitefinity.Services.Search.Data;

namespace SitefinityWebApp.Mvc.Models
{
    public class ResultsViewModel
    {
        public List<SearchResult> Results { get; set; }
        public SearchCriteria Criteria { get; set; }
        public Pagination Pagination { get; set; }
    }
}