﻿using System;
using System.Collections.Generic;
using System.Linq;
using MatrixGroup.Sitefinity.Config.AppSettings;
﻿using ServiceStack.Logging;
﻿using Telerik.Sitefinity.Abstractions;
﻿using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Services.Search;
using Telerik.Sitefinity.Services.Search.Data;
using Telerik.Sitefinity.Web.UI.Components;
using Telerik.Sitefinity.Publishing;
using Telerik.Sitefinity.Publishing.Model;

namespace SitefinityWebApp.Custom.News
{
	public class BlendedNewsHelper
	{
	    private static ILog log = LogManager.GetLogger(typeof (BlendedNewsHelper));

		public static IResultSet GetNewsItems(string[] providers, Guid searchIndexGuid, out int hitCount, Guid[] categories = null, string terms = null, int skip = 0, int take = 0, string[] orderToSortResults = null)
		{
            ISearchService searchService = ServiceBus.ResolveService<ISearchService>();
            ISearchQuery searchQuery = ObjectFactory.Resolve<ISearchQuery>();
            var queryBuilder = ObjectFactory.Resolve<IQueryBuilder>();

            //Note: Make sure the Index item has been re-generated via sitfinity's backend to get proper results
            //Guid.Parse("6894B15C-7836-6C70-9642-FF00005F0421")
            var publishingPoint = PublishingManager.GetManager("SearchPublishingProvider")
                                                           .GetPublishingPoint(searchIndexGuid) as PublishingPoint;
            string catalogName = (publishingPoint.PipeSettings.First(p => p.PipeName == "SearchIndex") as SearchIndexPipeSettings).CatalogName;

            var orderBy = orderToSortResults!=null? orderToSortResults: new string[] { "PublicationDate DESC" };

            
            //searchQuery.IndexName = AppSettingsUtility.GetValue<string>("NewsCatalogName");
            searchQuery.IndexName = catalogName;
            searchQuery.OrderBy = orderBy;
            searchQuery.Skip = skip;
            searchQuery.Take = take;
            searchQuery.SearchGroup = BuildQuery(categories, providers);

            var resultSet = searchService.Search(searchQuery);

            hitCount = resultSet.HitCount;

            log.InfoFormat("search cate:{0}, limit:{1}, hit:{2}", catalogName, take, hitCount);
            
            return resultSet;
		}

