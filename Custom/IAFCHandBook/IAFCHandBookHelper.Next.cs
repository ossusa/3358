using SitefinityWebApp.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Telerik.OpenAccess;
using Telerik.Sitefinity;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.DynamicModules;
using Telerik.Sitefinity.DynamicModules.Model;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Libraries.Model;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Publishing;
using Telerik.Sitefinity.RelatedData;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Taxonomies;

namespace SitefinityWebApp.Custom.IAFCHandBook
{
    public partial class IAFCHandBookHelper
    {
        public Type ExternalResourcesType => externalResourcesType;
		public Type ResourceType => resourceType;

		#region  CreateIAFCHandBookResourcesData
		public void CreateIAFCHandBookResourcesData(Guid resourceLiveId, Type resourceType)
        {
            try
            {
                var providerName = string.Empty;
                var transactionName = "createIAFCHandBookResourcesDataTransaction";
                var relationFieldName = GetRelatedResourceFieldName(resourceType);
                var currentUtcDateTime = DateTime.UtcNow;

                DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager(providerName, transactionName);

                var resLive = dynamicModuleManager.GetDataItem(resourceType, resourceLiveId);
                var resMaster = dynamicModuleManager.Lifecycle.GetMaster(resLive) as DynamicContent;

                DynamicContent resDataMaster = dynamicModuleManager.CreateDataItem(handBookResourcesType);

                var title = resMaster.GetValue("Title").ToString();
                resDataMaster.SetValue("Title", title);

                var categories = resMaster.GetValue<TrackedList<Guid>>("Category").ToArray();
                resDataMaster.Organizer.AddTaxa("Category", categories);

                var shortsummary = resMaster.GetValue("shortsummary");
                resDataMaster.SetValue("shortsummary", shortsummary);

                if (resourceType == ResourceType)
                {
                    var article = resMaster.GetValue("Article");
                    resDataMaster.SetValue("ResourceDescription", article);
                }
                else if (resourceType == ExternalResourcesType)
                {
                    var time = resMaster.GetValue("Time");
                    resDataMaster.SetValue("Time", time);

                    var videoEmbedCode = resMaster.GetValue("VideoEmbedCode");
                    resDataMaster.SetValue("VideoEmbedCode", videoEmbedCode);

                    var resourceDescription = resMaster.GetValue("ResourceDescription");
                    resDataMaster.SetValue("ResourceDescription", resourceDescription);
                }

                var departmenttype = resMaster.GetValue<TrackedList<Guid>>("departmenttype").ToArray();
                resDataMaster.Organizer.AddTaxa("departmenttype", departmenttype);

                var feeding = resMaster.GetValue<TrackedList<Guid>>("feeding").ToArray();
                resDataMaster.Organizer.AddTaxa("feeding", feeding);

                var organizationalauthors = resMaster.GetValue<TrackedList<Guid>>("organizationalauthors").ToArray();
                resDataMaster.Organizer.AddTaxa("organizationalauthors", organizationalauthors);

                var resourceLink = resMaster.GetValue("ResourceLink");
                resDataMaster.SetValue("ResourceLink", resourceLink);

                var resourcetypes = resMaster.GetValue<TrackedList<Guid>>("resourcetypes").ToArray();
                resDataMaster.Organizer.AddTaxa("resourcetypes", resourcetypes);

                var showInDetail = resMaster.GetValue("showInDetail");
                resDataMaster.SetValue("showInDetail", showInDetail);

                var titleExists = dynamicModuleManager.GetDataItems(handBookResourcesType)
                    .Where(i => i.GetValue<string>("DataTitle").ToLower() == title.ToLower())
                    .Any();

                if (titleExists)
                {
                    title += "_data_title";
                }

                resDataMaster.SetValue("DataTitle", title);

                var featuredimage = resMaster.GetRelatedItems<Image>("featuredimage").ToList();
                foreach (var image in featuredimage)
                {
                    resDataMaster.CreateRelation(image, "featuredimage");
                }

                var attachfiles = resMaster.GetRelatedItems<Document>("attachfiles").ToList();
                foreach (var file in attachfiles)
                {
                    resDataMaster.CreateRelation(file, "attachfiles");
                }

                resDataMaster.UrlName = new Lstring(Regex.Replace(title, UrlNameCharsToReplace, UrlNameReplaceString));
                resDataMaster.Owner = SecurityManager.GetCurrentUserId();
                resDataMaster.PublicationDate = currentUtcDateTime;
                resDataMaster.DateCreated = currentUtcDateTime;

                var likeTitle = $@"{title}_{handBookResourcesType.Name}";

                var likeTitleExists = dynamicModuleManager.GetDataItems(resourceLikesType)
                    .Where(i => i.GetValue<string>("Title").ToLower() == likeTitle.ToLower())
                    .Any();

                if (likeTitleExists)
                {
                    likeTitle += "_like";
                }

                // Create like
                DynamicContent likeMaster = dynamicModuleManager.CreateDataItem(resourceLikesType);
                likeMaster.SetValue("Title", likeTitle);
                likeMaster.SetValue("AmountOfLikes", 0);
                likeMaster.SetValue("AmountOfDislikes", 0);
                likeMaster.UrlName = new Lstring(Regex.Replace(likeTitle, UrlNameCharsToReplace, UrlNameReplaceString));
                likeMaster.Owner = SecurityManager.GetCurrentUserId();
                likeMaster.PublicationDate = currentUtcDateTime;
                likeMaster.DateCreated = currentUtcDateTime;

                dynamicModuleManager.Lifecycle.Publish(likeMaster);

                resDataMaster.CreateRelation(resMaster, relationFieldName);
                resDataMaster.CreateRelation(likeMaster, "Likes");

                dynamicModuleManager.Lifecycle.Publish(resDataMaster);
                resDataMaster.SetWorkflowStatus(dynamicModuleManager.Provider.ApplicationName, "Published");

                TransactionManager.CommitTransaction(transactionName);
            }
            catch (Exception e)
            {
                log.Error($@"{nameof(CreateIAFCHandBookResourcesData)} Error: {e.Message}");
            }
        }
		#endregion  CreateIAFCHandBookResourcesData

