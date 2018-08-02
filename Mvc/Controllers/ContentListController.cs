using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using MatrixGroup.Sitefinity.Config.AppSettings;
using MatrixGroup.Sitefinity.Utilities;
using SitefinityWebApp.Custom.ContentList;
using Telerik.Microsoft.Practices.EnterpriseLibrary.Common.Utility;
using Telerik.Sitefinity.Data.Summary;

using Telerik.Sitefinity.Modules.Blogs;
using Telerik.Sitefinity.Mvc;
using SitefinityWebApp.Mvc.Models;
using Telerik.Sitefinity.Publishing;
using Telerik.Sitefinity.Publishing.Model;
using Telerik.Sitefinity.Publishing.Web.Services;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Services.Search;
using Telerik.Sitefinity.Services.Search.Data;

using Telerik.Sitefinity.Web;
using Telerik.Sitefinity.Utilities;

namespace SitefinityWebApp.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "ContentList", Title = "Content List", SectionName = "MvcWidgets")]
    public class ContentListController : Controller
    {
        private int _itemsPerPage = 15;
        private string _defaultSortExpression = "PublishDate DESC";
        private int _summaryWordCount = 25;
        private string _searchFieldNames = "Title";
        private string _fieldsToShow = "Title,Link,PublicationDate";
        private string _resultTemplate = "Index";

        private string _catalogId = (AppSettingsUtility.GetValue<string>("ContentListCatalogId")) ?? Guid.Empty.ToString();
        public Guid CatalogId
        {
            // get the ResourceLibraryCatalogId for the resource library from database table sf_publishing_point, column ID
            get { return Guid.Parse(_catalogId); }
            set { _catalogId = value.ToString(); }
        }

        [Category("Matrix")]
        public int ItemsPerPage
        {
            get { return _itemsPerPage; }
            set { _itemsPerPage = value; }
        }

        [Category("Matrix")]
        public string FieldsToShow
        {
            get { return _fieldsToShow; }
            set { _fieldsToShow = value; }
        }

        [Category("Matrix")]
        public string ResultTemplate
        {
            get { return _resultTemplate; }
            set { _resultTemplate = value; }
        }
        
        [Category("Matrix")]
        public string DefaultSortExpression
        {
            get { return _defaultSortExpression; }
            set { _defaultSortExpression = value; }
        }

        [Category("Matrix")]
        public string SearchFieldNames
        {
            get { return _searchFieldNames; }
            set { _searchFieldNames = value; }
        }

        [Category("Matrix")]
        public int SummaryWordCount
        {
            get { return _summaryWordCount; }
            set { _summaryWordCount = value; }
        }

        [Category("Matrix")]

        private string _contentTypeMapList = (AppSettingsUtility.GetValue<string>("ContentListContentTypeFilterMapList"));
        public string ContentTypeMapList
        {
            // format for this property would be like
            // Action Alert:Telerik.Sitefinity.DynamicTypes.Model.ActionAlerts.ActionAlert,Blog Posts:Telerik.Sitefinity.Blogs.Model.ExternalBlogPost

            get { return _contentTypeMapList; }
            set { _contentTypeMapList = value; }
        }

        [Category("Matrix")]
        private string _filterTaxaIdList = (AppSettingsUtility.GetValue<string>("ContentListCategoryFilterTaxaId")) ?? Guid.Empty.ToString();
        public string FilterTaxaIdList
        {
            // format for this property would be like
            // 6902919C-4174-6FCD-88EA-FF000002C0F4,BAAC919C-4174-6FCD-88EA-FF000002C0F4

            get { return _filterTaxaIdList; }
            set {  _filterTaxaIdList = value; }
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

        /// <summary>
        /// This is the default Action.
        /// </summary>
        public ActionResult Index(ContentListSearchCriteria criteria, int page = 1)
        {
            var taxaIdList = ContentListHelper.Topics(FilterTaxaIdList);
            var model = new ContentListResultsViewModel();
            model.Criteria = criteria;

            model.Criteria.TopicsList = new SelectList[taxaIdList.Count];
            var selectListIndex = 0;
            foreach (var topic in taxaIdList) model.Criteria.TopicsList[selectListIndex++] = new SelectList(topic, "Key", "Value");

            model.Criteria.TypesList = new SelectList(ContentListHelper.SearchTypes(ContentTypeMapList), "Item1", "Item1");

            if (!String.IsNullOrEmpty(model.Criteria.StartDate))
            {
                var dateParts = model.Criteria.StartDate.Split('/');

                model.Criteria.StartDateMonth = dateParts[0];
                model.Criteria.StartDateDay = dateParts[1];
                model.Criteria.StartDateYear = dateParts[2];
                
            }

            model.Criteria.FieldsToShow = FieldsToShow.Split(',');
            model.Criteria.SearchFieldNames = SearchFieldNames.Split(',');

            int hitCount;
            model.Results = ContentListHelper.GetSearchResults(CatalogName, criteria, ItemsPerPage * (page - 1), ItemsPerPage, SummaryWordCount, out hitCount).ToList();
            model.Pagination = new Pagination() { RouteValues = model.Criteria, CurrentPage = page, ItemsPerPage = ItemsPerPage, TotalItems = hitCount };

            return View(ResultTemplate,model);
        }

        [HttpPost]
        [ActionName("Index")]
        public ActionResult IndexPost(ContentListSearchCriteria criteria)
        {
            if ( ! (String.IsNullOrEmpty(criteria.StartDateMonth)  && String.IsNullOrEmpty(criteria.StartDateDay)  && String.IsNullOrEmpty(criteria.StartDateYear)) )
            {
                criteria.StartDate = String.Format("{0}/{1}/{2}", criteria.StartDateMonth, criteria.StartDateDay, criteria.StartDateYear);
            }

            criteria.StartDateMonth = criteria.StartDateDay = criteria.StartDateYear = null;
            var queryString = criteria.ToQueryString();
            var requestUrl = SiteMapBase.GetActualCurrentNode().Url;

            return Redirect(requestUrl + "?" + queryString);
        }




    }
}