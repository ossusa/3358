using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.Runtime.Caching;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Taxonomies.Model;
using Telerik.Sitefinity.Services.Search.Data;
using SitefinityWebApp.Mvc.Models;
using System.Globalization;
using CsQuery.ExtensionMethods;
using Telerik.Sitefinity.Data.Summary;
using NLog;
using SitefinityWebApp.Custom.News;
using Telerik.Sitefinity;
using Telerik.Sitefinity.Modules.News;
using Telerik.Sitefinity.News.Model;
using Telerik.Sitefinity.Modules.Blogs;
using Telerik.Sitefinity.DynamicModules;
using Telerik.Sitefinity.Utilities.TypeConverters;
using Telerik.Sitefinity.DynamicModules.Model;
using MatrixGroup.Sitefinity.Config.AppSettings;
using ServiceStack;


namespace SitefinityWebApp.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "RelatedContentForDynamicContent", Title = "Related Content For Dynamic Content", SectionName = "Custom")]
    public class RelatedContentForDynamicContentController : Controller
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        #region widget settings

        private int _numberOfPosts = 5;
        [Category("General")]
        public int NumberOfPosts { get { return this._numberOfPosts; } set { this._numberOfPosts = value; } }

        private string _viewTemplate
        {
            get
            {
                if (SitefinityExtensions.IsNullOrEmpty(ViewTemplate))
                {
                    return "RelatedContentForDynamicContent";
                }

                return ViewTemplate;
            }
        }
        [Category("General")]
        public string ViewTemplate { get; set; }

        private Guid _searchIndex
        {
            get
            {
                var searchIndexes = AppSettingsUtility.GetValue<string>("SearchIndex.Guids");
                var searchIndexPairs = searchIndexes.Split(':').ToList();

                foreach (var searchIndexPair in searchIndexPairs)
                {
                    //var searchIndexName = searchIndexPair.Split('|').First();
                    var kv = searchIndexPair.Split('|');
                    if (kv.Length > 0)
                    {
                        if (!String.IsNullOrEmpty(SearchIndex) && kv[0].ToUpper() == SearchIndex.ToUpper() && kv[1].IsGuid())
                        {
                            return Guid.Parse(kv[1]);
                        }
                    }

                    /*if (SearchIndex != null && SearchIndex.ToUpper() == searchIndexName.ToUpper())
                    {
                        return Guid.Parse(searchIndexPair.Split('|').Last());
                    }*/
                }

                return AppSettingsUtility.GetValue<Guid>("NewsCatalogGuid");
            }
        }
        [Category("General")]
        public string SearchIndex { get; set; }

        private string[] Providers
        {
            get
            {
                if (string.IsNullOrEmpty(ProviderNames))
                {
                    return null;
                }

                return Enumerable.ToArray(ProviderNames.Split(','));
            }
        }
        [Category("General")]
        public string ProviderNames { get; set; }

        #endregion

        private static ObjectCache cache = MemoryCache.Default;

        //**************Actions**************//
        //***********************************//
        //***********************************//
        #region HandleUnknownAction

        public ActionResult Index()
        {
            log.Info("Related in Index");
            return View("RelatedContentForDynamicContent", GetContentFor());
        }

        protected void HandleUnknownAction_tt(string actionName = "")
        {
            string template = String.IsNullOrEmpty(_viewTemplate)? "RelatedContentForDynamicContent" : _viewTemplate;
            List<BlendedListItem> blendedListItems = new List<BlendedListItem>()
            {
                new BlendedListItem()
                {
                    Categories = new List<CategoryPair>()
                    {
                        new CategoryPair() {Guid = Guid.Empty.ToString(), Name = "test"}
                    },
                    Title = "Test tt",
                    LocationStreet = "street",
                    LocationState = "state",
                    LocationCity = "city",
                    Summary = "summ",
                    Content = "Test content",
                    Link = "x",
                    EventStartDate = DateTime.MinValue,
                    EventEndDate = DateTime.MinValue,
                    PublicationDate = DateTime.MinValue,
                    Featured = false,
                    ResourceTypes = new List<CategoryPair>(),
                    OrganizationalAuthors = new List<CategoryPair>(),
                    DefaultLinkBase = "",
                    SelfPaced = false,
                    DisplayDate = DateTime.MinValue.ToString(),
                    Image = "",
                    Protected = false,
                    
                    ContentType = "",
                }
            };
            log.Error("teamplatename: {0}", template);

            

            this.View(template, blendedListItems).ExecuteResult(ControllerContext);
        }

        protected  override void HandleUnknownAction(string actionName="")
        {
            try
            {
                log.Info("Begin HandleUnknownAction. view:{0}", _viewTemplate);
                var dynamicModuleManager = DynamicModuleManager.GetManager();
                /*if (string.IsNullOrEmpty(_viewTemplate))
                {
                    this.View(new EmptyResult()).ExecuteResult(ControllerContext);
                    
                }*/

                var dynamicTypes = new List<Type>();
                var dynamicTypeNames = AppSettingsUtility.GetValue<string>("RelatedItems.DynamicType.List").Split('|').ToList();

                foreach (var dynamicTypeName in dynamicTypeNames)
                {
                    if (!string.IsNullOrWhiteSpace(dynamicTypeName))
                    {
                        dynamicTypes.Add(TypeResolutionService.ResolveType("Telerik.Sitefinity.DynamicTypes.Model." + dynamicTypeName));
                    }
                    
                }
                // set default
                if (dynamicTypes.Count == 0)
                {
                    dynamicTypes.Add(TypeResolutionService.ResolveType("Telerik.Sitefinity.DynamicTypes.Model.OnSceneArticles.OnSceneArticle"));
                }


                var taxonomyManager = TaxonomyManager.GetManager();
                var currentContext = System.Web.HttpContext.Current;
                var postUrl = currentContext.Request.Url.AbsolutePath;

                if (postUrl.Contains('?'))
                {
                    postUrl = postUrl.Split('?').First();
                    log.Trace("postUrl: {0}", SitefinityExtensions.IsNullOrEmpty(postUrl) ? "There is no url available" : postUrl);
                }

                postUrl = currentContext.Request.Url.AbsolutePath.Split('/').Last();
                log.Debug("postUrl: {0}", SitefinityExtensions.IsNullOrEmpty(postUrl) ? "There is no url available" : postUrl);

                foreach (var dynamicType in dynamicTypes)
                {
                    var item = dynamicModuleManager.GetDataItems(dynamicType)
                        .FirstOrDefault(bp => bp.UrlName == postUrl && bp.Status == Telerik.Sitefinity.GenericContent.Model.ContentLifecycleStatus.Live);

                    if (item != null)
                    {
                        //log.Debug("item: {0}", SitefinityExtensions.IsNullOrEmpty(item.GetValue<Lstring>("Title").Value) ? "There is no title set" : item.GetValue<Lstring>("Title").Value);
                        //---item = dynamicModuleManager.Lifecycle.GetLive(item) as DynamicContent;
                        //log.Trace("Got the live version of the Blog Post.");
                        try
                        {
                            //var allCategories = Enumerable.ToArray(item.GetValue<IList<Guid>>("Category"));
                            var allCategories = item.GetValue<IList<Guid>>("Category")?.ToArray();
                            //log.Debug("Got a list of category ids for the post, {0}.", allCategories.Count());
                            List<Guid> categories = new List<Guid>();
                            
                            //log.Trace("Set the list for Blended List Query.");
                            int hitCount = 0;
                            //log.Trace("Set the hitcount variable required for Sitefinity Search.");
                            if (allCategories != null && allCategories.Any())
                            {
                                foreach (var currGuid in allCategories)
                                {
                                    var currTaxon = taxonomyManager.GetTaxon<HierarchicalTaxon>(currGuid);

                                    if (currTaxon != null)
                                    {
                                        //log.Debug("Got taxonomy, {0}, for the id, {1}", SitefinityExtensions.IsNullOrEmpty(currTaxon.Title.Value) ? "There is no title set" : currTaxon.Title.Value);
                                        categories.Add(currGuid);
                                        //log.Trace("Taxon added to the list for the search.");
                                    }
                                }
                            }

                            IEnumerable<IDocument> resultListItems = BlendedNewsHelper.GetNewsDocs(Providers, _searchIndex, out hitCount,
                                categories?.ToArray(), null, 0, this.NumberOfPosts + 10);
                            
                            List<BlendedListItem> blendedListItems = new List<BlendedListItem>();
                            if (resultListItems != null)
                            {
                                var rs = SetBlendedListItems(resultListItems);
                                if (rs != null)
                                {
                                    blendedListItems = rs
                                    .Where(bli => !bli.Link.Contains(item.UrlName) &&
                                                  !bli.Featured).Take(this.NumberOfPosts).ToList();
                                }

                            }
                            //string template = _viewTemplate;
                            //log.Info("Finishing HandleUnknownAction");
                            this.View(_viewTemplate, blendedListItems).ExecuteResult(ControllerContext);

                        }
                        catch (Exception ex)
                        {
                            log.Error("Source: {0}", ex.Source);
                            log.Error("Stack Trace: {0}", ex.StackTrace);
                            log.Error("HandleUnknownAction-relatedContentForDyn: {0}", ex.Message);
                        }
                    }
                    else
                    {
                        log.Debug("There was no item for the given url, ({0}).", SitefinityExtensions.IsNullOrEmpty(postUrl) ? "There is no url available" : postUrl);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Info("exceptionRelatedContent:{0}, inner:{1}", ex.Message, ex.InnerException?.Message );
            }
            
        }

        private List<BlendedListItem> GetNewsBlendedForUrl(List<Type> dynamicContent, string url)
        {
            List<BlendedListItem> newsBlend = new List<BlendedListItem>();
            if (dynamicContent.Any())
            {
                var dynamicModuleManager = DynamicModuleManager.GetManager();
                var taxonomyManager = TaxonomyManager.GetManager();
                log.Info("contYpe:{0}".Fmt(dynamicContent.Count));
                foreach (Type contentType in dynamicContent)
                {
                    log.Info("type of:{0} with url{1}".Fmt(contentType, url));
                    DynamicContent content = dynamicModuleManager.GetDataItems(contentType).FirstOrDefault(bp => bp.UrlName == url);
                    if (content != null)
                    {
                        log.Info("dyn content url is:{0} from {1}".Fmt(content.UrlName, url));
                        DynamicContent liveContent = dynamicModuleManager.Lifecycle.GetLive(content) as DynamicContent;
                        if (liveContent != null)
                        {
                            List<Guid> categories = liveContent.GetValue<IList<Guid>>("Category").ToList();
                            Guid[] contentCategories = new Guid[0];
                            int hitCount = 0;
                            contentCategories =
                                categories.Where(c => taxonomyManager.GetTaxon<HierarchicalTaxon>(c) != null)
                                    .Select(m => m)
                                    .ToArray();
                            //var resultContent = BlendedNewsHelper.GetNewsItems(Providers, _searchIndex, out hitCount, contentCategories, null, 0, this.NumberOfPosts + 10);
                            var resultContent = BlendedNewsHelper.GetNewsDocs(Providers, _searchIndex, out hitCount, contentCategories, null, 0, this.NumberOfPosts + 10);
                            List<BlendedListItem> newsResult = SetBlendedListItems(resultContent);
                            return newsResult;
                        }
                    }

                }

            }
            return newsBlend;
        }

        private List<BlendedListItem> GetContentFor()
        {
            List<Type> dynamicTypes = new List<Type>();
            string postUrl = "/";
            List<BlendedListItem> result = new List<BlendedListItem>();
            try
            {

                //var dynamicModuleManager = DynamicModuleManager.GetManager();
                //var taxonomyManager = TaxonomyManager.GetManager();
                var currentContext = System.Web.HttpContext.Current;
                IList<SitefinityWebApp.Mvc.Models.BlendedListItem> actResult = new List<BlendedListItem>();
                postUrl = currentContext.Request.Url.AbsolutePath;

                var dynamicTypeNames = AppSettingsUtility.GetValue<string>("RelatedItems.DynamicType.List").Split('|').ToList();
                dynamicTypes = dynamicTypeNames.Select(dynamicTypeName => TypeResolutionService.ResolveType("Telerik.Sitefinity.DynamicTypes.Model." + dynamicTypeName)).ToList();

                /*if (postUrl.Contains('?'))
                {
                    //log.Debug("postUrl contains a '?'.");
                    postUrl = postUrl.Split('?').First();
                    //log.Trace("postUrl: {0}", postUrl.IsNullOrEmpty() ? "There is no url available" : postUrl);
                }*/
                postUrl = currentContext.Request.Url.AbsolutePath.Split('/').Last();

                result = GetNewsBlendedForUrl(dynamicTypes, postUrl);
            }
            catch (Exception ex)
            {
                log.Info(StringExtensions.Fmt("error {0}", ex.Message));
            }
            return result;
        }
        #endregion


        //***********Helper Methods***********//
        //************************************//
        //************************************//
        #region SetBlendedListItems

        private List<BlendedListItem> SetBlendedListItems(IEnumerable<IDocument> results)
        {
            log.Info("Begin SetBlendedListItems");
            
            
            var resultSet = new List<BlendedListItem>();
            var taxonomyManager = TaxonomyManager.GetManager();
            if (results != null)
            {
                foreach (var result in results)
                {
                    //log.Trace("Getting a new instance of BlendedListItem.");
                    var featured = result.Fields.Any(c => c.Name == "FeaturedRank") && (result.GetValue("FeaturedRank")?.ToString() == "1" ? true : false);
                    var newsResult = new BlendedListItem()
                    {
                        Title = result.GetValue("Title")?.ToString() ?? string.Empty,
                        LocationStreet = result.Fields.Any(c => c.Name == "LocationStreet") ? (result.GetValue("LocationStreet")?.ToString() ?? string.Empty):String.Empty,
                        LocationState = result.Fields.Any(c => c.Name == "LocationState") ? result.GetValue("LocationState")?.ToString() ?? string.Empty: string.Empty,
                        LocationCity = result.Fields.Any(c => c.Name == "LocationCity") ? result.GetValue("LocationCity")?.ToString() ?? string.Empty: string.Empty,
                        Summary = result.GetValue("Summary")?.ToString() ?? string.Empty,
                        Content = result.GetValue("Content")?.ToString() ?? String.Empty,
                        Featured = featured,
                        Link = result.Fields.Any(c => c.Name == "Link") ? result.GetValue("Link")?.ToString() ?? string.Empty: string.Empty,
                        DefaultLinkBase = result.Fields.Any(c => c.Name == "DefaultLinkBase") ? result.GetValue("DefaultLinkBase")?.ToString() ?? string.Empty: string.Empty,
                        DisplayDate = result.Fields.Any(c => c.Name == "DisplayDate") ? result.GetValue("DisplayDate")?.ToString() ?? string.Empty: string.Empty,
                        ContentType = result.Fields.Any(c => c.Name == "ContentTypeDescription") ? result.GetValue("ContentTypeDescription")?.ToString() ?? String.Empty: string.Empty,
                        Image = result.Fields.Any(c => c.Name == "ImageUrl") ? result.GetValue("ImageUrl")?.ToString() ?? string.Empty: string.Empty

                    };


                    if (result.Fields.Any(x => x.Name == "Selfpaced"))
                    {
                        var selfPaced = result.GetValue("Selfpaced")?.ToString() ?? String.Empty;
                        if (!selfPaced.IsNullOrWhitespace())
                        {
                            newsResult.SelfPaced = bool.Parse(result.GetValue("Selfpaced").ToString());
                        }
                    }
                    if (result.Fields.Any(x => x.Name == "DateField"))
                    {
                        if (result.Fields.Any(x => x.Name == "EventStart"))
                        {
                            DateTime dt;
                            if (!String.IsNullOrEmpty(result.GetValue("DateField")?.ToString()))
                            {
                                //newsResult.DateField = DateTime.ParseExact(result.GetValue("DateField").ToString(), "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                                DateTime.TryParseExact(result.GetValue("DateField").ToString(), "MM/dd/yyyy HH:mm:ss", new CultureInfo("en-US"), DateTimeStyles.None, out dt);
                                newsResult.DateField = dt;
                            }
                        }

                    }

                    if (result.Fields.Any(x => x.Name == "Protected"))
                    {
                        string IsProtected = result.GetValue("Protected")?.ToString() ?? "0";
                        newsResult.Protected = !string.IsNullOrEmpty(IsProtected) && (IsProtected == "1" ? true : false);

                    }

                    DateTime eDateTime;
                    if (result.Fields.Any(x => x.Name == "EventStart"))
                    {

                        if (!String.IsNullOrEmpty(result.GetValue("EventStart")?.ToString()))
                        {
                            //ParseExact(result.GetValue("EventStart").ToString(), "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                            DateTime.TryParseExact(result.GetValue("EventStart").ToString(), "MM/dd/yyyy HH:mm:ss", new CultureInfo("en-US"), DateTimeStyles.None, out eDateTime);
                            newsResult.EventStartDate = eDateTime;
                        }
                    }

                    if (result.Fields.Any(x => x.Name == "EventEnd"))
                    {
                        //newsResult.EventEndDate = DateTime.ParseExact(result.GetValue("EventStart").ToString(), "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        if (!String.IsNullOrEmpty(result.GetValue("EventEnd")?.ToString()))
                        {
                            DateTime.TryParseExact(result.GetValue("EventEnd").ToString(), "MM/dd/yyyy HH:mm:ss", new CultureInfo("en-US"), DateTimeStyles.None, out eDateTime);
                            newsResult.EventEndDate = eDateTime;
                        }
                    }

                    if (result.Fields.Any(x => x.Name == "PublishDate"))
                    {
                        //newsResult.PublicationDate = DateTime.ParseExact(result.GetValue("PublishDate").ToString(), "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        if (!String.IsNullOrEmpty(result.GetValue("PublishDate")?.ToString()))
                        {
                            DateTime.TryParseExact(result.GetValue("PublishDate").ToString(), "MM/dd/yyyy HH:mm:ss", new CultureInfo("en-US"), DateTimeStyles.None, out eDateTime);
                            newsResult.PublicationDate = eDateTime;
                        }

                    }

                    if (result.Fields.Any(x => x.Name == "CategoryIds"))
                    {
                        newsResult.Categories = new List<CategoryPair>();

                        var categoryIds = result.GetValue("CategoryIds")?.ToString()?.Split(',')?.ToList();
                        if (categoryIds != null && categoryIds.Any())
                        {
                            foreach (var categoryItem in categoryIds)
                            {
                                try
                                {
                                    var categoryPair = new CategoryPair();
                                    if (!String.IsNullOrEmpty(categoryItem) && categoryItem.IsGuid())
                                    {
                                        categoryPair.Guid = categoryItem;
                                        var taxon = taxonomyManager.GetTaxon(Guid.Parse(categoryItem));
                                        categoryPair.Name = taxon.Title.Value;
                                        newsResult.Categories.Add(categoryPair);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    log.Error("Source: {0}", ex.Source);
                                    log.Error("Stack Trace: {0}", ex.StackTrace);
                                    log.Error("RelatedContentDyn-GetCategoryIds: {0}", ex.Message);
                                }
                            }
                        }

                        //newsResult.Categories = result.GetValue("CategoryList").ToString().Split(',').Select(x => x.Trim()).ToList();
                        //log.Trace("CategoryList Set.");
                    }

                    if (result.Fields.Any(x => x.Name == "ResourceTypesIds"))
                    {
                        newsResult.ResourceTypes = new List<CategoryPair>();

                        var resourceTypesIds = result.GetValue("ResourceTypesIds")?.ToString()?.Split(',')?.ToList();
                        if (resourceTypesIds != null && resourceTypesIds.Any())
                        {
                            foreach (var resourceTypeItem in resourceTypesIds)
                            {
                                try
                                {
                                    var categoryPair = new CategoryPair();
                                    if (!String.IsNullOrEmpty(resourceTypeItem) && resourceTypeItem.IsGuid())
                                    {
                                        categoryPair.Guid = resourceTypeItem;
                                        var taxon = taxonomyManager.GetTaxon(Guid.Parse(resourceTypeItem));
                                        categoryPair.Name = taxon.Title.Value;
                                        newsResult.Categories.Add(categoryPair);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    log.Error("Source: {0}", ex.Source);
                                    log.Error("Stack Trace: {0}", ex.StackTrace);
                                    log.Error("RelatedContentDyn-GetResourceTypesIds: {0}", ex.Message);
                                }
                            }
                        }

                        //newsResult.ResourceTypes = result.GetValue("ResourceTypesList").ToString().Split(',').Select(x => x.Trim()).ToList();
                        //log.Trace("ResourceTypesList Set.");
                    }

                    if (result.Fields.Any(x => x.Name == "OrganizationalAuthorsIds"))
                    {
                        newsResult.OrganizationalAuthors = new List<CategoryPair>();

                        var organizationalAuthorIds = result.GetValue("OrganizationalAuthorsIds")?.ToString()?.Split(',')?.ToList();
                        if (organizationalAuthorIds != null && organizationalAuthorIds.Any())
                        {
                            foreach (var organizationalAuthorItem in organizationalAuthorIds)
                            {
                                try
                                {
                                    var categoryPair = new CategoryPair();
                                    if (!String.IsNullOrEmpty(organizationalAuthorItem) && organizationalAuthorItem.IsGuid())
                                    {
                                        categoryPair.Guid = organizationalAuthorItem;
                                        var taxon = taxonomyManager.GetTaxon(Guid.Parse(organizationalAuthorItem));
                                        categoryPair.Name = taxon.Title.Value;
                                        newsResult.Categories.Add(categoryPair);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    log.Error("Source: {0}", ex.Source);
                                    log.Error("Stack Trace: {0}", ex.StackTrace);
                                    log.Error("RelatedContentDyn-GetOrganizationalAuthorsIds: {0}", ex.Message);
                                }
                            }
                        }

                        //newsResult.OrganizationalAuthors = result.GetValue("OrganizationalAuthorsList").ToString().Split(',').Select(x => x.Trim()).ToList();
                        //log.Trace("OrganizationalAuthorsList Set.");
                    }
                    
                    if (string.IsNullOrWhiteSpace(newsResult.Summary) && !string.IsNullOrWhiteSpace(newsResult.Content))
                    {
                        newsResult.Summary = SummaryParser.GetSummary(newsResult.Content, new SummarySettings(SummaryMode.Words, 40, true));
                    }

                    resultSet.Add(newsResult);
                }
            }
            return resultSet;
        }

        
        private List<BlendedListItem> SetBlendedListItems_dev(IResultSet results)
        {
            var resultSet = new List<BlendedListItem>();
            var taxonomyManager = TaxonomyManager.GetManager();
            if (results != null && results.Any())
            {
                foreach (IDocument result in results)
                {
                    var item = new BlendedListItem()
                    {
                        Title = result.GetValue("Title")?.ToString()??"",

                    };
                }
                
            }

            return resultSet;
        }
        #endregion

        protected override void OnException(ExceptionContext filterContext)
        {
            // Let other exceptions just go unhandled
            if (filterContext.Exception is InvalidOperationException)
            {
                log.Error("ExceptionBlendedNewsList:{0}", filterContext.Exception?.Message);
                // Configure the response object 
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
                //context.HttpContext.Response.StatusCode = 500;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }
            else
            {
                log.Error("MvcException:{0}, type:{1}", filterContext.Exception?.Message, filterContext.Exception?.GetType()?.FullName);
                // Configure the response object 
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
                //context.HttpContext.Response.StatusCode = 500;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }
        }
    }
}