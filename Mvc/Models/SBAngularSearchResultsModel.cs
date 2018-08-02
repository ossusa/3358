using System;
using System.Linq;

namespace SitefinityWebApp.Mvc.Models
{
    /*========================================================================\
     |   LOOKING TO MAKE CHANGES TO THIS WIDGET? CHECK OUT THE CONTROLLER!    |
     `=======================================================================*/

    /// <summary>
    /// 
    /// </summary>
    public class SBAngularSearchResultsModel
    {

        /* Quick question about these definitions:
         * Should they have the attribute [Category] that I've seen in the
         * controller file?
         * Also, why are these fields redefined in the controller?
         */

        /// <summary>
        ///   String containing a comma-separated series of integers, each
        /// representing a database of crawled information.
        /// </summary>
        public string CollectionId { get; set; }
        
        /// <summary>
        ///   String containing an IP Address or URL pointing to the Searchblox
        /// instance.
        /// </summary>
        public string SearchBloxServerIp { get; set; }

        /// <summary>
        ///   String containing a series of facet names, comma separated.
        /// </summary>
        public string FacetNames {get; set; }

        /// <summary>
        ///   String containing the name of the special date ranges facet.
        /// </summary>
        public string DateRangeName { get; set; }
        
        /// <summary>
        ///   If set to true, will display a search box alongside the search
        /// results.
        /// </summary>
        public bool ShowSearchBox {get; set; }
        
        /// <summary>
        ///   Gets or sets the message.
        /// </summary>
        public string CategoryLimiter { get; set; }
        
        /// <summary>
        ///   Gets or sets the metatag limiter, whatever that means.
        /// </summary>
        public string MetatagLimiter { get; set; }

        /// <summary>
        ///   String containing each category to exclude, comma separated.
        /// </summary>
        public string ExcludedCategories { get; set; }
    }
}