using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SitefinityWebApp.Mvc.Models
{
    public class SearchCriteria
    {
        public string Term { get; set; }
        public string SecondTerm { get; set; }
        public string[] Types { get; set; }
        public string[] Authors { get; set; }
        public string[] Topics { get; set; }

        [Display(Name = "Sort By")]
        public string OrderBy { get; set; }

        #region SelectLists

        public SelectList TypesList { get; set; }
        public SelectList AuthorsList { get; set; }
        public SelectList TopicsList { get; set; }

        #endregion
    }
}