        public static IEnumerable<IDocument> GetNewsDocs(string[] providers, Guid searchIndexGuid, out int hitCount, Guid[] categories = null, string terms = null, int skip = 0, int take = 0, string[] orderToSortResults = null)
        {
            ISearchService searchService = ServiceBus.ResolveService<ISearchService>();
            ISearchQuery searchQuery = ObjectFactory.Resolve<ISearchQuery>();
            var queryBuilder = ObjectFactory.Resolve<IQueryBuilder>();

            //Note: Make sure the Index item has been re-generated via sitfinity's backend to get proper results
            //Guid.Parse("6894B15C-7836-6C70-9642-FF00005F0421")
            var publishingPoint = PublishingManager.GetManager("SearchPublishingProvider")
                                                           .GetPublishingPoint(searchIndexGuid) as PublishingPoint;
            string catalogName = (publishingPoint.PipeSettings.First(p => p.PipeName == "SearchIndex") as SearchIndexPipeSettings).CatalogName;

            var orderBy = orderToSortResults != null ? orderToSortResults : new string[] { "PublicationDate DESC" };


            //searchQuery.IndexName = AppSettingsUtility.GetValue<string>("NewsCatalogName");
            searchQuery.IndexName = catalogName;
            searchQuery.OrderBy = orderBy;
            searchQuery.Skip = skip;
            searchQuery.Take = take;
            searchQuery.SearchGroup = BuildQuery(categories, providers);

            var resultSet = searchService.Search(searchQuery);

            hitCount = resultSet.HitCount;

            log.InfoFormat("search cate:{0}, limit:{1}, hit:{2}", catalogName, take, hitCount);


            return resultSet.SetContentLinks();
        }
        /// <summary>
        /// Sitfinity update after 7.3 requires ISearchQuery Object to be passed into ISearchService. 
        /// In order to accommodate the change, based on prior string definition the required changes
        /// have been meet.
        /// 
        /// Ex.
        ///     Prior setup produced 
        ///                 "(Provider:OpenAccessDataProvider OR Provider:NewsReleaseProvider OR Provider:TodaysHeadlinesProvider) AND (CategoryIds:b97e989c41746fcd88eaff000002c0f4)"
        ///         when passed in
        ///             categories => {System.Guid[1]}
        ///                         [0]: {b97e989c-4174-6fcd-88ea-ff000002c0f4}
        /// 
        ///             providers => {string[3]}
        ///                         [0]: "OpenAccessDataProvider"
        ///                         [1]: "NewsReleaseProvider"
        ///                         [2]: "TodaysHeadlinesProvider"
        ///     
        ///     the breaking changed required the string produced to be changed to ISearchQuery
        ///     which has been converted to SearchQueryGroup.
        /// 
        ///     for more info on the object definition, please refer to sitefinity definition. 
        /// 
        /// </summary>
        /// <param name="categories"> Categories to target </param>
        /// <param name="providers"> Providers to uses </param>
        /// <param name="terms"> Optional terms </param>
        /// <returns></returns>
        public static SearchQueryGroup BuildQuery(Guid[] categories, string[] providers, string terms = null)
		{
            SearchQueryGroup mainSearchQueryGroups = new SearchQueryGroup(QueryOperator.And);

            if (providers != null && providers.Any())
            {
                SearchQueryGroup searchProvideryGroup = new SearchQueryGroup(QueryOperator.Or);
                foreach (var pro in providers)
                {
                    SearchTerm searchTerm = new SearchTerm();
                    searchTerm.Field = "Provider";
                    searchTerm.Value = pro;

                    searchProvideryGroup.AddTerm(searchTerm);
                }
                mainSearchQueryGroups.AddGroup(searchProvideryGroup);
            }

            if (categories != null && categories.Any())
            {
                SearchQueryGroup searchCategoriesGroup = new SearchQueryGroup(QueryOperator.Or);
                foreach (var cat in categories)
                {
                    SearchTerm searchTerm = new SearchTerm();
                    searchTerm.Field = "CategoryIds";
                    searchTerm.Value = cat.ToString().Replace("-", "");

                    searchCategoriesGroup.AddTerm(searchTerm);
                }
                mainSearchQueryGroups.AddGroup(searchCategoriesGroup);
            }

            if (!string.IsNullOrWhiteSpace(terms))
            {
                SearchQueryGroup searchTermGroup = new SearchQueryGroup(QueryOperator.And);
                string[] searchFields = new[] { "Title", "Content", "Summary" };
                foreach (var field in searchFields)
                {
                    SearchTerm searchTerm = new SearchTerm();
                    searchTerm.Field = field;
                    //add wildcards to the query terms
                    searchTerm.Value = string.Join(" ", terms.Split(' ').Where(t => !string.IsNullOrWhiteSpace(t)).Select(term => term + "*").ToList());
                    searchTermGroup.AddTerm(searchTerm);
                }

                mainSearchQueryGroups.AddGroup(searchTermGroup);
            }

            if (categories == null && providers == null && string.IsNullOrEmpty(terms))
            {
                SearchQueryGroup searchAnyGroup = new SearchQueryGroup(QueryOperator.Or);
               // var searchFields = new[] { "Title", "Content" };
                var searchField = "Title";
                var sTerm = "a* b* c* d* e* f* g* h* i* j* k* l* m* n* o* p* q* r* s* t* u* v* w* x* y* z*";

                foreach (var value in sTerm.Split(' '))
                {
                    //SearchTerm searchTerm = new SearchTerm();
                    //searchTerm.Field = "Content";
                    ////add wildcards to the query terms
                    //searchTerm.Value = value;
                    //searchAnyGroup.AddTerm(searchTerm);
                    var searchTerm = new SearchTerm
                    {
                        Field = searchField,
                        Value = value
                    };

                    searchAnyGroup.AddTerm(searchTerm);
                }
                mainSearchQueryGroups.AddGroup(searchAnyGroup);
            }

            return mainSearchQueryGroups;
		}
	}
}