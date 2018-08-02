using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using SitefinityWebApp.Mvc.Models;
using Telerik.Sitefinity.Data.Summary;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Services.Search;
using Telerik.Sitefinity.Services.Search.Data;
using Telerik.Sitefinity.Utilities;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Modules.GenericContent;
using Telerik.Sitefinity.Lists.Model;
using Telerik.Sitefinity.Utilities.TypeConverters;
using Telerik.Sitefinity.Modules.Libraries;
using Telerik.Sitefinity.Taxonomies;
using MatrixGroup.Sitefinity.Config.AppSettings;
using MatrixGroup.Sitefinity.Utilities;
using ServiceStack;
using ServiceStack.Logging;
using Telerik.Microsoft.Practices.EnterpriseLibrary.Logging;
using Telerik.Sitefinity.Taxonomies.Model;


namespace SitefinityWebApp.Custom.ResourceLibrary
{   
    public class ResourceLibraryHelper
    {
        private static ILog log = LogManager.GetLogger(typeof (ResourceLibraryHelper));
             
        public static List<Tuple<string, string, int>> SearchTypes = new List<Tuple<string, string, int>>()
            {
                new Tuple<string, string, int>("News", "News", 1),
                new Tuple<string, string, int>("Events", "Events", 2),
                new Tuple<string, string, int>("Resources", "Resource", 3),
                new Tuple<string, string, int>("Positions", "Position", 4),
                new Tuple<string, string, int>("Policy", "Policy", 5),
                new Tuple<string, string, int>("Ready Set Go Departments", "rsg", 6),
                new Tuple<string, string, int>("Press Releases", "PressRelease", 7),
                new Tuple<string, string, int>("On Scene Articles", "OnSceneArticles", 8),
                new Tuple<string, string, int>("Case Studies", "Case Studies", 9),
                new Tuple<string, string, int>("Marketing Communication Materials", "Marketing Communication Materials", 10),
                new Tuple<string, string, int>("Guides Toolkits and Templates", "Guides Toolkits and Templates", 11),
                new Tuple<string, string, int>("Presentations", "Presentations", 12),
                new Tuple<string, string, int>("Reports and Publications", "Reports and Publications", 13),
                new Tuple<string, string, int>("Research", "Research", 14),
                new Tuple<string, string, int>("SOPs and SOGs", "SOPs and SOGs", 15),
                new Tuple<string, string, int>("Strategy Development Tools", "Strategy Development Tools", 16),
            };

