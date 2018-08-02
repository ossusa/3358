using MatrixGroup.Sitefinity.Config.AppSettings;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Taxonomies.Model;
using SitefinityWebApp.Custom.News;
using Telerik.Sitefinity.Services.Search.Data;
using SitefinityWebApp.Mvc.Models;
using System.Globalization;
using Telerik.Sitefinity.Data.Summary;
using Telerik.Sitefinity.Modules.Events;
using Telerik.Sitefinity.Events.Model;

namespace SitefinityWebApp.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "RelatedContentForEvents", Title = "Related Content For Events", SectionName = "Custom")]
    public class RelatedContentForEventsController : Controller
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
                    return "RelatedContentForEvents";
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
            var eventsManager = EventsManager.GetManager();
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

            var item = eventsManager.GetEvents().Where(e => e.UrlName == postUrl).FirstOrDefault();

            if (item != null)
            {
                log.Debug("item: {0}", item.Title.Value.IsNullOrEmpty() ? "There is no title set" : item.Title.Value);
                item = eventsManager.Lifecycle.GetLive(item) as Event;
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

            
            foreach (IDocument result in results)
            {
                
                var newsResult = new BlendedListItem();

                //log.Trace(() => String.Join("\n", result.Fields.Select(f => f.Name + ": " + f.Value)));

                if (result.Fields.Any(x => x.Name == "Title"))
                {
                    newsResult.Title = result.GetValue("Title").ToString();
                  
                }

                if (result.Fields.Any(x => x.Name == "LocationStreet"))
                {
                    newsResult.LocationStreet = result.GetValue("LocationStreet").ToString();
                  
                }

                if (result.Fields.Any(x => x.Name == "LocationState"))
                {
                    newsResult.LocationState = result.GetValue("LocationState").ToString();
                  
                }

                if (result.Fields.Any(x => x.Name == "LocationCity"))
                {
                    newsResult.LocationCity = result.GetValue("LocationCity").ToString();
                  
                }

                if (result.Fields.Any(x => x.Name == "Content"))
                {
                    newsResult.Content = result.GetValue("Content").ToString();
                  
                }

                var featured = result.GetValue("FeaturedRank").ToString() == "1" ? true : false;
                newsResult.Featured = featured;

                if (result.Fields.Any(x => x.Name == "Link"))
                {
                    newsResult.Link = result.GetValue("Link").ToString();
                  
                }

                if (result.Fields.Any(x => x.Name == "EventStart"))
                {
                    if (!String.IsNullOrEmpty(result.GetValue("EventStart")?.ToString()))
                    {
                        try
                        {
                            DateTime eDateTime;
                            //newsResult.EventStartDate = DateTime.ParseExact(result.GetValue("EventStart").ToString(), "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                            DateTime.TryParseExact(result.GetValue("EventStart").ToString(), "MM/dd/yyyy HH:mm:ss", new CultureInfo("en-US"),DateTimeStyles.None, out eDateTime);

                            newsResult.EventStartDate = eDateTime;
                  
                        }
                        catch (Exception ex)
                        {
                            log.Error("EventStart-Message: {0}", ex.Message);
                        }
                    }
                    
                }

                if (result.Fields.Any(x => x.Name == "EventEnd"))
                {
                    
                    if (!String.IsNullOrEmpty((result.GetValue("EventEnd")?.ToString())))
                    {
                        try
                        {
                            //newsResult.EventEndDate = DateTime.ParseExact(result.GetValue("EventStart").ToString(), "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                            DateTime eDateTime;
                            DateTime.TryParseExact(result.GetValue("EventEnd").ToString(), "MM/dd/yyyy HH:mm:ss", new CultureInfo("en-US"), DateTimeStyles.None, out eDateTime);

                            newsResult.EventEndDate = eDateTime;
                        }
                        catch (Exception ex)
                        {
                            log.Error("EventEnd-Message: {0}", ex.Message);
                        }
                    }
                }

                if (result.Fields.Any(x => x.Name == "PublishDate"))
                {
                    
                    if (!String.IsNullOrEmpty((result.GetValue("PublishDate")?.ToString())))
                    {
                        try
                        {
                            //newsResult.PublicationDate = DateTime.ParseExact(result.GetValue("PublishDate").ToString(), "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                            DateTime eDateTime;
                            DateTime.TryParseExact(result.GetValue("PublishDate").ToString(), "MM/dd/yyyy HH:mm:ss", new CultureInfo("en-US"), DateTimeStyles.None, out eDateTime);

                            newsResult.PublicationDate = eDateTime;
                        }
                        catch (Exception ex)
                        {
                            log.Error("PublishDate-Message: {0}", ex.Message);
                        }
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
                            if (!categoryItem.IsNullOrWhitespace() && categoryItem.IsGuid())
                            {
                                var categoryPair = new CategoryPair();
                                categoryPair.Guid = categoryItem;
                                var taxon = taxonomyManager.GetTaxon(Guid.Parse(categoryItem));
                                categoryPair.Name = taxon.Title.Value;
                                newsResult.Categories.Add(categoryPair);
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            log.Error("CategoryIds-Message: {0}", ex.Message);
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
                            if (!resourceTypeItem.IsNullOrWhitespace() && resourceTypeItem.IsGuid())
                            {
                                var categoryPair = new CategoryPair();
                                categoryPair.Guid = resourceTypeItem;
                                var taxon = taxonomyManager.GetTaxon(Guid.Parse(resourceTypeItem));
                                categoryPair.Name = taxon.Title.Value;
                                newsResult.Categories.Add(categoryPair);
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error("ResourceTypesIds-Message: {0}", ex.Message);
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
                            if (!organizationalAuthorItem.IsNullOrWhitespace() && organizationalAuthorItem.IsGuid())
                            {
                                var categoryPair = new CategoryPair();
                                categoryPair.Guid = organizationalAuthorItem;
                                var taxon = taxonomyManager.GetTaxon(Guid.Parse(organizationalAuthorItem));
                                categoryPair.Name = taxon.Title.Value;
                                newsResult.Categories.Add(categoryPair);
                            }
                        }
                        catch (Exception ex)
                        {
                            
                            log.Error("OrganizationalAuthorsIds-Message: {0}", ex.Message);
                        }
                    }
                    //newsResult.OrganizationalAuthors = result.GetValue("OrganizationalAuthorsList").ToString().Split(',').Select(x => x.Trim()).ToList();
                    //log.Trace("OrganizationalAuthorsList Set.");
                }

                if (result.Fields.Any(x => x.Name == "DefaultLinkBase"))
                {
                    newsResult.DefaultLinkBase = result.GetValue("DefaultLinkBase").ToString();
                   
                }

                if (result.Fields.Any(x => x.Name == "Selfpaced"))
                {
                    var selfPaced = result.GetValue("Selfpaced").ToString();
                    if (!selfPaced.IsNullOrWhitespace())
                    {
                        newsResult.SelfPaced = bool.Parse(result.GetValue("Selfpaced").ToString());
                      
                    }
                }

                if (result.Fields.Any(x => x.Name == "DisplayDate"))
                {
                    newsResult.DisplayDate = result.GetValue("DisplayDate").ToString();
                   
                }

                if (result.Fields.Any(x => x.Name == "DateField"))
                {
                    if (!String.IsNullOrEmpty((result.GetValue("DateField")?.ToString())))
                    {
                        try
                        {
                            
                            DateTime eDateTime;
                            DateTime.TryParseExact(result.GetValue("DateField").ToString(), "MM/dd/yyyy HH:mm:ss", new CultureInfo("en-US"), DateTimeStyles.None, out eDateTime);

                            newsResult.DateField = eDateTime;
                        }
                        catch (Exception ex)
                        {
                            log.Error("Source: {0}", ex.Source);
                            log.Error("Stack Trace: {0}", ex.StackTrace);
                            log.Error("Message: {0}", ex.Message);
                        }
                    }
                    
                }

                if (result.Fields.Any(x => x.Name == "ContentTypeDescription"))
                {
                    newsResult.ContentType = result.GetValue("ContentTypeDescription").ToString();
                   
                }

                if (result.Fields.Any(x => x.Name == "ImageUrl"))
                {
                    newsResult.Image = result.GetValue("ImageUrl").ToString();
                 
                }

                if (result.Fields.Any(x => x.Name == "Protected"))
                {
                    newsResult.Protected = result.GetValue("Protected") == "1";
                  
                }

                if (string.IsNullOrWhiteSpace(newsResult.Summary) && !string.IsNullOrWhiteSpace(newsResult.Content))
                {
                    
                    newsResult.Summary = SummaryParser.GetSummary(newsResult.Content, new SummarySettings(SummaryMode.Words, 40, true));
                    
                }

                resultSet.Add(newsResult);
                
            }
            
            return resultSet;
        }

        #endregion
    }
}