using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitefinityWebApp.Mvc.Models
{
    public class ContentListSearchResult
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Summary { get; set; }
        public string ImageUrl { get; set; }
        public string Type { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime? EventStart { get; set; }
        public DateTime? EventEnd { get; set; }
    }
}