		#region UpdateIAFCHandBookResourcesData
		public void UpdateIAFCHandBookResourcesData(Guid resourceLiveId, Type resourceType)
        {
            try
            {
                var relationFieldName = GetRelatedResourceFieldName(resourceType);
                var currentUtcDateTime = DateTime.UtcNow;

                DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();

                var resLive = dynamicModuleManager.GetDataItem(resourceType, resourceLiveId);
                var resMaster = dynamicModuleManager.Lifecycle.GetMaster(resLive) as DynamicContent;

                var resDataList = resMaster.GetRelatedParentItems(handBookResourcesType.FullName, null, relationFieldName)
                    .ToList();

                var resData = resDataList.First() as DynamicContent;
                DynamicContent resDataMaster = dynamicModuleManager.Lifecycle.GetMaster(resData) as DynamicContent;
                DynamicContent resDataTemp = dynamicModuleManager.Lifecycle.CheckOut(resDataMaster) as DynamicContent;

                var title = resMaster.GetValue("Title").ToString();
                resDataTemp.SetValue("Title", title);

                var categories = resMaster.GetValue<TrackedList<Guid>>("Category").ToArray();
                resDataTemp.Organizer.Clear("Category");
                resDataTemp.Organizer.AddTaxa("Category", categories);

                var shortsummary = resMaster.GetValue("shortsummary");
                resDataTemp.SetValue("shortsummary", shortsummary);

                if (resourceType == ResourceType)
                {
                    var article = resMaster.GetValue("Article");
                    resDataTemp.SetValue("ResourceDescription", article);
                }
                else if (resourceType == ExternalResourcesType)
                {
                    var time = resMaster.GetValue("Time");
                    resDataTemp.SetValue("Time", time);

                    var videoEmbedCode = resMaster.GetValue("VideoEmbedCode");
                    resDataTemp.SetValue("VideoEmbedCode", videoEmbedCode);

                    var resourceDescription = resMaster.GetValue("ResourceDescription");
                    resDataTemp.SetValue("ResourceDescription", resourceDescription);
                }

                var departmenttype = resMaster.GetValue<TrackedList<Guid>>("departmenttype").ToArray();
                resDataTemp.Organizer.Clear("departmenttype");
                resDataTemp.Organizer.AddTaxa("departmenttype", departmenttype);

                var feeding = resMaster.GetValue<TrackedList<Guid>>("feeding").ToArray();
                resDataTemp.Organizer.Clear("feeding");
                resDataTemp.Organizer.AddTaxa("feeding", feeding);

                var organizationalauthors = resMaster.GetValue<TrackedList<Guid>>("organizationalauthors").ToArray();
                resDataTemp.Organizer.Clear("organizationalauthors");
                resDataTemp.Organizer.AddTaxa("organizationalauthors", organizationalauthors);

                var resourceLink = resMaster.GetValue("ResourceLink");
                resDataTemp.SetValue("ResourceLink", resourceLink);

                var resourcetypes = resMaster.GetValue<TrackedList<Guid>>("resourcetypes").ToArray();
                resDataTemp.Organizer.Clear("resourcetypes");
                resDataTemp.Organizer.AddTaxa("resourcetypes", resourcetypes);

                var showInDetail = resMaster.GetValue("showInDetail");
                resDataTemp.SetValue("showInDetail", showInDetail);

                var dataTitle = resDataTemp.GetValue("DataTitle").ToString();
                var titleExists = dynamicModuleManager.GetDataItems(handBookResourcesType)
                    .Where(i => i.GetValue<string>("DataTitle").ToLower() == title.ToLower() &&
                                i.GetValue<string>("DataTitle").ToLower() != dataTitle.ToLower())
                    .Any();

                if (titleExists)
                {
                    title += "_data_title";
                }

                resDataTemp.SetValue("DataTitle", title);

                resDataTemp.DeleteRelations("featuredimage");
                var featuredimage = resMaster.GetRelatedItems<Image>("featuredimage").ToList();
                foreach (var image in featuredimage)
                {
                    resDataTemp.CreateRelation(image, "featuredimage");
                }

                resDataTemp.DeleteRelations("attachfiles");
                var attachfiles = resMaster.GetRelatedItems<Document>("attachfiles").ToList();
                foreach (var file in attachfiles)
                {
                    resDataTemp.CreateRelation(file, "attachfiles");
                }

                resDataTemp.UrlName = new Lstring(Regex.Replace(title, UrlNameCharsToReplace, UrlNameReplaceString));
                resDataTemp.PublicationDate = currentUtcDateTime;
                resDataTemp.LastModified = currentUtcDateTime;
                resDataTemp.LastModifiedBy = SecurityManager.GetCurrentUserId();

                resDataMaster = dynamicModuleManager.Lifecycle.CheckIn(resDataTemp) as DynamicContent;
                dynamicModuleManager.Lifecycle.Publish(resDataMaster);
                dynamicModuleManager.SaveChanges();
            }
            catch (Exception e)
            {
                log.Error($@"{nameof(UpdateIAFCHandBookResourcesData)} Error: {e.Message}");
            }
        }
		#endregion UpdateIAFCHandBookResourcesData

