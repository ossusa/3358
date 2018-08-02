using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SitefinityWebApp.Custom.News;
using Telerik.Sitefinity.Data.Summary;
using Telerik.Sitefinity.Mvc;
using SitefinityWebApp.Mvc.Models;
using MatrixGroup.Sitefinity.Config.AppSettings;
using System.Globalization;
using System.Text;
using ServiceStack.Logging;
using ServiceStack;
using Telerik.Sitefinity.Blogs.Model;
using Telerik.Sitefinity.DynamicModules;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Modules.Blogs;
using Telerik.Sitefinity.Utilities.TypeConverters;

namespace SitefinityWebApp.Mvc.Controllers
{
	[ControllerToolboxItem(Name = "BlendedNewsList", Title = "Blended News List", SectionName = "Matrix"), Telerik.Sitefinity.Web.UI.ControlDesign.ControlDesigner(typeof(WidgetDesigners.BlendedNewsDesigner))]
	public class BlendedNewsListController : Controller
	{
	    private ILog log = LogManager.GetLogger(typeof (BlendedNewsListController));

		#region Properties

		public int Limit { get; set; }
		public int ItemsPerPage { get; set; }
		public string ViewTemplate { get; set; }

        public string SearchIndexName { get; set; }
        private Guid SearchIndex
        {
            get
            {
                var searchIndexes = AppSettingsUtility.GetValue<string>("SearchIndex.Guids");
                var searchIndexPairs = searchIndexes.Split(':').ToList();

                foreach (var searchIndexPair in searchIndexPairs)
                {
                    var searchIndexName = searchIndexPair.Split('|').First();

                    if (SearchIndexName != null && SearchIndexName.ToUpper() == searchIndexName.ToUpper())
                    {
                        return Guid.Parse(searchIndexPair.Split('|').Last());
                    }
                }

                return AppSettingsUtility.GetValue<Guid>("NewsCatalogGuid");
            }
        }

        public string ProviderNames { get; set; }
        public string[] Providers
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

		public string CategoryIds { get; set; }
		protected Guid[] Categories
		{
			get
			{
				if (string.IsNullOrEmpty(CategoryIds))
				{
					return null;
				}

				return CategoryIds.Split(',').Select(Guid.Parse).ToArray();
			}
		}
        #endregion

	    
	    protected override void HandleUnknownAction(string actionName="Index")
	    {
            int hitCount, skip = 0, take = 0;
            var resultSet = GetBlenedResult(out hitCount, "", take, skip);
            

            Pagination pagination = new Pagination()
            {
                CurrentPage = 1,
                ItemsPerPage = ItemsPerPage <= 0 ? 1 : ItemsPerPage,
                TotalItems = hitCount
            };
            var model = new BlendedNewsListModel { ResultSet = resultSet, Pagination = pagination };
            log.Info("UnknownAction");
            //seems like we dont have have any option for other view
            this.View("Index", model).ExecuteResult(ControllerContext);
        }

        public ActionResult Index_tt(int page = 1, string term = null)
	    {
            Pagination pagination = new Pagination()
            {
                CurrentPage = page,
                ItemsPerPage = ItemsPerPage <= 0 ? 1 : ItemsPerPage,
                TotalItems = 0
            };
            var model = new BlendedNewsListModel { ResultSet = new List<NewsResult>()
            {
                new NewsResult() { }
            }, Pagination = pagination };

            //Providers, SearchIndex, out hitCount, Categories
            log.InfoFormat("Index:{0}", (new {Term= term, Provi=Providers?.Join(","),
                SearchIndex = SearchIndex, Categories = Categories?.Join(",") }).SerializeToString() );

            return View(ViewTemplate, model);
        }
       