        public static IEnumerable<SearchResult> GetSearchResults_stv(string catalog, SearchCriteria criteria, int skip, int take, int summaryWordCount, out int hitCount)
        {
            var searchService = ServiceBus.ResolveService<ISearchService>();
            var searchQuery = ObjectFactory.Resolve<ISearchQuery>();

            var queryBuilder = ObjectFactory.Resolve<IQueryBuilder>();
            StringBuilder sb = new StringBuilder();
            

            searchQuery.Skip = skip;
            searchQuery.Take = take;
            searchQuery.OrderBy = criteria.OrderBy == null ? new string[] { "PublicationDate DESC" } : new string[] { criteria.OrderBy };
            searchQuery.HighlightedFields = null;
            searchQuery.IndexName = catalog;
            searchQuery.SearchGroup = BuildSearchQuery(criteria, new[] { "Title", "Content", "DocumentText" });

            var resultSet = searchService.Search(searchQuery);

            hitCount = resultSet.HitCount;

            var results = new List<SearchResult>();
            var taxonomyManager = TaxonomyManager.GetManager();

            log.InfoFormat("ResourceLib:{0};", 
                criteria.ToQueryString());

            foreach (var result in resultSet.SetContentLinks())
            {
                var searchResult = new SearchResult();

                try
                {
                    if(!SitefinityExtensions.IsNullOrEmpty(result.GetValue("CategoryIds").ToString()))
                    {
                        var categoryIds = result.GetValue("CategoryIds").ToString().Split(' ').ToList();
                        
                        searchResult.CategoryPair = new List<CategoryPair>();
                        var parentGuidId = AppSettingsUtility.GetValue<Guid>("ResourceLibraryTaxonomyId");
                        
                        foreach (var categoryId in categoryIds)
                        {
                            log.InfoFormat("ResourceLibraryTaxonomyId-cateids:{0}, parentGuidId:{1}", categoryIds, parentGuidId);
                            var category = taxonomyManager.GetTaxon(Guid.Parse(categoryId)) as HierarchicalTaxon;

                            do
                            {
                                if (category.Parent != null && category.Parent.Id == parentGuidId)
                                {
                                    category = taxonomyManager.GetTaxon(Guid.Parse(categoryId)) as HierarchicalTaxon;
                                    var categoryPair = new CategoryPair();
                                    categoryPair.Name = category.Title.Value;
                                    categoryPair.Guid = categoryId.ToString().Replace("-", "");

                                    log.InfoFormat("CateInParent, Title:{0}, guid:{1}", categoryPair.Name, categoryPair.Guid);
                                    sb.Append(String.Format("CateInParent, Title:{0}, guid:{1}", categoryPair.Name,
                                        categoryPair.Guid));
                                    searchResult.CategoryPair.Add(categoryPair);
                                    break;
                                }

                                category = taxonomyManager.GetTaxon(category.Parent.Id) as HierarchicalTaxon;

                            } while (category.Parent != null);

                        }
                    }
                    if (!SitefinityExtensions.IsNullOrEmpty(result.GetValue("OrganizationalAuthorsIds").ToString()))
                    {
                        var categoryIds = result.GetValue("OrganizationalAuthorsIds").ToString().Split(' ').ToList();
                        searchResult.Author = new List<CategoryPair>();
                        var taxonomyGuidId = AppSettingsUtility.GetValue<Guid>("ResourceLibraryTaxonomyAuthorId");

                        foreach (var categoryId in categoryIds)
                        {
                            var category = taxonomyManager.GetTaxon(Guid.Parse(categoryId)) as HierarchicalTaxon;

                            if (category.Taxonomy.Id == taxonomyGuidId)
                            {
                                var categoryPair = new CategoryPair();
                                categoryPair.Name = category.Title.Value;
                                categoryPair.Guid = categoryId.ToString().Replace("-", "");
                                searchResult.Author.Add(categoryPair);

                                log.InfoFormat("ResourceLibraryTaxonomyAuthorId-Title:{0}, guid:{1}", categoryPair.Name, categoryPair.Guid);
                                sb.AppendFormat("ResourceLibraryTaxonomyAuthorId-Title:{0}, guid:{1}", categoryPair.Name,
                                    categoryPair.Guid);
                            }
                        }
                    }
                    if (!SitefinityExtensions.IsNullOrEmpty(result.GetValue("ResourceTypesIds").ToString()))
                    {
                        var categoryIds = result.GetValue("ResourceTypesIds").ToString().Split(' ').ToList();
                        searchResult.ResourceTypes = new List<CategoryPair>();
                        var taxonomyGuidId = AppSettingsUtility.GetValue<Guid>("ResourceLibraryTaxonomyTypeId");

                        foreach (var categoryId in categoryIds)
                        {
                            var category = taxonomyManager.GetTaxon(Guid.Parse(categoryId)) as HierarchicalTaxon;

                            if (category.Taxonomy.Id == taxonomyGuidId)
                            {
                                var categoryPair = new CategoryPair();
                                categoryPair.Name = category.Title.Value;
                                categoryPair.Guid = categoryId.ToString().Replace("-", "");
                                searchResult.ResourceTypes.Add(categoryPair);

                                log.InfoFormat("ResourceLibraryTaxonomyTypeId-Title:{0}, guid:{1}", categoryPair.Name, categoryPair.Guid);
                                sb.AppendFormat("ResourceLibraryTaxonomyTypeId-Title:{0}, guid:{1}", categoryPair.Name, categoryPair.Guid);
                            }
                        }
                    }
                    if (!SitefinityExtensions.IsNullOrEmpty(result.GetValue("Title").ToString()))
                    {
                        searchResult.Title = result.GetValue("Title").ToString();
                    }
                    if (!SitefinityExtensions.IsNullOrEmpty(result.GetValue("Link").ToString()))
                    {
                        searchResult.Link = result.GetValue("Link").ToString();
                    }
                    if (!SitefinityExtensions.IsNullOrEmpty(result.GetValue("PublishDate").ToString()))
                    {
                        searchResult.PublicationDate = DateTime.Parse(result.GetValue("PublishDate").ToString());
                    }
                    if (!SitefinityExtensions.IsNullOrEmpty(result.GetValue("DocumentLibrary").ToString()))
                    {
                        searchResult.DocumentFolder = result.GetValue("DocumentLibrary").ToString();
                    }
                    if (!SitefinityExtensions.IsNullOrEmpty(result.GetValue("Summary").ToString()))
                    {
                        searchResult.Summary = result.GetValue("Summary").ToString();
                    }
                    if (!SitefinityExtensions.IsNullOrEmpty(result.GetValue("ContentType").ToString()))
                    {
                        searchResult.Type = result.GetValue("ContentType").ToString();
                    }

                    if (!SitefinityExtensions.IsNullOrEmpty(result.GetValue("ThumbnailUrl").ToString()))
                    {
                        searchResult.ImageUrl = result.GetValue("ThumbnailUrl").ToString();
                    }
                    else if (!SitefinityExtensions.IsNullOrEmpty(result.GetValue("ImageUrl").ToString()))
                    {
                        searchResult.ImageUrl = result.GetValue("ImageUrl").ToString();
                    }

                    var contentType = TypeResolutionService.ResolveType(searchResult.Type);

                    searchResult.Type = contentType.Name;

                    

                    if (result.Fields.Any(f => f.Name == "EventStart") && !string.IsNullOrWhiteSpace(result.GetValue("EventStart").ToString()))
                    {
                        searchResult.EventStart = DateTime.Parse(result.GetValue("EventStart").ToString());
                    }

                    if (result.Fields.Any(f => f.Name == "EventEnd") && !string.IsNullOrWhiteSpace(result.GetValue("EventEnd").ToString()))
                    {
                        searchResult.EventEnd = DateTime.Parse(result.GetValue("EventEnd").ToString());
                    }
                    var content = result.GetValue("Content").ToString().StripHtmlTags();
                    var docLink = result.GetValue("DocumentLink");
                    var documentText = result.GetValue("DocumentText");
                    if (string.IsNullOrWhiteSpace(content) && !string.IsNullOrWhiteSpace(docLink.ToString()))
                    {
                        searchResult.Link = docLink.ToString();
                        content = documentText.ToString();
                    }

                    if (string.IsNullOrWhiteSpace(searchResult.Summary))
                    {
                        searchResult.Summary = SummaryParser.GetSummary(content, new SummarySettings(SummaryMode.Words, summaryWordCount, true, true));
                    }

                }
                catch (Exception ex)
                {
                    //ex.ToString();
                    log.InfoFormat("Resources-Exception:{0}, details:{1}", ex.Message, sb.ToString());
                }

                results.Add(searchResult);
            }

            return results;
        }