		#region DeleteIAFCHandBookResourcesData
		public void DeleteIAFCHandBookResourcesData(Guid resourceID, Type resourceType)
        {
            try
            {
                var relationFieldName = GetRelatedResourceFieldName(resourceType);

                DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();

                var res = dynamicModuleManager.GetDataItem(resourceType, resourceID);

                var resDataList = res.GetRelatedParentItems(handBookResourcesType.FullName, null, relationFieldName);
                var resData = resDataList.First() as DynamicContent;
                DynamicContent resDataMaster = dynamicModuleManager.Lifecycle.GetMaster(resData) as DynamicContent;

                dynamicModuleManager.Provider.DeleteDataItem(resDataMaster);
                dynamicModuleManager.SaveChanges();
            }
            catch (Exception e)
            {
                log.Error($@"{nameof(DeleteIAFCHandBookResourcesData)} Error: {e.Message}");
            }
        }
		#endregion DeleteIAFCHandBookResourcesData

		#region IsResourceContainedWithinTopicsCategory
		public bool IsResourceContainedWithinTopicsCategory(Guid liveResourceId, Type resourceType)
        {
            if (resourceType != ExternalResourcesType &&
                resourceType != ResourceType)
            {
                throw new ArgumentException("Invalid type passed.", nameof(resourceType));
            }

            DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();

            var resLive = dynamicModuleManager.GetDataItem(resourceType, liveResourceId);
            var resMaster = dynamicModuleManager.Lifecycle.GetMaster(resLive) as DynamicContent;

            var categories = resMaster.GetValue<IList<Guid>>("Category");
            var isContained = categories.Where(c => topicCategories.Contains(c)).Any();

            return isContained;
        }
		#endregion IsResourceContainedWithinTopicsCategory