        public ActionResult Index(int page = 1, string term = null)
		{
            log.InfoFormat("Begin Blended list Index. for:{0}", HttpContext?.Request?.Url?.AbsolutePath);
			int hitCount, skip = 0, take = 0;

			if (Limit != 0)
			{
                log.InfoFormat("Limit to:{0}", Limit);
				take = Limit;
			}
			if (ItemsPerPage != 0)
			{
                log.Info("ItemsPerPge is not equal to 0.");
				take = ItemsPerPage;
				skip = ItemsPerPage * (page - 1);
			}

            //log.Info("Search Index: {0}", SearchIndex.ToString());
            //log.Info("Calling BlendedListHelper.GetNewsItems.");
            var resultSet = GetBlenedResult(out hitCount, term, skip, take);
            //var resultSet = GetBlenedResult(out hitCount, term, skip, take);

            Pagination pagination = new Pagination()
            {
                CurrentPage = page,
                ItemsPerPage = ItemsPerPage <= 0 ? 1 : ItemsPerPage,
                TotalItems = hitCount
            };
			/*if (ItemsPerPage != 0 && hitCount > ItemsPerPage)
			{
				pagination = new Pagination()
				{
					CurrentPage = page,
					ItemsPerPage = ItemsPerPage,
					TotalItems = hitCount
				};
			}*/

			var model = new BlendedNewsListModel { ResultSet = resultSet, Pagination = pagination };
            log.InfoFormat("blednew-view:{0}, model:{1}", ViewTemplate, model?.ResultSet?.Count());
			return View(ViewTemplate, model);
		}

