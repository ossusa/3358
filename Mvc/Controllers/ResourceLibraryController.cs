using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using MatrixGroup.Sitefinity.Config.AppSettings;
using MatrixGroup.Sitefinity.Utilities;
using ServiceStack;
using ServiceStack.Logging;
using SitefinityWebApp.Custom.ResourceLibrary;
using Telerik.Microsoft.Practices.EnterpriseLibrary.Common.Utility;
using Telerik.Sitefinity.Data.Summary;
using Telerik.Sitefinity.Modules;
using Telerik.Sitefinity.Modules.Blogs;
using Telerik.Sitefinity.Mvc;
using SitefinityWebApp.Mvc.Models;
using Telerik.Sitefinity.Publishing;
using Telerik.Sitefinity.Publishing.Model;
using Telerik.Sitefinity.Publishing.Web.Services;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Services.Search;
using Telerik.Sitefinity.Services.Search.Data;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Taxonomies.Model;
using Telerik.Sitefinity.Web;
using Telerik.Sitefinity.Utilities;

namespace SitefinityWebApp.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "ResourceLibrary", Title = "ResourceLibrary", SectionName = "Matrix")]
    public class ResourceLibraryController : Controller
    {
        private int _itemsPerPage = 15;
        private string _defaultSortExpression = "PublicationDate DESC";
        private int _summaryWordCount = 25;
        private ILog log = LogManager.GetLogger(typeof (ResourceLibraryController));

        protected Guid CatalogId
        {
            get { return AppSettingsUtility.GetValue<Guid>("ResourceLibraryCatalogId"); }
        }

        [Category("Matrix")]
        public int ItemsPerPage
        {
            get { return _itemsPerPage; }
            set { _itemsPerPage = value; }
        }

        [Category("Matrix")]
        public string DefaultSortExpression
        {
            get { return _defaultSortExpression; }
            set { _defaultSortExpression = value; }
        }

        [Category("Matrix")]
        public int SummaryWordCount
        {
            get { return _summaryWordCount; }
            set { _summaryWordCount = value; }
        }

        private String _catalog = null;
        protected String CatalogName
        {
            get
            {
                if (_catalog == null)
                {
                    var publishingPoint = PublishingManager.GetManager("SearchPublishingProvider")
                                                           .GetPublishingPoint(CatalogId) as PublishingPoint;
                    _catalog = (publishingPoint.PipeSettings.First(p => p.PipeName == "SearchIndex") as SearchIndexPipeSettings).CatalogName;

                }
                return _catalog;
            }
        }

        #region Overrides of Controller

        protected override void HandleUnknownAction(string actionName="")
        {
            log.InfoFormat("HandleUnknownAction-ResourceLibrary:ask for:{0}, but use Index.cshtml", actionName);
            if (string.IsNullOrEmpty(actionName))
            {
                this.View(new EmptyResult()).ExecuteResult(ControllerContext);
            }
            var model = new ResultsViewModel();
            model.Criteria = new SearchCriteria();
            model.Criteria.TopicsList = new SelectList(Topics, "Key", "Value");
            model.Criteria.AuthorsList = new SelectList(Authors, "Key", "Value");
            model.Criteria.TypesList = new SelectList(Types, "Key", "Value");
            //model.Criteria.TypesList = new SelectList(ResourceLibraryHelper.SearchTypes, "Item2", "Item1");
            

            int hitCount;
            model.Results = new List<SearchResult>(); //ResourceLibraryHelper.GetSearchResults(CatalogName, null, ItemsPerPage , ItemsPerPage, SummaryWordCount, out hitCount).ToList();
            model.Pagination = new Pagination() { RouteValues = model.Criteria, CurrentPage = 1, ItemsPerPage = ItemsPerPage, TotalItems = 0 };

            this.View("Index", model).ExecuteResult(ControllerContext);
        }

        #endregion

        /*public ActionResult Index(SearchCriteria criteria = null, int page = 1)
        {
            var model = new ResultsViewModel();
            model.Criteria = criteria;
            model.Criteria.TopicsList = new SelectList(Topics, "Key", "Value");
            model.Criteria.AuthorsList = new SelectList(Authors, "Key", "Value");
            model.Criteria.TypesList = new SelectList(Types, "Key", "Value");
            //model.Criteria.TypesList = new SelectList(ResourceLibraryHelper.SearchTypes, "Item2", "Item1");

            if (criteria != null && criteria.OrderBy == null)
            {
                criteria.OrderBy = DefaultSortExpression;
            }

            int hitCount;
            var sresult = ResourceLibraryHelper.GetSearchResults(CatalogName, criteria, ItemsPerPage * (page - 1), ItemsPerPage, SummaryWordCount, out hitCount).ToList();

            model.Results = new List<SearchResult>()
            {
                new SearchResult()
                {
                    Title = String.Format("test: CatalogName:{0}, crit:{1}", CatalogName, criteria?.ToQueryString()),
                    CategoryPair = new List<CategoryPair>(),
                    Summary = string.Format("Topic:{0}, Authors:{1}, Types:{2}", 
                    Topics.Keys.Join(","), Authors.Keys.Join(","),Types.Keys.Join(",") )
                , Link = string.Format("result:{0}", sresult.Any()?sresult.Select(c => c.Title).ToList().Join(","):" empty"),
                    PublicationDate = DateTime.MinValue, ResourceTypes = new List<CategoryPair>(), Type = "", EventStart = DateTime.MinValue, EventEnd = DateTime.MinValue, DocumentFolder = "", ImageUrl = "", Author = new List<CategoryPair>(), DocumentParentFolders = new ArrayOfString()
                
                }
            };
            model.Pagination = new Pagination()
            {
                RouteValues = model.Criteria, CurrentPage = page, ItemsPerPage = ItemsPerPage, TotalItems = 1
            };
            
            return View(model);
        }*/

        /// <summary>
        /// This is the default Action.
        /// </summary>
        public ActionResult Index(SearchCriteria criteria = null, int page = 1)
        {
            log.InfoFormat("ResourceLibCriteria:{0}", criteria?.Term );

            var model = new ResultsViewModel();
            model.Criteria = criteria;
            model.Criteria.TopicsList = new SelectList(Topics, "Key", "Value");
            model.Criteria.AuthorsList = new SelectList(Authors, "Key", "Value");
            model.Criteria.TypesList = new SelectList(Types, "Key", "Value");
            //model.Criteria.TypesList = new SelectList(ResourceLibraryHelper.SearchTypes, "Item2", "Item1");

            if (criteria != null && criteria.OrderBy == null)
            {
                criteria.OrderBy = DefaultSortExpression;
            }

            int hitCount;
            model.Results = ResourceLibraryHelper.GetSearchResults(CatalogName, criteria, ItemsPerPage * (page - 1), ItemsPerPage, SummaryWordCount, out hitCount).ToList();
            model.Pagination = new Pagination() { RouteValues = model.Criteria, CurrentPage = page, ItemsPerPage = ItemsPerPage, TotalItems = hitCount };
            log.InfoFormat("hit:{0}, title:{1}", hitCount, model.Results?.Select(c => c.Title).Join(","));
            return View(model);
            /*var model = new ResultsViewModel();
            model.Criteria = criteria;
            model.Criteria.TopicsList = new SelectList(Topics, "Key", "Value");
            model.Criteria.AuthorsList = new SelectList(Authors, "Key", "Value");
            model.Criteria.TypesList = new SelectList(Types, "Key", "Value");
            //model.Criteria.TypesList = new SelectList(ResourceLibraryHelper.SearchTypes, "Item2", "Item1");
            int hitCount;
            if (criteria != null && criteria.OrderBy == null)
            {
                criteria.OrderBy = DefaultSortExpression;
                try
                {
                    model.Results = ResourceLibraryHelper.GetSearchResults(CatalogName, criteria, ItemsPerPage * (page - 1), ItemsPerPage, SummaryWordCount, out hitCount).ToList();
                    model.Pagination = new Pagination() { RouteValues = model.Criteria, CurrentPage = page, ItemsPerPage = ItemsPerPage, TotalItems = hitCount };
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("ResourceLibToModel:{0}", ex.Message);
                    model.Results = new List<SearchResult>();
                    model.Pagination = new Pagination() { RouteValues = model.Criteria, CurrentPage = page, ItemsPerPage = ItemsPerPage, TotalItems = 0 };
                }

                //model.Results = ResourceLibraryHelper.TestGetSearchResults(CatalogName, criteria, ItemsPerPage * (page - 1), ItemsPerPage, SummaryWordCount, out hitCount).ToList();


                return View(model);
            }

            model.Results = ResourceLibraryHelper.GetSearchResults(CatalogName, criteria, ItemsPerPage * (page - 1), ItemsPerPage, SummaryWordCount, out hitCount).ToList();//new List<SearchResult>();
            model.Pagination = new Pagination() { RouteValues = model.Criteria, CurrentPage = page, ItemsPerPage = ItemsPerPage, TotalItems = 0 };
            return View(model);*/
        }

        [HttpPost]
        [ActionName("Index")]
        public ActionResult IndexPost(SearchCriteria criteria)
        {
            log.InfoFormat("postToResourceLibrary:{0}", criteria == null);
            var queryString = criteria.ToQueryString();
            var requestUrl = SiteMapBase.GetActualCurrentNode().Url;

            return Redirect(requestUrl + "?" + queryString);
        }

        private Dictionary<string, string> _searchTopics = null;
        private Dictionary<string, string> Topics
        {
            get
            {
                if (_searchTopics == null)
                {
                    _searchTopics = new Dictionary<string, string>();

                    var taxonomyManager = TaxonomyManager.GetManager();

                    var parentId = AppSettingsUtility.GetValue<Guid>("ResourceLibraryTaxonomyId");
                    try
                    {
                        var taxa = taxonomyManager.GetTaxa<HierarchicalTaxon>()
                                       .Where(t => t.Parent.Id == parentId)
                                       .OrderBy(t => t.Ordinal);
                                       //.ForEach(t => _searchTopics.Add(t.Id.ToString().Replace("-", ""), t.Title.ToString()));

                        foreach (var taxon in taxa)
                        {
                            _searchTopics.Add(taxon.Id.ToString().Replace("-", ""), taxon.Title.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                }

                return _searchTopics;
            }
        }

        private Dictionary<string, string> _searchTypes = null;
        private Dictionary<string, string> Types
        {
            get
            {
                if (_searchTypes == null)
                {
                    _searchTypes = new Dictionary<string, string>();

                    var taxonomyManager = TaxonomyManager.GetManager();

                    var taxonomyId = AppSettingsUtility.GetValue<Guid>("ResourceLibraryTaxonomyTypeId");
                    try
                    {
                        var taxa = taxonomyManager.GetTaxa<HierarchicalTaxon>()
                                       .Where(t => t.Taxonomy.Id == taxonomyId)
                                       .OrderBy(t => t.Ordinal);
                        //.ForEach(t => _searchTopics.Add(t.Id.ToString().Replace("-", ""), t.Title.ToString()));

                        foreach (var taxon in taxa)
                        {
                            _searchTypes.Add(taxon.Id.ToString().Replace("-", ""), taxon.Title.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                }

                return _searchTypes;
            }
        }

        private Dictionary<string, string> _searchAuthors = null;
        private Dictionary<string, string> Authors
        {
            get
            {
                if (_searchAuthors == null)
                {
                    _searchAuthors = new Dictionary<string, string>();

                    var taxonomyManager = TaxonomyManager.GetManager();

                    var taxonomyId = AppSettingsUtility.GetValue<Guid>("ResourceLibraryTaxonomyAuthorId");
                    try
                    {
                        var taxa = taxonomyManager.GetTaxa<HierarchicalTaxon>()
                                       .Where(t => t.Taxonomy.Id == taxonomyId)
                                       .OrderBy(t => t.Ordinal);
                        //.ForEach(t => _searchTopics.Add(t.Id.ToString().Replace("-", ""), t.Title.ToString()));

                        foreach (var taxon in taxa)
                        {
                            _searchAuthors.Add(taxon.Id.ToString().Replace("-", ""), taxon.Title.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                }

                return _searchAuthors;
            }
        }



    }
}