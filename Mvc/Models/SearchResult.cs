using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitefinityWebApp.Mvc.Models
{
    public class SearchResult
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Summary { get; set; }
        public string ImageUrl { get; set; }
        public string Type { get; set; }
        public List<CategoryPair> ResourceTypes { get; set; }
        public List<CategoryPair> Author { get; set; }
        public string DocumentFolder { get; set; }
        public List<string> DocumentParentFolders { get; set; }
        public DateTime PublicationDate { get; set; }
        public List<CategoryPair> CategoryPair { get; set; }
        public DateTime? EventStart { get; set; }
        public DateTime? EventEnd { get; set; }

        public SearchResult()
        {
            this.Title = "";
            this.Link = "";
            this.Summary = "";
            this.ImageUrl = "";
            this.Type = "";
            this.ResourceTypes = new List<CategoryPair>();
            this.Author = new List<CategoryPair>();
            this.DocumentFolder = "";
            this.PublicationDate = new DateTime();
            this.CategoryPair = new List<CategoryPair>();
            this.EventStart = new DateTime();
            this.EventEnd = new DateTime();
        }
    }

    public class CategoryPair
    {
        public string Name { get; set; }
        public string Guid { get; set; }

        public CategoryPair()
        {
            this.Name = "";
            this.Guid = "";
        }
    }
}