	    private List<NewsResult> GetBlenedResult(out int hitCount, string term, int skip, int take)
	    {
	        //hitCount = 1;
            /*return new List<NewsResult>()
            {
                new NewsResult()
                {
                    Content = "test",
                    DateField = DateTime.UtcNow,
                    DisplayDate = DateTime.UtcNow.ToLongDateString(),
                    Featured = false,
                    Image = null,
                    ImageCaption = "imgCap",
                    ImageId = string.Empty,
                    Link = "link url",
                    PublicationDate = DateTime.UtcNow,
                    Title = "Test Title", Summary = "test Summary"
                }
            };*/
            List<NewsResult> resultSet = new List<NewsResult>();
            //log.Info("content, title, Id \r\n");
            var results = BlendedNewsHelper.GetNewsDocs(Providers, SearchIndex, out hitCount, Categories, term, skip, take);

            foreach (var result in results)
            {

                //newsResult.DisplayDate = result.GetValue("DisplayDate").ToString();
                //
                try
                {
                    var ctnType = result.GetValue("ContentType")?.ToString() ?? string.Empty;

                    var featured = result.Fields.Any(c => c.Name== "FeaturedRank") ? result.GetValue("FeaturedRank")?.ToString() ?? string.Empty: string.Empty;
                    NewsResult newsResult = new NewsResult()
                    {
                        ImageId = result.Fields.Any(c => c.Name == "FeaturedRank")? result.GetValue("ImageId")?.ToString() ?? string.Empty: string.Empty,
                        Title = result.GetValue("Title")?.ToString() ?? string.Empty,
                        Summary = result.GetValue("Summary")?.ToString() ?? string.Empty,
                        Featured = !string.IsNullOrEmpty(featured) && (featured == "1" ? true : false),
                        Content = result.GetValue("Content")?.ToString() ?? string.Empty,
                        Link = result.Fields.Any(c => c.Name == "FeaturedRank") ? result.GetValue("Link")?.ToString() ?? string.Empty: string.Empty
                    };

                    var dynaManager = DynamicModuleManager.GetManager();
                    string txtId = result.GetValue("Id")?.ToString() ?? String.Empty;
                    log.InfoFormat("try to get from iD:{0} ofType:{1}-title:{2}", txtId, ctnType, newsResult.Title);
                    try
                    {
                        if (!string.IsNullOrEmpty(ctnType) && !string.IsNullOrEmpty(txtId) && txtId.IsGuid())
                        {
                            Guid itemId = new Guid(txtId);
                            if (ctnType.IndexOf("BlogPost") > 0)
                            {
                                var manager = BlogsManager.GetManager();
                                var bp = manager.GetBlogPosts()
                                    //.Where(c => c.Status == Telerik.Sitefinity.GenericContent.Model.ContentLifecycleStatus.Live)
                                    .FirstOrDefault(b => b.Id == itemId);
                                if (bp != null)
                                {
                                    bp = manager.Lifecycle.GetLive(bp) as BlogPost;
                                    log.InfoFormat("has item:{0}-date:{1}", bp?.GetType()?.FullName, bp?.GetValue("DisplayDate")?.ToString() ?? string.Empty);
                                    newsResult.DisplayDate = bp?.GetValue("DisplayDate")?.ToString() ?? string.Empty;
                                }

                            }
                            else
                            {
                                var item = dynaManager.GetDataItems(TypeResolutionService.ResolveType(ctnType))
                                .Where(c => c.Status == Telerik.Sitefinity.GenericContent.Model.ContentLifecycleStatus.Live)
                                .FirstOrDefault(c => c.Id == itemId);

                                log.InfoFormat("has item:{0}-date:{1}", item?.GetType()?.FullName, item?.GetValue("DisplayDate")?.ToString() ?? string.Empty);
                                newsResult.DisplayDate = item?.GetValue("DisplayDate")?.ToString() ?? string.Empty;
                            }

                            /*if (result.Fields.Any(x => x.Name == "DisplayDate") && item?.GetValue("DisplayDate") != null)
                            {
                                newsResult.DisplayDate = item.GetValue("DisplayDate").ToString() ?? string.Empty;
                            }*/


                        }
                    }
                    catch (Exception ex)
                    {
                        log.InfoFormat("failedToGetDateItemType:{0}-id:{1}, msg:{2}", ctnType, txtId, ex.Message);
                    }

                    string formatString = "yyyyMMddHHmmssfff";
                    // seem like there are already assing below
                    if (!String.IsNullOrEmpty(result.GetValue("PublicationDate")?.ToString()))
                    {
                        DateTime pubd = DateTime.MinValue;
                        //DateTime dt = DateTime.ParseExact(result.GetValue("PublicationDate").ToString(), formatString, null);
                        //newsResult.PublicationDate = dt.ToLocalTime();
                        DateTime.TryParseExact(result.GetValue("PublicationDate").ToString(), formatString, new CultureInfo("en-US"), DateTimeStyles.None, out pubd);
                        newsResult.PublicationDate = pubd;
                        //log.InfoFormat("pubd:{0}", pubd.ToString("MMMM d, yyyy"));
                        //sb.Append("PubDate:").Append(pubd).Append("\r");
                    }

                    if (result.Fields.Any(x => x.Name == "DateField"))
                    {
                        try
                        {
                            /*DateTime dt2 = DateTime.ParseExact(result.GetValue("DateField").ToString(), "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                            newsResult.DateField = dt2.ToLocalTime();*/
                            if (!String.IsNullOrEmpty(result.GetValue("DateField")?.ToString()))
                            {
                                DateTime eDateTime = DateTime.MinValue;
                                DateTime.TryParseExact(result.GetValue("DateField").ToString(), "MM/dd/yyyy HH:mm:ss", new CultureInfo("en-US"), DateTimeStyles.None, out eDateTime);
                                newsResult.DateField = eDateTime;
                                //log.InfoFormat("datef:{0}", eDateTime.ToString("MMMM d, yyyy"));
                            }
                        }
                        catch (Exception ex)
                        {
                            log.ErrorFormat("GetBlendedResult-DateField:{0}, inner:{1}, stack:{2}", ex.Message, ex.InnerException?.Message, ex.StackTrace);
                        }
                    }

                    if (string.IsNullOrWhiteSpace(newsResult.Summary) && !string.IsNullOrWhiteSpace(newsResult.Content))
                    {
                        newsResult.Summary = SummaryParser.GetSummary(newsResult.Content, new SummarySettings(SummaryMode.Words, 40, true));
                    }

                    resultSet.Add(newsResult);

                }
                catch (Exception ex)
                {
                    log.InfoFormat("bled-createNewsResult:{0}", ex.Message);
                }
                
            }
            return resultSet;
        }

