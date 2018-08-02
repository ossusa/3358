using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Sitefinity.Libraries.Model;
using Telerik.Sitefinity.Modules.Libraries;

namespace SitefinityWebApp.Mvc.Models
{
    public class BlendedListItem
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public string Link { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public bool Featured { get; set; }
        public string Tags { get; set; }
        public string LocationStreet { get; set; }
        public string LocationState { get; set; }
        public string LocationCity { get; set; }
        public List<CategoryPair> Categories { get; set; }
        public string ContentType { get; set; }
        public string Image { get; set; }
        public bool Protected { get; set; }
        public string DefaultLinkBase { get; set; }
        public bool SelfPaced { get; set; }
        public string DisplayDate { get; set; }
        public DateTime DateField { get; set; }
        public List<CategoryPair> ResourceTypes { get; set; }
        public List<CategoryPair> OrganizationalAuthors { get; set; }
    }
}