using MatrixGroup.Sitefinity.Config.AppSettings;
using SitefinityWebApp.Mvc.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Telerik.Microsoft.Practices.EnterpriseLibrary.Common.Utility;
using Telerik.Sitefinity.Data.Summary;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Services.Search;
using Telerik.Sitefinity.Services.Search.Model;
using Telerik.Sitefinity.Services.Search.Data;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Taxonomies.Model;
using Telerik.Sitefinity.Utilities;

namespace SitefinityWebApp.Custom.ContentList
{
    public class ContentListHelper
    {
        public static List<Tuple<string, string, int>> _searchTypes = null;
        
        public static List<Tuple<string, string, int>> SearchTypes()
        {
            return SearchTypes(AppSettingsUtility.GetValue<string>("ContentListContentTypeFilterMapList"));
        }

        public static List<Tuple<string, string, int>> SearchTypes(string ContentTypeMapList)
            {
                if (_searchTypes == null || true){
                    var contentTypes = ContentTypeMapList.Split(',');

                    _searchTypes = new List<Tuple<string, string, int>>();
                        var typeOrder = 1;
                    foreach(var contentType  in contentTypes ){
                        var aType = contentType.Split(':');
                        _searchTypes.Add (new Tuple<string, string, int>(aType[0],aType[1], typeOrder++) );
                    }
                }

                return _searchTypes;
            }

        private static List<Dictionary<string, string>> _searchTopics = null;
        public static List<Dictionary<string, string>> Topics ( string FilterTaxaIdList )
        {
                if (_searchTopics == null)
                {
                    _searchTopics = new List<Dictionary<string, string>>();

                    var taxaIds = FilterTaxaIdList.Split(',');

                    foreach (var taxaId in taxaIds)
                    {
                        var taxaGuid = Guid.Parse(taxaId);

                        var aTopic = new Dictionary<string, string>();
                        TaxonomyManager.GetManager()
                                       .GetTaxa<HierarchicalTaxon>()
                                       .Where(t => t.Parent.Id == taxaGuid)
                                       .OrderBy(t => t.Ordinal)
                                       .ForEach(t => aTopic.Add(t.Id.ToString().Replace("-", ""), t.Title.ToString()));
                        if (aTopic != null) _searchTopics.Add(aTopic);
                    }
                }

                return _searchTopics;
        }

        public static IEnumerable<ContentListSearchResult> GetSearchResults(string catalog, ContentListSearchCriteria criteria, int skip, int take, int summaryWordCount, out int hitCount)
        {
            var searchService = ServiceBus.ResolveService<LuceneSearchService>();

			var searchQuery = new SearchQuery();
            if (String.IsNullOrWhiteSpace(criteria.Term) && criteria.Topics == null && criteria.Types == null && string.IsNullOrEmpty(criteria.StartDate) )
            {
				searchQuery.Text = "a* OR b* OR c* OR d* OR e* OR f* OR g* OR h* OR i* OR j* OR k* OR l* OR m* OR n* OR o* OR p* OR r* OR s* OR t* OR u* OR v* OR w*";
				searchQuery.SearchFields = new[] { "Title" };

                if (string.IsNullOrEmpty(criteria.OrderBy))
                {
                    criteria.OrderBy = "PublishDate DESC";
                }
            }
            else
            {
				BuildSearchQuery(out searchQuery, criteria, criteria.SearchFieldNames);
            }

			searchQuery.IndexName = catalog;
			searchQuery.HighlightedFields = null;
			searchQuery.Skip = skip;
			searchQuery.Take = take;
			searchQuery.OrderBy = criteria.OrderBy == null ? null : new string[] { criteria.OrderBy, "PublishDate DESC" };

			var resultSet = searchService.Search(searchQuery);


            hitCount = resultSet.HitCount;

            var results = new List<ContentListSearchResult>();

            foreach (var result in resultSet.SetContentLinks())
            {
                var searchResult = new ContentListSearchResult();
                foreach (var fieldName in criteria.FieldsToShow)
                {
                    switch (fieldName)
                    {
                        case "Title":
                            searchResult.Title = result.GetValue("Title");
                            break;

                        case "PublicationDate":
                            searchResult.PublicationDate = DateTime.Parse(result.GetValue("PublicationDate")).ToLocalTime();
                            break;

                        case "Link":
                            searchResult.Link = result.GetValue("Link");
                            break;
                        case "Type":
                            var type = result.GetValue("ContentType");
                            searchResult.Type = _searchTypes.Where(t => t.Item2 == type).FirstOrDefault().Item1;
                            break;
                        default:
                            searchResult.Summary = result.GetValue(fieldName);
                            if (string.IsNullOrWhiteSpace(searchResult.Summary))
                            {
                                var content = result.GetValue("Content").StripHtmlTags();
                                searchResult.Summary = SummaryParser.GetSummary(content, new SummarySettings(SummaryMode.Words, summaryWordCount, true, true));
                            }
                            break;


                    }
                }
                results.Add(searchResult);
            }

            return results;
        }

        public static void BuildSearchQuery(out SearchQuery query, ContentListSearchCriteria model, string[] searchFields)
        {
            //GOAL: (Title:[term]* Content:[term*]) AND (Title:[secondTerm]* Content:[secondTerm]*) AND (Type:[t1] OR Type:[t2] ... OR Type:[tN]) AND (Category:[c1] ... OR Category:[cN])
			query.SearchFields = new[] {"Title", "Content", "Type", }
            var queryGroups = new List<string>();

            //**** TERMS ****
            if (!string.IsNullOrEmpty(model.Term))
            {
                //build the format string e.g. (Title:{0} AND Content:{0})
                var formats = searchFields.Select(f => f + ":{0}");
                var fullFormat = "(" + string.Join(" ", formats) + ")";

                //add wildcards to the query terms
                var queryTerms = string.Concat(model.Term, " ", model.SecondTerm);
                var tokenized =
                    queryTerms.Split(' ').Where(t => !string.IsNullOrWhiteSpace(t)).Select(term => term + "*");

                //build the query groups
                var formattedTerms = string.Join(" AND ", tokenized.Select(t => string.Format(fullFormat, t)));

                queryGroups.Add(formattedTerms);
            }

            //**** DATES ****
            if ( !string.IsNullOrEmpty(model.StartDate)  )
            {

                //build the query groups
                var formattedTerms = String.Format("PublicationDate:'{0}'",model.StartDate);
                //var formattedTerms = string.Join(" AND ", "PublicationDate:'3/6/2013'");

                queryGroups.Add(formattedTerms);
            }

            //**** TYPES *****
            if (model.Types != null && model.Types.Any())
            {
                var typeFormat = "ContentType:{0}*";
                var typeQueries =
                    model.Types.Select(t => string.Format(typeFormat, _searchTypes.First(ty => ty.Item1 == t || ty.Item2 == t).Item2));
                var formattedTypes = "(" + string.Join(" OR ", typeQueries) + ")";

                queryGroups.Add(formattedTypes);
            }

            //**** TOPICS ****
            if (model.Topics != null && model.Topics.Any())
            {
                var topicFormat = "CategoryIds:{0}";
                var topicQueries = model.Topics.Select(t => string.Format(topicFormat, t));
                var formattedTopics = "(" + string.Join(" OR ", topicQueries) + ")";

                queryGroups.Add(formattedTopics);
            }

            return string.Join(" AND ", queryGroups);
        }
    }
}