        private List<NewsResult> GetBlenedResult_org(out int hitCount, string term, int skip, int take)
	    {

            var results = BlendedNewsHelper.GetNewsItems(Providers, SearchIndex, out hitCount, Categories, term, skip, take);
            log.InfoFormat("There are {0}, results, prov:{1}, idx:{2}, term:{3}, take:{4}", 
                results.HitCount, 
                Providers?.Join(","), 
                SearchIndex,
                Categories?.Join(","),
                term,take);
            
            List<NewsResult> resultSet = new List<NewsResult>();
            //log.Info("content, title, Id \r\n");
            StringBuilder sb = new StringBuilder();
            foreach (var result in results)
            {
                
                //newsResult.DisplayDate = result.GetValue("DisplayDate").ToString();
                //
                try
                {
                    var ctnType = result.GetValue("ContentType")?.ToString() ?? string.Empty;

                    var featured = result.GetValue("FeaturedRank")?.ToString() ?? string.Empty;
                    NewsResult newsResult = new NewsResult()
                    {
                        ImageId = result.GetValue("ImageId")?.ToString() ?? string.Empty,
                        Title = result.GetValue("Title")?.ToString() ?? string.Empty,
                        Summary = result.GetValue("Summary")?.ToString() ?? string.Empty,
                        Featured = !string.IsNullOrEmpty(featured) && (featured == "1" ? true : false),
                        Content = result.GetValue("Content")?.ToString() ?? string.Empty,
                        Link = result.GetValue("Link")?.ToString() ?? string.Empty
                    };

                    var dynaManager = DynamicModuleManager.GetManager();
                    string txtId = result.GetValue("Id")?.ToString() ?? String.Empty;
                    log.InfoFormat("try to get from iD:{0} ofType:{1}-title:{2}", txtId, ctnType, newsResult.Title);
                    try
                    {
                        if (!string.IsNullOrEmpty(ctnType) && !string.IsNullOrEmpty(txtId) && txtId.IsGuid())
                        {
                            Guid itemId = new Guid(txtId);
                            if (ctnType.IndexOf("BlogPost") > 0)
                            {
                                var manager = BlogsManager.GetManager();
                                var bp = manager.GetBlogPosts()
                                    //.Where(c => c.Status == Telerik.Sitefinity.GenericContent.Model.ContentLifecycleStatus.Live)
                                    .FirstOrDefault(b => b.Id == itemId);
                                if (bp != null)
                                {
                                    bp = manager.Lifecycle.GetLive(bp) as BlogPost;
                                    log.InfoFormat("has item:{0}-date:{1}", bp?.GetType()?.FullName, bp?.GetValue("DisplayDate")?.ToString() ?? string.Empty);
                                    newsResult.DisplayDate = bp?.GetValue("DisplayDate")?.ToString() ?? string.Empty;
                                }
                                
                            }
                            else
                            {
                                var item = dynaManager.GetDataItems(TypeResolutionService.ResolveType(ctnType))
                                .Where(c => c.Status == Telerik.Sitefinity.GenericContent.Model.ContentLifecycleStatus.Live)
                                .FirstOrDefault(c => c.Id == itemId);

                                log.InfoFormat("has item:{0}-date:{1}", item?.GetType()?.FullName, item?.GetValue("DisplayDate")?.ToString() ?? string.Empty);
                                newsResult.DisplayDate = item?.GetValue("DisplayDate")?.ToString() ?? string.Empty;
                            }
                            
                            /*if (result.Fields.Any(x => x.Name == "DisplayDate") && item?.GetValue("DisplayDate") != null)
                            {
                                newsResult.DisplayDate = item.GetValue("DisplayDate").ToString() ?? string.Empty;
                            }*/
                            

                        }
                    }
                    catch (Exception ex)
                    {
                        log.InfoFormat("failedToGetDateItemType:{0}-id:{1}, msg:{2}", ctnType, txtId, ex.Message);
                    }
                    
                    string formatString = "yyyyMMddHHmmssfff";
                    // seem like there are already assing below
                    if (!String.IsNullOrEmpty(result.GetValue("PublicationDate")?.ToString()))
                    {
                        DateTime pubd = DateTime.MinValue;
                        //DateTime dt = DateTime.ParseExact(result.GetValue("PublicationDate").ToString(), formatString, null);
                        //newsResult.PublicationDate = dt.ToLocalTime();
                        DateTime.TryParseExact(result.GetValue("PublicationDate").ToString(), formatString, new CultureInfo("en-US"), DateTimeStyles.None, out pubd);
                        newsResult.PublicationDate = pubd;
                        //log.InfoFormat("pubd:{0}", pubd.ToString("MMMM d, yyyy"));
                        //sb.Append("PubDate:").Append(pubd).Append("\r");
                    }

                    if (result.Fields.Any(x => x.Name == "DateField"))
                    {
                        try
                        {
                            /*DateTime dt2 = DateTime.ParseExact(result.GetValue("DateField").ToString(), "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                            newsResult.DateField = dt2.ToLocalTime();*/
                            if (!String.IsNullOrEmpty(result.GetValue("DateField")?.ToString()))
                            {
                                DateTime eDateTime = DateTime.MinValue;
                                DateTime.TryParseExact(result.GetValue("DateField").ToString(), "MM/dd/yyyy HH:mm:ss", new CultureInfo("en-US"), DateTimeStyles.None, out eDateTime);
                                newsResult.DateField = eDateTime;
                                //log.InfoFormat("datef:{0}", eDateTime.ToString("MMMM d, yyyy"));
                            }
                        }
                        catch (Exception ex)
                        {
                            log.ErrorFormat("GetBlendedResult-DateField:{0}, inner:{1}, stack:{2}", ex.Message, ex.InnerException?.Message, ex.StackTrace);
                        }
                    }

                    if (string.IsNullOrWhiteSpace(newsResult.Summary) && !string.IsNullOrWhiteSpace(newsResult.Content))
                    {
                        newsResult.Summary = SummaryParser.GetSummary(newsResult.Content, new SummarySettings(SummaryMode.Words, 40, true));
                    }

                    resultSet.Add(newsResult);

                }
                catch (Exception ex)
                {
                    log.InfoFormat("bled-createNewsResult:{0}", ex.Message);
                }


                
                //newsResult.ImageCaption = result.GetValue("ImageCaption").ToString();
                
                // testing below
                /*if (!String.IsNullOrEmpty(result.GetValue("PublicationDate")?.ToString()))
                {
                    //DateTime dt = DateTime.ParseExact(result.GetValue("PublicationDate").ToString(), formatString, null);
                    //newsResult.PublicationDate = dt.ToLocalTime();
                    DateTime.TryParseExact(result.GetValue("PublicationDate").ToString(), formatString, new CultureInfo("en-US"), DateTimeStyles.None, out eDateTime);
                    newsResult.DisplayDate = eDateTime.ToLocalTime().ToShortDateString();
                }*/
                ///


                

                
            }
	        return resultSet;
	    }

        #region Helper Methods
        protected override void OnException(ExceptionContext filterContext)
        {
            // Let other exceptions just go unhandled
            if (filterContext.Exception is InvalidOperationException)
            {
                log.ErrorFormat("ExceptionBlendedNewsList:{0}", filterContext.Exception?.Message);
                // Configure the response object 
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
                //context.HttpContext.Response.StatusCode = 500;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }
            else
            {
                log.ErrorFormat("MvcException:{0}, type:{1}", filterContext.Exception?.Message, filterContext.Exception?.GetType()?.FullName );
                // Configure the response object 
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
                //context.HttpContext.Response.StatusCode = 500;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }
        }
        #endregion
    }
}