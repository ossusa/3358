using MatrixGroup.Sitefinity.Config.AppSettings;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Blogs.Model;
using Telerik.Sitefinity.Modules.Blogs;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Taxonomies.Model;
using SitefinityWebApp.Custom.News;
using Telerik.Sitefinity.Services.Search.Data;
using SitefinityWebApp.Mvc.Models;
using System.Globalization;
using MatrixGroup.Data.Extension;
using Telerik.Sitefinity.Data.Summary;

namespace SitefinityWebApp.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "RelatedContentForBlogPosts", Title = "Related Content For Blog Posts", SectionName = "Custom")]
    public class RelatedContentForBlogPostsController : Controller
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
                if (ViewTemplate.IsNullOrEmpty())
                {
                    return "RelatedContentForBlogPosts";
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
                    var searchIndexName = searchIndexPair.Split('|').First();

                    if (SearchIndex != null && SearchIndex.ToUpper() == searchIndexName.ToUpper())
                    {
                        return Guid.Parse(searchIndexPair.Split('|').Last());
                    }
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

                return ProviderNames.Split(',').ToArray();
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

        protected override void HandleUnknownAction(string actionName)
        {
            log.Info("Begin HandleUnknownAction.");
            var blogsManager = BlogsManager.GetManager();
            log.Trace("Got an instance of the BlogsManager.");
            var taxonomyManager = TaxonomyManager.GetManager();
            log.Trace("Got an instance of the TaxonomyManager.");
            var currentContext = System.Web.HttpContext.Current;
            log.Trace("Got the Current Context.");
            var postUrl = currentContext.Request.Url.AbsolutePath;

            if (postUrl.Contains('?'))
            {
                log.Debug("postUrl contains a '?'.");
                postUrl = postUrl.Split('?').First();
                log.Trace("postUrl: {0}", postUrl.IsNullOrEmpty() ? "There is no url available" : postUrl);
            }

            postUrl = currentContext.Request.Url.AbsolutePath.Split('/').Last();
            log.Debug("postUrl: {0}", postUrl.IsNullOrEmpty() ? "There is no url available" : postUrl);

            var item = blogsManager.GetBlogPosts().Where(bp => bp.UrlName == postUrl).FirstOrDefault();

            if (item != null)
            {
                log.Debug("item: {0}", item.Title.Value.IsNullOrEmpty() ? "There is no title set" : item.Title.Value);
                item = blogsManager.Lifecycle.GetLive(item) as BlogPost;
                log.Trace("Got the live version of the Blog Post.");
                try
                {
                    var allCategories = item.GetValue<IList<Guid>>("Category").ToArray();
                    log.Debug("Got a list of category ids for the post, {0}.", allCategories.Count());
                    List<Guid> categories = new List<Guid>();
                    log.Trace("Set the list for Blended List Query.");
                    int hitCount = 0;
                    log.Trace("Set the hitcount variable required for Sitefinity Search.");

                    foreach (var currGuid in allCategories)
                    {
                       
                        var currTaxon = taxonomyManager.GetTaxon<HierarchicalTaxon>(currGuid);

                        if (currTaxon != null)
                        {
                            log.Debug("Got taxonomy, {0}, for the id, {1}", currTaxon.Title.Value.IsNullOrEmpty() ? "There is no title set" : currTaxon.Title.Value);
                            categories.Add(currGuid);
                            log.Trace("Taxon added to the list for the search.");
                        }
                    }

                    log.Info("Calling BlendedListHelper.GetItems.");
                    var resultListItems = BlendedNewsHelper.GetNewsItems(Providers, _searchIndex, out hitCount, categories.ToArray(), null, 0, this.NumberOfPosts + 10);
                    log.Info("Number of results: {0}", hitCount);
                    log.Info("Calling SetBlendedListItems.");
                    var blendedListItems = SetBlendedListItems(resultListItems);
                    log.Debug("Prune the current Item if it is in the list.");
                    blendedListItems = blendedListItems
                        .Where(bli => !bli.Link.Contains(item.UrlName) &&
                                      !bli.Featured).Take(this.NumberOfPosts).ToList();

                    log.Debug("Prep the template for use.");
                    string template = _viewTemplate;
                    log.Info("Finishing HandleUnknownAction");
                    this.View(template, blendedListItems).ExecuteResult(ControllerContext);
                }
                catch (Exception ex)
                {
                    log.Error("Source: {0}", ex.Source);
                    log.Error("Stack Trace: {0}", ex.StackTrace);
                    log.Error("Message: {0}", ex.Message);
                }
            }
            else
            {
                log.Debug("There was no item for the given url, ({0}).", postUrl.IsNullOrEmpty() ? "There is no url available" : postUrl);
            }
        }

        #endregion


        //***********Helper Methods***********//
        //************************************//
        //************************************//
        #region SetBlendedListItems

        private List<BlendedListItem> SetBlendedListItems(IResultSet results)
        {
            log.Info("Begin SetBlendedListItems.");
            var resultSet = new List<BlendedListItem>();
            var taxonomyManager = TaxonomyManager.GetManager();

            log.Debug("Iterating through the results.");
            foreach (IDocument result in results)
            {
                log.Trace("Getting a new instance of BlendedListItem.");
                var newsResult = new BlendedListItem();

                log.Trace(() => String.Join("\n", result.Fields.Select(f => f.Name + ": " + f.Value)));

                if (result.Fields.Any(x => x.Name == "Title"))
                {
                    newsResult.Title = result.GetValue("Title").ToString();
                    log.Trace("Title Set.");
                }

                if (result.Fields.Any(x => x.Name == "LocationStreet"))
                {
                    newsResult.LocationStreet = result.GetValue("LocationStreet").ToString();
                    log.Trace("LocationStreet Set.");
                }

                if (result.Fields.Any(x => x.Name == "LocationState"))
                {
                    newsResult.LocationState = result.GetValue("LocationState").ToString();
                    log.Trace("LocationState Set.");
                }

                if (result.Fields.Any(x => x.Name == "LocationCity"))
                {
                    newsResult.LocationCity = result.GetValue("LocationCity").ToString();
                    log.Trace("LocationCity Set.");
                }

                if (result.Fields.Any(x => x.Name == "Summary"))
                {
                    newsResult.Summary = result.GetValue("Summary").ToString();
                    log.Trace("Summary Set.");
                }

                if (result.Fields.Any(x => x.Name == "Content"))
                {
                    newsResult.Content = result.GetValue("Content").ToString();
                    log.Trace("Content Set.");
                }

                var featured = result.GetValue("FeaturedRank").ToString() == "1" ? true : false;
                newsResult.Featured = featured;

                if (result.Fields.Any(x => x.Name == "Link"))
                {
                    newsResult.Link = result.GetValue("Link").ToString();
                    log.Trace("Link  Set.");
                }

                if (result.Fields.Any(x => x.Name == "EventStart"))
                {
                    try
                    {
                        DateTime dp=DateTime.MinValue;
                        //newsResult.EventStartDate = DateTime.ParseExact(result.GetValue("EventStart").ToString(), "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        DateTime.TryParseExact(result.GetValue("EventStart").ToString(), "MM/dd/yyyy HH:mm:ss", new CultureInfo("en-US"), DateTimeStyles.None, out dp);
                        newsResult.EventStartDate = dp;
                        //log.Trace("EventStart Set.");
                    }
                    catch (Exception ex)
                    {
                        log.Error("Source: {0}", ex.Source);
                        log.Error("Stack Trace: {0}", ex.StackTrace);
                        log.Error("Message: {0}", ex.Message);
                    }
                }

                if (result.Fields.Any(x => x.Name == "EventEnd"))
                {
                    try
                    {
                        /*newsResult.EventEndDate = DateTime.ParseExact(result.GetValue("EventStart").ToString(), "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        log.Trace("EventStart Set.");*/
                        DateTime dp = DateTime.MinValue;
                        DateTime.TryParseExact(result.GetValue("EventEnd").ToString(), "MM/dd/yyyy HH:mm:ss", new CultureInfo("en-US"), DateTimeStyles.None, out dp);
                        newsResult.EventEndDate = dp;
                    }
                    catch (Exception ex)
                    {
                        log.Error("Source: {0}", ex.Source);
                        log.Error("Stack Trace: {0}", ex.StackTrace);
                        log.Error("Message: {0}", ex.Message);
                    }
                }

                if (result.Fields.Any(x => x.Name == "PublishDate"))
                {
                    try
                    {
                        /*newsResult.PublicationDate = DateTime.ParseExact(result.GetValue("PublishDate").ToString(), "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        log.Trace("PublishDate Set.");*/
                        DateTime dp = DateTime.MinValue;
                        DateTime.TryParseExact(result.GetValue("PublishDate").ToString(), "MM/dd/yyyy HH:mm:ss", new CultureInfo("en-US"), DateTimeStyles.None, out dp);
                        newsResult.PublicationDate = dp;
                    }
                    catch (Exception ex)
                    {
                        log.Error("Source: {0}", ex.Source);
                        log.Error("Stack Trace: {0}", ex.StackTrace);
                        log.Error("Message: {0}", ex.Message);
                    }
                }

                if (result.Fields.Any(x => x.Name == "CategoryIds"))
                {
                    newsResult.Categories = new List<CategoryPair>();

                    var categoryIds = result.GetValue("CategoryIds").ToString().Split(',').ToList();
                    foreach (var categoryItem in categoryIds)
                    {
                        try
                        {
                            if (!categoryItem.IsNullOrWhitespace())
                            {
                                var categoryPair = new CategoryPair();
                                if (categoryItem.IsGuid())
                                {
                                    categoryPair.Guid = categoryItem;
                                    var taxon = taxonomyManager.GetTaxon(Guid.Parse(categoryItem));
                                    categoryPair.Name = taxon.Title.Value;
                                    newsResult.Categories.Add(categoryPair);
                                }
                                
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error("Source: {0}", ex.Source);
                            log.Error("Stack Trace: {0}", ex.StackTrace);
                            log.Error("Message: {0}", ex.Message);
                        }
                    }
                    //newsResult.Categories = result.GetValue("CategoryList").ToString().Split(',').Select(x => x.Trim()).ToList();
                    //log.Trace("CategoryList Set.");
                }

                if (result.Fields.Any(x => x.Name == "ResourceTypesIds"))
                {
                    newsResult.ResourceTypes = new List<CategoryPair>();

                    var resourceTypesIds = result.GetValue("ResourceTypesIds").ToString().Split(',').ToList();
                    foreach (var resourceTypeItem in resourceTypesIds)
                    {
                        try
                        {
                            if (!resourceTypeItem.IsNullOrWhitespace())
                            {
                                var categoryPair = new CategoryPair();
                                if (resourceTypeItem.IsGuid())
                                {
                                    categoryPair.Guid = resourceTypeItem;
                                    var taxon = taxonomyManager.GetTaxon(Guid.Parse(resourceTypeItem));
                                    categoryPair.Name = taxon.Title.Value;
                                    newsResult.Categories.Add(categoryPair);
                                }
                                
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error("Source: {0}", ex.Source);
                            log.Error("Stack Trace: {0}", ex.StackTrace);
                            log.Error("Message: {0}", ex.Message);
                        }
                    }
                    //newsResult.ResourceTypes = result.GetValue("ResourceTypesList").ToString().Split(',').Select(x => x.Trim()).ToList();
                    //log.Trace("ResourceTypesList Set.");
                }

                if (result.Fields.Any(x => x.Name == "OrganizationalAuthorsIds"))
                {
                    newsResult.OrganizationalAuthors = new List<CategoryPair>();

                    var organizationalAuthorIds = result.GetValue("OrganizationalAuthorsIds").ToString().Split(',').ToList();
                    foreach (var organizationalAuthorItem in organizationalAuthorIds)
                    {
                        try
                        {
                            if (!organizationalAuthorItem.IsNullOrWhitespace())
                            {
                                var categoryPair = new CategoryPair();
                                if (organizationalAuthorItem.IsGuid())
                                {
                                    categoryPair.Guid = organizationalAuthorItem;
                                    var taxon = taxonomyManager.GetTaxon(Guid.Parse(organizationalAuthorItem));
                                    categoryPair.Name = taxon.Title.Value;
                                    newsResult.Categories.Add(categoryPair);
                                }
                                
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error("Source: {0}", ex.Source);
                            log.Error("Stack Trace: {0}", ex.StackTrace);
                            log.Error("Message: {0}", ex.Message);
                        }
                    }
                    //newsResult.OrganizationalAuthors = result.GetValue("OrganizationalAuthorsList").ToString().Split(',').Select(x => x.Trim()).ToList();
                    //log.Trace("OrganizationalAuthorsList Set.");
                }

                if (result.Fields.Any(x => x.Name == "DefaultLinkBase"))
                {
                    newsResult.DefaultLinkBase = result.GetValue("DefaultLinkBase").ToString();
                    log.Trace("DefaultLinkBase Set.");
                }

                if (result.Fields.Any(x => x.Name == "Selfpaced"))
                {
                    var selfPaced = result.GetValue("Selfpaced").ToString();
                    if (!selfPaced.IsNullOrWhitespace())
                    {
                        newsResult.SelfPaced = bool.Parse(result.GetValue("Selfpaced").ToString());
                        log.Trace("SelfPaced Set."); 
                    }
                }

                if (result.Fields.Any(x => x.Name == "DisplayDate"))
                {
                    newsResult.DisplayDate = result.GetValue("DisplayDate").ToString();
                    log.Trace("DisplayDate Set.");
                }

                if (result.Fields.Any(x => x.Name == "DateField"))
                {
                    try
                    {
                        /*newsResult.DateField = DateTime.ParseExact(result.GetValue("DateField").ToString(), "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        log.Trace("DateField Set.");*/
                        DateTime dp = DateTime.MinValue;
                        DateTime.TryParseExact(result.GetValue("DateField").ToString(), "MM/dd/yyyy HH:mm:ss", new CultureInfo("en-US"), DateTimeStyles.None, out dp);
                        newsResult.DateField = dp;

                    }
                    catch (Exception ex)
                    {
                        log.Error("Source: {0}", ex.Source);
                        log.Error("Stack Trace: {0}", ex.StackTrace);
                        log.Error("Message: {0}", ex.Message);
                    }
                }

                if (result.Fields.Any(x => x.Name == "ContentTypeDescription"))
                {
                    newsResult.ContentType = result.GetValue("ContentTypeDescription").ToString();
                    log.Trace("ContentTypeDescription Set.");
                }

                if (result.Fields.Any(x => x.Name == "ImageUrl"))
                {
                    newsResult.Image = result.GetValue("ImageUrl").ToString();
                    log.Trace("Image Set.");
                }

                if (result.Fields.Any(x => x.Name == "Protected"))
                {
                    newsResult.Protected = result.GetValue("Protected") == "1";
                    log.Trace("Protected Set.");
                }

                if (string.IsNullOrWhiteSpace(newsResult.Summary) && !string.IsNullOrWhiteSpace(newsResult.Content))
                {
                    log.Trace("The summary field is empty and the content has a value, so we will make a summary.");
                    newsResult.Summary = SummaryParser.GetSummary(newsResult.Content, new SummarySettings(SummaryMode.Words, 40, true));
                    log.Trace("Summary made and set.");
                }

                resultSet.Add(newsResult);
                log.Trace("result added to the resturn set.");
            }

            log.Info("Finishing SetBlendedListItems.");
            return resultSet;
        }

        #endregion
    }
}