        #region test

        public static IEnumerable<SearchResult> GetSearchResults(string catalog, SearchCriteria criteria, int skip,
            int take, int summaryWordCount, out int hitCount)
        {
            var searchService = ServiceBus.ResolveService<ISearchService>();
            var searchQuery = ObjectFactory.Resolve<ISearchQuery>();

            var queryBuilder = ObjectFactory.Resolve<IQueryBuilder>();
            StringBuilder sb = new StringBuilder();


            searchQuery.Skip = skip;
            searchQuery.Take = take;
            searchQuery.OrderBy = criteria.OrderBy == null ? new string[] { "PublicationDate DESC" } : new string[] { criteria.OrderBy };
            searchQuery.HighlightedFields = null;
            searchQuery.IndexName = catalog;
            searchQuery.SearchGroup = BuildSearchQuery(criteria, new[] { "Title", "Content", "DocumentText" });

            var resultSet = searchService.Search(searchQuery);

            hitCount = resultSet.HitCount;

            var results = new List<SearchResult>();
            var taxonomyManager = TaxonomyManager.GetManager();

            log.InfoFormat("ResourceLib:{0};",
                criteria.ToQueryString());
            
            var searchResult = new SearchResult();

            foreach (IDocument item in resultSet.SetContentLinks() )
            {
                
                try
                {   
                    var fields = item.Fields;
                    searchResult = new SearchResult()
                    {
                        CategoryPair = new List<CategoryPair>(),
                        Author = new List<CategoryPair>(),
                        ResourceTypes = new List<CategoryPair>(),
                        Title = item.GetValue("Title")?.ToString() ?? "",
                        Link = item.GetValue("Link")?.ToString() ?? "",
                        PublicationDate = DateTime.MinValue,
                        DocumentFolder = item.GetValue("DocumentLibrary")?.ToString()??"",
                        Summary = item.GetValue("Summary")?.ToString()??"",
                        Type = item.GetValue("ContentType")?.ToString()??"",
                        ImageUrl = item.GetValue("ThumbnailUrl")?.ToString()??item.GetValue("ImageUrl")?.ToString()??"",
                        EventStart = DateTime.MinValue,
                        EventEnd = DateTime.MinValue
                    };

                    searchResult.Type = TypeResolutionService.ResolveType(searchResult.Type)?.Name ?? "";

                    if (!string.IsNullOrEmpty(item.GetValue("PublishDate")?.ToString()))
                    {
                        DateTime d1 = DateTime.MinValue;
                        DateTime.TryParse(item.GetValue("PublishDate").ToString(), out d1);
                        searchResult.PublicationDate = d1;
                    }
                    if (fields.Any(f => f.Name == "EventStart") && !string.IsNullOrWhiteSpace(item.GetValue("EventStart")?.ToString()))
                    {
                        DateTime d1 = DateTime.MinValue;
                        DateTime.TryParse(item.GetValue("EventStart").ToString(), out d1);
                        searchResult.EventStart = d1;
                    }
                    if (fields.Any(f => f.Name == "EventEnd") && !string.IsNullOrWhiteSpace(item.GetValue("EventEnd")?.ToString()))
                    {
                        DateTime d1 = DateTime.MinValue;
                        DateTime.TryParse(item.GetValue("EventEnd").ToString(), out d1);
                        searchResult.EventEnd = d1;
                    }
                    /*if (string.IsNullOrWhiteSpace(searchResult.Summary))
                    {
                        searchResult.Summary = SummaryParser.GetSummary(content, new SummarySettings(SummaryMode.Words, summaryWordCount, true, true));
                    }*/

                    sb.AppendFormat("getCategories");
                    if (fields.Any(f => f.Name == "CategoryIds"))
                    {
                        var parentGuidId = AppSettingsUtility.GetValue<Guid>("ResourceLibraryTaxonomyId");
                        item.GetValue("CategoryIds")?.ToString().Split(' ')?
                            .ToList()
                            .ForEach(m =>
                            {
                                if (m.IsGuid())
                                {
                                    var category = taxonomyManager.GetTaxon(Guid.Parse(m)) as HierarchicalTaxon;
                                    //while (category?.Parent != null)
                                    //{
                                        if (category?.Parent?.Id == parentGuidId)
                                        {
                                            searchResult.CategoryPair.Add(new CategoryPair()
                                            {
                                                Name = category.Title.Value,
                                                Guid = m.Replace("-", string.Empty)
                                            });
                                            category = taxonomyManager.GetTaxon(category.Parent.Id) as HierarchicalTaxon;
                                        }
                                        
                                    //};
                                }
                            });
                    }

                    sb.AppendFormat("getAuthors");
                    if (fields.Any(f => f.Name == "OrganizationalAuthorsIds") && !string.IsNullOrEmpty(item.GetValue("OrganizationalAuthorsIds")?.ToString()))
                    {
                        item.GetValue("OrganizationalAuthorsIds").ToString().Split(' ').ToList()
                            .ForEach(m =>
                            {
                                var taxonomyGuidId = AppSettingsUtility.GetValue<Guid>("ResourceLibraryTaxonomyAuthorId");
                                if (m.IsGuid())
                                {
                                    var category = taxonomyManager.GetTaxon(Guid.Parse(m)) as HierarchicalTaxon;
                                    if (category?.Taxonomy?.Id != null && category.Taxonomy.Id == taxonomyGuidId)
                                    {
                                        searchResult.Author.Add(new CategoryPair()
                                        {
                                            Name = category.Title.Value,
                                            Guid = m.Replace("-", string.Empty)
                                        });
                                    }
                                }
                                
                            });
                    }

                    sb.AppendFormat("getTypes");
                    if (fields.Any(f => f.Name == "ResourceTypesIds") &&
                        !string.IsNullOrEmpty(item.GetValue("ResourceTypesIds")?.ToString()))
                    {
                        item.GetValue("ResourceTypesIds").ToString().Split(' ').ToList()
                            .ForEach(m =>
                            {
                                var taxonomyGuidId = AppSettingsUtility.GetValue<Guid>("ResourceLibraryTaxonomyTypeId");
                                if (m.IsGuid())
                                {
                                    var category = taxonomyManager.GetTaxon(Guid.Parse(m)) as HierarchicalTaxon;
                                    if (category?.Taxonomy?.Id != null && category.Taxonomy.Id == taxonomyGuidId)
                                    {
                                        searchResult.ResourceTypes.Add(new CategoryPair()
                                        {
                                            Name = category.Title.Value,
                                            Guid = m.Replace("-", string.Empty)
                                        });
                                    }
                                }
                                
                            });
                    }
                        
                        results.Add(searchResult);
                    log.InfoFormat("Resources-result:{0} item:{1} \r", sb.ToString(), StringExtensions.ToJson(searchResult));
                }
                catch (Exception ex)
                {
                   log.InfoFormat("testfailed:{0}, details:{1}", ex.Message, sb.ToString());
                }
            }

            return results;
        }
        #endregion