		#region IsHandBookResourcesDataExistsFor
		public bool IsHandBookResourcesDataExistsFor(Guid liveResourceId, Type resourceType)
        {
            var relationFieldName = GetRelatedResourceFieldName(resourceType);

            DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();

            var resLive = dynamicModuleManager.GetDataItem(resourceType, liveResourceId);
            var resMaster = dynamicModuleManager.Lifecycle.GetMaster(resLive) as DynamicContent;

            var resDataList = resMaster.GetRelatedParentItems(handBookResourcesType.FullName, null, relationFieldName);
            return resDataList.Any();
        }
		#endregion IsHandBookResourcesDataExistsFor

		#region GetRecentlyAddedResourcesNext
		public List<IAFCHandBookResourceModel> GetRecentlyAddedResourcesNext()
        {
            DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();

            var recentlyAddedResources = dynamicModuleManager.GetDataItems(handBookResourcesType)
                .Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live)
                .OrderByDescending(d => d.DateCreated)
                .Take(6)
                .ToList();

            var listOfMyResources = new List<IAFCHandBookResourceModel>();

            foreach (var res in recentlyAddedResources)
            {
                var handBookResource = GetResourceDetailsNext(res);
                listOfMyResources.Add(handBookResource);
            }

            return listOfMyResources;
        }
		#endregion GetRecentlyAddedResourcesNext

		#region GetFeaturedResourcesDataNext
		public IAFCHandBookResourceModel GetFeaturedResourcesDataNext()
        {
            IAFCHandBookResourceModel handBookResourceModel = new IAFCHandBookResourceModel();

            try
            {
                DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();

                var featuredResource = dynamicModuleManager.GetDataItems(handBookResourcesType)
                    .Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live)
                    .Where(d => d.GetValue<IList<Guid>>("Category").Contains(Featured_VWS_A_RIT))
                    .OrderByDescending(d => d.DateCreated)
                    .FirstOrDefault();

                if (featuredResource != null)
                {
                    handBookResourceModel = GetResourceDetailsNext(featuredResource);
                }
            }
            catch (Exception e)
            {
                log.Error($@"{nameof(GetFeaturedResourcesDataNext)} Error: {e.Message}");
            }