        #region BuildSearchQuery

        public static SearchQueryGroup BuildSearchQuery(SearchCriteria model, string[] searchFields)
        {
            //GOAL: (Title:[term]* Content:[term*]) AND (Title:[secondTerm]* Content:[secondTerm]*) AND (Type:[t1] OR Type:[t2] ... OR Type:[tN]) AND (Category:[c1] ... OR Category:[cN])
            //log.Trace("Keyword: {0}", keyword);
            var queryGroups = new SearchQueryGroup(QueryOperator.And);

            //var queryGroups = new List<string>();

            //**** TERMS ****
            var documentLibraryToSearch = AppSettingsUtility.GetValue<string>("ResourceLibrary.DocumentLibrary.ToSearch");
            if (!string.IsNullOrEmpty(documentLibraryToSearch))
            {
                var SearchDocumentLibraryGroup = new SearchQueryGroup(QueryOperator.Or);
                var documentLibraries = documentLibraryToSearch.Split(',').ToList();

                foreach (var documentLibrary in documentLibraries)
                {
                    var searchTerm = new SearchTerm();
                    searchTerm.Field = "DocumentLibrary";
                    searchTerm.Value = documentLibrary;
                    SearchDocumentLibraryGroup.AddTerm(searchTerm);
                }

                queryGroups.AddGroup(SearchDocumentLibraryGroup);
            }

            //**** TERMS ****
            if (!string.IsNullOrEmpty(model.Term))
            {
                var SearchTermGroup = new SearchQueryGroup(QueryOperator.Or);
                var keyword = model.Term + " " + model.SecondTerm;

                foreach (var field in searchFields)
                {
                    var keywords = keyword.Split(' ');
                    var count = keywords.Count();

                    for (int i = 0; i < count; i++)
                    {
                        if (!SitefinityExtensions.IsNullOrEmpty(keywords[i]))
                        {
                            var searchTerm = new SearchTerm();
                            searchTerm.Field = field;
                            //log.Trace("Search Term Field: {0}", searchTerm.Field);
                            searchTerm.Value = keywords[i];
                            //log.Trace("Search Term Value: {0}", searchTerm.Value);
                            SearchTermGroup.AddTerm(searchTerm);
                        }
                    }
                }

                queryGroups.AddGroup(SearchTermGroup);
            }

            //**** TYPES *****
            if (model.Types != null && model.Types.Any())
            {
                var searchTypeGroup = new SearchQueryGroup(QueryOperator.Or);
                foreach (var type in model.Types)
                {
                        var searchTerm = new SearchTerm();
                        searchTerm.Field = "ResourceTypesIds";
                        searchTerm.Value = type;

                        searchTypeGroup.AddTerm(searchTerm);
                }
                queryGroups.AddGroup(searchTypeGroup);
            }

            //**** TOPICS ****
            if (model.Topics != null && model.Topics.Any())
            {
                var searchCategoriesGroup = new SearchQueryGroup(QueryOperator.Or);
                foreach (var cat in model.Topics)
                {
                    var searchTerm = new SearchTerm();
                    searchTerm.Field = "CategoryIds";
                    searchTerm.Value = cat.ToString().Replace("-", "");

                    searchCategoriesGroup.AddTerm(searchTerm);
                }
                queryGroups.AddGroup(searchCategoriesGroup);
            }

            //**** Authors ****
            if (model.Authors != null && model.Authors.Any())
            {
                var searchAuthorsGroup = new SearchQueryGroup(QueryOperator.Or);
                foreach (var cat in model.Authors)
                {
                    var searchTerm = new SearchTerm();
                    searchTerm.Field = "OrganizationalAuthorsIds";
                    searchTerm.Value = cat.ToString().Replace("-", "");

                    searchAuthorsGroup.AddTerm(searchTerm);
                }
                queryGroups.AddGroup(searchAuthorsGroup);
            }

            if (model.Types == null && model.Topics == null && string.IsNullOrEmpty(model.Term) && string.IsNullOrEmpty(model.SecondTerm))
            {
                SearchQueryGroup SearchAnyGroup = new SearchQueryGroup(QueryOperator.Or);
                var searchField = "Title";
                var sTerm = "a* b* c* d* e* f* g* h* i* j* k* l* m* n* o* p* q* r* s* t* u* v* w* x* y* z*";

                foreach (var term in sTerm.Split(' '))
                {
                    var searchTerm = new SearchTerm
                    {
                        Field = searchField,
                        Value = term
                    };
                    //add wildcards to the query terms
                    SearchAnyGroup.AddTerm(searchTerm);
                }
                queryGroups.AddGroup(SearchAnyGroup);
            }

            return queryGroups;
        }

        #endregion
    }
}