            return handBookResourceModel;
        }
		#endregion GetFeaturedResourcesDataNext

		#region GetResourcesPerCategoryNext
		public IAFCHandBookResourcesPerCatergoryModel GetResourcesPerCategoryNext(string categoryName, string orderBy)
        {
            IAFCHandBookResourcesPerCatergoryModel model = new IAFCHandBookResourcesPerCatergoryModel();

            try
            {
                Guid categoryID = GetCategoryGuidByName(categoryName);
                var topicCategory = GetTopicCategories(categoryID);

                model.Category.Id = categoryID;
                model.Category.CategoryTitle = topicCategory.ResourceCategoryTile;
                model.Category.CategoryUrl = topicCategory.ResourceCategoryUrl;
                model.Category.ParentCategoryTitle = topicCategory.ResourceParentCategoryTitle;
                model.Category.CategoryDescription = topicCategory.ResourceCategoryDescription;

                var orderByList = InitOrderBy(orderBy, topicCategory.ResourceCategoryUrl);
                model.OrderBy = orderByList;

                DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();

				var recentlyAddedResources = dynamicModuleManager.GetDataItems(handBookResourcesType)
					.Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live)
					.Where(r => r.GetValue<IList<Guid>>("Category").Contains(categoryID));

				if (orderBy == OrderByMostRecent)
                {
                    recentlyAddedResources = recentlyAddedResources.OrderByDescending(r => r.DateCreated);
                }
                else if (orderBy == OrderByMostPopular)
                {
                    recentlyAddedResources = recentlyAddedResources.OrderByDescending(r => r.GetValue<decimal?>("AmountOfLikes"));
                }
                else if (orderBy == OrderByAlphabeticalAZ)
                {
                    recentlyAddedResources = recentlyAddedResources.OrderBy(r => r.GetValue<string>("Title"));
                }
                else if (orderBy == OrderByAlphabeticalZA)
                {
                    recentlyAddedResources = recentlyAddedResources.OrderByDescending(r => r.GetValue<string>("Title"));
                }

                var recentlyAddedResourcesList = recentlyAddedResources.ToList();

                var listOfMyResources = new List<IAFCHandBookResourceModel>();
                TimeSpan totalDuration = new TimeSpan(0, 0, 0);
                int resourcesAmount = 0;

                foreach (var res in recentlyAddedResourcesList)
                {
                    var handBookResource = GetResourceDetailsNext(res);
                    listOfMyResources.Add(handBookResource);
                    totalDuration = totalDuration.Add(handBookResource.ResourceDetails.Duration);
                    resourcesAmount++;
                }

                model.Resources = listOfMyResources;
                model.Category.ResourcesTotalDuration = totalDuration.ToString();
                model.Category.ResourcesAmount = resourcesAmount;
                model.MoreCategories = GetMoreCategories(model.Category.Id);
            }
            catch (Exception e)
            {
                log.Error($@"{nameof(GetResourcesPerCategoryNext)} Error: {e.Message}");
            }

            return model;
        }
		#endregion GetResourcesPerCategoryNext

		#region GetResourceDetailsNext
		public IAFCHandBookResourceModel GetResourceDetailsNext(DynamicContent resource, bool isMyHandBookItem = false)
        {
            var handBookResourceModel = new IAFCHandBookResourceModel(resource.Id,
                resource.GetValue("Title").ToString(),
                resource.DateCreated);

            try
            {
                handBookResourceModel.ResourceDetails = GetResourceDetailsInfoNext(resource, isMyHandBookItem);

                if (isMyHandBookItem)
                {
                    handBookResourceModel.ResourceUrl = handBookResourceModel.ResourceDetails.Category.MyHandBookCategoryUrl + "/resourcedetails/" + resource.UrlName.ToString();
                }
                else
                {
                    handBookResourceModel.ResourceUrl = handBookResourceModel.ResourceDetails.Category.CategoryUrl + "/resourcedetails/" + resource.UrlName.ToString();
                }

                var handBookLikeModel = new IAFCHandBookLikesModel
                {
                    Likes = Convert.ToInt32(resource.GetValue("AmountOfLikes")),
                    Dislikes = Convert.ToInt32(resource.GetValue("AmountOfDislikes"))
                };

                handBookResourceModel.Likes = handBookLikeModel;
                handBookResourceModel.CommentsAmount = Convert.ToInt32(resource.GetValue("AmountOfComments"));
                var resourceCompleted = IsResourceMarkedAsComplete(handBookResourceModel.Id);
                handBookResourceModel.IsResourceAddedToMyHandBook = IsResourceAddedToMyHandBook(handBookResourceModel.Id) || resourceCompleted;
                handBookResourceModel.IsResourceCompleted = resourceCompleted;
            }
            catch (Exception e)
            {
                log.Error($@"{nameof(GetResourceDetailsNext)} Error: {e.Message}");
            }

            return handBookResourceModel;
        }
		#endregion GetResourceDetailsNext

		#region GetResourceDetailsInfoNext
		public IAFCHandBookResourceDetailsModel GetResourceDetailsInfoNext(DynamicContent resource, bool isMyHandBookItem = false)
        {
            var resourceInfo = new IAFCHandBookResourceDetailsModel();

            string resourceTypeTitle = string.Empty;
            string resourceCategoryTitle = string.Empty;
            string resourceCategoryUrl = string.Empty;
            string resourceParentCategoryTitle = string.Empty;
            string resourceParentCategoryUrl = string.Empty;
            string myHandbookResourceCategoryUrl = string.Empty;
            string myHandbookResourceParentCategoryUrl = string.Empty;

            // Get single field
            // !!! resourceInfo.id = externalResourceItem.Id; todo: Fill id with proper data if it is needed somewhere.
            resourceInfo.ResourceTitle = resource.GetValue("Title").ToString();
            resourceInfo.ResourceSummary = resource.GetValue("shortsummary").ToString();
            resourceInfo.ResourceDescription = resource.GetValue("ResourceDescription").ToString();
            resourceInfo.Duration = ParseTime(resource.GetValue("Time").ToString());
            resourceInfo.DurationStr = resourceInfo.Duration.ToString();
            resourceInfo.VideoEmbedCode = resource.GetValue("VideoEmbedCode").ToString();

            TaxonomyManager taxonomyManager = TaxonomyManager.GetManager();
            // get first resource type
            var resourceTypesID = resource.GetPropertyValue<TrackedList<Guid>>("resourcetypes").First();
            if (resourceTypesID != null)
            {
                resourceTypeTitle = taxonomyManager.GetTaxon(resourceTypesID).Title.ToString();
            }

            // get first category
            var resourceCategoriesIDs = resource.GetPropertyValue<TrackedList<Guid>>("Category");
            var categoryItem = resourceCategoriesIDs.Where(c => topicCategories.Contains(c)).First();
            if (categoryItem != null)
            {
                var topicCategory = GetTopicCategories(categoryItem);
                resourceCategoryTitle = topicCategory.ResourceCategoryTile;
                resourceParentCategoryTitle = topicCategory.ResourceParentCategoryTitle;
                resourceParentCategoryUrl = topicCategory.ResourceParentCategoryUrl;
                resourceCategoryUrl = topicCategory.ResourceCategoryUrl;
                if (isMyHandBookItem)
                {
                    myHandbookResourceCategoryUrl = topicCategory.MyHandbookResourceCategoryUrl;
                    myHandbookResourceParentCategoryUrl = topicCategory.MyHandbookResourceParentCategoryUrl;
                }
            }

            resourceInfo.Category.Id = categoryItem;
            resourceInfo.Category.CategoryTitle = resourceCategoryTitle;
            resourceInfo.Category.CategoryUrl = resourceCategoryUrl;
            resourceInfo.Category.ParentCategoryTitle = resourceParentCategoryTitle;
            resourceInfo.Category.ParentCategoryUrl = resourceParentCategoryUrl;
            resourceInfo.Category.MyHandBookCategoryUrl = myHandbookResourceCategoryUrl;
            resourceInfo.Category.MyHandBookParentCategoryUrl = myHandbookResourceParentCategoryUrl;
            resourceInfo.ResourceType = resourceTypeTitle;

            var img = resource.GetRelatedItems<Image>("featuredimage").FirstOrDefault();
            if (img != null)
            {
                resourceInfo.ImageUrl = img.Url;
            }

            return resourceInfo;
        }
		#endregion GetResourceDetailsInfoNext

		#region ParseTime
		private TimeSpan ParseTime(string timeStr)
        {
            TimeSpan duration = new TimeSpan(0, 0, 0);

            try
            {
                timeStr = timeStr.Trim();

                if (timeStr != String.Empty)
                {
                    var durationList = timeStr.Split(',');
                    switch (durationList.Count())
                    {
                        case 1:
                            {
                                duration = new TimeSpan(0, 0, int.Parse(durationList[0]));
                                break;
                            }
                        case 2:
                            {
                                duration = new TimeSpan(0, int.Parse(durationList[0]), int.Parse(durationList[1]));
                                break;
                            }
                        case 3:
                            {
                                duration = new TimeSpan(int.Parse(durationList[0]), int.Parse(durationList[1]), int.Parse(durationList[2]));
                                break;
                            }
                    }
                }
            }
            catch (Exception)
            {
                log.Error("Can not parse duration time = '" + timeStr + "'.");
            }

            return duration;
        }
		#endregion ParseTime

		#region GetResourcesAmountPerCategoryNext
		private int GetResourcesAmountPerCategoryNext(Guid categoryId)
        {
            DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();

            int amount = dynamicModuleManager.GetDataItems(handBookResourcesType)
                .Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live)
                .Where(d => d.GetValue<IList<Guid>>("Category").Contains(categoryId))
                .Count();

            return amount;
        }
		#endregion GetResourcesAmountPerCategoryNext

		#region GetRelatedResourceFieldName
		private string GetRelatedResourceFieldName(Type resourceType)
        {
            var relationFieldName = string.Empty;

            if (resourceType == ExternalResourcesType)
            {
                relationFieldName = "ExternalResources";
            }
            else if (resourceType == ResourceType)
            {
                relationFieldName = "Resources";
            }
            else
            {
                throw new ArgumentException("Invalid type passed.", nameof(resourceType));
            }

            return relationFieldName;
        }
		#endregion GetRelatedResourceFieldName
	}
}