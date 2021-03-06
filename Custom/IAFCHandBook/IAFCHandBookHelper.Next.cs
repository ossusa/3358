﻿using SitefinityWebApp.Mvc.Models;
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
using Telerik.Sitefinity.Taxonomies.Model;

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

				var likeTitle = $@"{title}_{"HR_like"}";

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
				likeMaster.SetWorkflowStatus(dynamicModuleManager.Provider.ApplicationName, "Published");

				resDataMaster.CreateRelation(resMaster, relationFieldName);
				resDataMaster.CreateRelation(likeMaster, "Likes");

				dynamicModuleManager.Lifecycle.Publish(resDataMaster);
				resDataMaster.SetWorkflowStatus(dynamicModuleManager.Provider.ApplicationName, "Published");

				TransactionManager.CommitTransaction(transactionName);

				//SendNotification(resDataMaster.Id);
			}
			catch (Exception e)
			{
				log.Error($@"{nameof(CreateIAFCHandBookResourcesData)} Error: {e.StackTrace}");
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


				var likeExists = resDataMaster.GetRelatedItems<DynamicContent>("Likes").ToList().Any();
				if(!likeExists)
				{
					var likeTitle = resDataTemp.GetValue("Title") + "HR_like";
					
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
					likeMaster.SetWorkflowStatus(dynamicModuleManager.Provider.ApplicationName, "Published");
					resDataTemp.CreateRelation(likeMaster, "Likes");

				}

				resDataTemp.UrlName = new Lstring(Regex.Replace(title, UrlNameCharsToReplace, UrlNameReplaceString));
				resDataTemp.PublicationDate = currentUtcDateTime;
				resDataTemp.LastModified = currentUtcDateTime;
				resDataTemp.LastModifiedBy = SecurityManager.GetCurrentUserId();

				resDataMaster = dynamicModuleManager.Lifecycle.CheckIn(resDataTemp) as DynamicContent;
				dynamicModuleManager.Lifecycle.Publish(resDataMaster);
				
				resDataMaster.SetWorkflowStatus(dynamicModuleManager.Provider.ApplicationName, "Published");

				dynamicModuleManager.SaveChanges();
			}
			catch (Exception e)
			{
				log.Error($@"{nameof(UpdateIAFCHandBookResourcesData)} Error: {e.StackTrace}");
			}
		}
		#endregion UpdateIAFCHandBookResourcesData


		#region UbpublishIAFCHandBookResourcesData
		public void UbpublishIAFCHandBookResourcesData(Guid resourceLiveId, Type resourceType)
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

				DynamicContent resLiveItem = dynamicModuleManager.Lifecycle.GetLive(resData) as DynamicContent;
				dynamicModuleManager.Lifecycle.Unpublish(resLiveItem);
				resData.SetWorkflowStatus(dynamicModuleManager.Provider.ApplicationName, "Unpublished");				

				dynamicModuleManager.SaveChanges();
			}
			catch (Exception e)
			{
				log.Error($@"{nameof(UbpublishIAFCHandBookResourcesData)} Error: {e.StackTrace}");
			}
		}
		#endregion UbpublishIAFCHandBookResourcesData

		#region DeleteIAFCHandBookResourcesData
		public void DeleteIAFCHandBookResourcesData(Guid resourceID, Type resourceType)
		{
			try
			{
				var relationFieldName = GetRelatedResourceFieldName(resourceType);
				
				var providerName = string.Empty;
				var transactionName = "createIAFCHandBookResourcesDataTransaction";								
				DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager(providerName, transactionName);

				var res = dynamicModuleManager.GetDataItem(resourceType, resourceID);

				var resDataList = res.GetRelatedParentItems(handBookResourcesType.FullName, null, relationFieldName);
				var resData = resDataList.First() as DynamicContent;
				var likeExists = resData.GetRelatedItems<DynamicContent>("Likes").ToList().Any();
				if (likeExists)
				{
					var likeItem =resData.GetRelatedItems<DynamicContent>("Likes").ToList().First();
					var likeMaster = dynamicModuleManager.Lifecycle.GetMaster(likeItem) as DynamicContent;
					dynamicModuleManager.Provider.DeleteDataItem(likeMaster);

				}
				DynamicContent resDataMaster = dynamicModuleManager.Lifecycle.GetMaster(resData) as DynamicContent;
				dynamicModuleManager.Provider.DeleteDataItem(resDataMaster);

				//dynamicModuleManager.SaveChanges();
				TransactionManager.CommitTransaction(transactionName);
			}
			catch (Exception e)
			{
				log.Error($@"{nameof(DeleteIAFCHandBookResourcesData)} Error: {e.StackTrace}");
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
				.Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live )				
				.Where(r => r.GetValue<IList<Guid>>("feeding").Contains(ProjectVWSARITGuid))
				.OrderByDescending(d => d.DateCreated)
				.Take(6)
				.ToList();

			var listOfMyResources = new List<IAFCHandBookResourceModel>();

			foreach (var res in recentlyAddedResources)
			{
				var handBookResource = GetResourceDetailsNext(res, null);
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
					.Where(d => d.GetValue<IList<Guid>>("feeding").Contains(ProjectVWSARITGuid))
					.OrderByDescending(d => d.DateCreated)
					.FirstOrDefault();

				if (featuredResource != null)
				{
					handBookResourceModel = GetResourceDetailsNext(featuredResource,null);
				}
			}
			catch (Exception e)
			{
				log.Error($@"{nameof(GetFeaturedResourcesDataNext)} Error: {e.StackTrace}");
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
				if(categoryID==Guid.Empty)
				{
					return null;
				}
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
					.Where(d => d.GetValue<IList<Guid>>("feeding").Contains(ProjectVWSARITGuid))
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
					var handBookResource = GetResourceDetailsNext(res, categoryID, false, null, orderBy);
					listOfMyResources.Add(handBookResource);
					totalDuration = totalDuration.Add(handBookResource.ResourceDetails.Duration);
					resourcesAmount++;
				}

				model.Resources = listOfMyResources;
				model.Category.ResourcesTotalDuration = totalDuration.ToString();
				model.Category.TotalDuration = totalDuration;
				model.Category.ResourcesAmount = resourcesAmount;
				model.MoreCategories = GetMoreCategories(model.Category.Id, null);
				model.IsCategoryFollowed = IsCategoryFollowed(model.Category.Id);
				model.IsAllResourcesAddedToMyHandBook = IsAllResourcesAddedToMyHandBook(model.Category.Id);
				model.IsUserAuthorized = IsUserAuthorized();
			}
			catch (Exception e)
			{
				log.Error($@"{nameof(GetResourcesPerCategoryNext)} Error: {e.StackTrace}");
			}

			return model;
		}
		#endregion GetResourcesPerCategoryNext

		#region GetMoreResourcesNext
		public List<IAFCHandBookMoreResourcesModel> GetMoreResourcesNext(Guid resourceId, Guid categoryID)
		{
			List<IAFCHandBookMoreResourcesModel> moreResources = new List<IAFCHandBookMoreResourcesModel>();

			DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();

			var moreResourcesArray = dynamicModuleManager.GetDataItems(handBookResourcesType)
				.Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live)
				.Where(d => d.GetValue<IList<Guid>>("feeding").Contains(ProjectVWSARITGuid))
				.Where(d => d.GetValue<IList<Guid>>("Category").Contains(categoryID))
				.Where(d => d.Id != resourceId)
				.OrderByDescending(r => r.DateCreated)
				.Take(5)
				.ToArray();

			foreach (var resItem in moreResourcesArray)
			{
				var moreResourcesItem = new IAFCHandBookMoreResourcesModel();
				moreResourcesItem.Id = resItem.Id;
				moreResourcesItem.ResourceDetails = GetResourceDetailsInfo(resItem, categoryID);
				

				var likesModel = new IAFCHandBookLikesModel
				{
					Likes = Convert.ToInt32(resItem.GetValue("AmountOfLikes")),
					Dislikes = Convert.ToInt32(resItem.GetValue("AmountOfDislikes")),
					IsResourceLiked = IsResourceLiked(resItem.GetRelatedItems("Likes").First().Id),
					IsResourceDisliked = IsResourceDisliked(resItem.GetRelatedItems("Likes").First().Id)
				};
				
				moreResourcesItem.Likes = likesModel;

				moreResourcesItem.ResourceUrl = moreResourcesItem.ResourceDetails.Category.CategoryUrl + "/resourcedetails/" + resItem.UrlName.ToString();
				if (moreResourcesItem.ResourceDetails.IsResourceHasMoreThen1Category)
				{
					TaxonomyManager taxonomyManager = TaxonomyManager.GetManager();
					var categoryName = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(t => t.Id == categoryID).Select(t => t.Name).FirstOrDefault();

					moreResourcesItem.ResourceUrl = moreResourcesItem.ResourceUrl + "," + categoryName;
				}
				moreResources.Add(moreResourcesItem);
			}

			return moreResources;
		}
		#endregion GetMoreResourcesNext

		#region GetResourceDetailsNext
		public IAFCHandBookResourceModel GetResourceDetailsNext(DynamicContent resource, Guid? categoryId, bool isMyHandBookItem = false, string userId = null, string orderBy= OrderByMostPopular)
		{
			var handBookResourceModel = new IAFCHandBookResourceModel(resource.Id,
				resource.GetValue("Title").ToString(),
				resource.DateCreated);
			
			try
			{				
				String sharedUrl = String.Empty;
				if (userId != null)
				{
					sharedUrl = "/" + userId;
					var userGuid = Guid.Parse(userId);
					handBookResourceModel.SharedUser = GetUserName(Guid.Parse(userId)) + "'s";
					handBookResourceModel.SharedUserId = Guid.Parse(userId);
				}	
				
				// get first category
				handBookResourceModel.ResourceDetails = GetResourceDetailsInfoNext(resource, categoryId, isMyHandBookItem, sharedUrl);

				if (isMyHandBookItem)
				{
					if (userId != null)
					{
						handBookResourceModel.ResourceUrl = handBookResourceModel.ResourceDetails.Category.MyHandBookCategoryUrl.Substring(0, handBookResourceModel.ResourceDetails.Category.MyHandBookCategoryUrl.Length-37) 
							+ "/resourcedetails/" + resource.UrlName.ToString();
					}
					else
					{
						handBookResourceModel.ResourceUrl = handBookResourceModel.ResourceDetails.Category.MyHandBookCategoryUrl + "/resourcedetails/" + resource.UrlName.ToString();
					}
				}
				else
				{
					if (userId != null)
					{
						handBookResourceModel.ResourceUrl = handBookResourceModel.ResourceDetails.Category.CategoryUrl.Substring(0, handBookResourceModel.ResourceDetails.Category.CategoryUrl .Length-37) 
							+ "/resourcedetails/" + resource.UrlName.ToString();
					}
					else
					{
						handBookResourceModel.ResourceUrl = handBookResourceModel.ResourceDetails.Category.CategoryUrl + "/resourcedetails/" + resource.UrlName.ToString();
					}
				}


				if (handBookResourceModel.ResourceDetails.IsResourceHasMoreThen1Category && categoryId != null)
				{

					TaxonomyManager taxonomyManager = TaxonomyManager.GetManager();
					var categoryName = taxonomyManager.GetTaxon(categoryId.Value).Name;
					handBookResourceModel.ResourceUrl = handBookResourceModel.ResourceUrl + "," + categoryName;
					if (orderBy != OrderByMostPopular)
					{
						handBookResourceModel.ResourceUrl = handBookResourceModel.ResourceUrl + "!" + orderBy;
						handBookResourceModel.OrderBy = orderBy;
					}
				}
				else
				{
					if (orderBy != OrderByMostPopular)
					{
						handBookResourceModel.ResourceUrl = handBookResourceModel.ResourceUrl + "!" + orderBy;
						handBookResourceModel.OrderBy = orderBy;
					}
				}
								
				handBookResourceModel.ResourceUrl = handBookResourceModel.ResourceUrl + sharedUrl;
				
				
				var handBookLikeModel = new IAFCHandBookLikesModel
				{
					Likes = Convert.ToInt32(resource.GetValue("AmountOfLikes")),
					Dislikes = Convert.ToInt32(resource.GetValue("AmountOfDislikes")),
					IsResourceLiked = IsResourceLiked(resource.GetRelatedItems("Likes").First().Id),
					IsResourceDisliked = IsResourceDisliked(resource.GetRelatedItems("Likes").First().Id)
				};

				handBookResourceModel.Likes = handBookLikeModel;
				handBookResourceModel.CommentsAmount = Convert.ToInt32(resource.GetValue("AmountOfComments"));
				var resourceCompleted = IsResourceMarkedAsComplete(handBookResourceModel.Id);
				handBookResourceModel.IsResourceAddedToMyHandBook = IsResourceAddedToMyHandBook(handBookResourceModel.Id) || resourceCompleted;
				handBookResourceModel.IsResourceCompleted = resourceCompleted;
				handBookResourceModel.IsUserAuthorized = IsUserAuthorized();
				

			}
			catch (Exception e)
			{
				log.Error($@"{nameof(GetResourceDetailsNext)} Error: {e.StackTrace}");
			}

			return handBookResourceModel;
		}
		#endregion GetResourceDetailsNext

		#region GetResourceDetailsInfoNext
		public IAFCHandBookResourceDetailsModel GetResourceDetailsInfoNext(DynamicContent resource, Guid? categoryId, bool isMyHandBookItem = false, string sharedUrl=null)
		{
			var resourceInfo = new IAFCHandBookResourceDetailsModel();


			string resourceTypeTitle = string.Empty;
			string resourceTypeName = string.Empty;
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
			var resourceTypes = resource.GetPropertyValue<TrackedList<Guid>>("resourcetypes");
			if (resourceTypes.Any())
			{
				var resourceTypesID = resource.GetPropertyValue<TrackedList<Guid>>("resourcetypes").First();

				var resourceType = taxonomyManager.GetTaxon(resourceTypesID);
				resourceTypeTitle = resourceType.Title;
				resourceTypeName = resourceType.Name;

			}

			// get first category
			var resourceCategoriesIDs = resource.GetPropertyValue<TrackedList<Guid>>("Category").ToArray();
			var moreThen1Category = resourceCategoriesIDs.Where(c => topicCategories.Contains(c)).Count() > 1;
			resourceInfo.IsResourceHasMoreThen1Category = moreThen1Category;
			Guid categoryItem;
			if (categoryId != null)
			{
				categoryItem = resourceCategoriesIDs.Where(c => c == categoryId).FirstOrDefault();
				if (categoryItem == Guid.Empty)
				{
					return null;
				}
			}
			else
			{
				categoryItem = resourceCategoriesIDs.Where(c => topicCategories.Contains(c)).First();
			}
			if (categoryItem != null)
			{
				var topicCategory = GetTopicCategories(categoryItem);
				resourceCategoryTitle = topicCategory.ResourceCategoryTile;
				resourceParentCategoryTitle = topicCategory.ResourceParentCategoryTitle;
				resourceParentCategoryUrl = topicCategory.ResourceParentCategoryUrl ;
				resourceCategoryUrl = topicCategory.ResourceCategoryUrl ;
				if (isMyHandBookItem)
				{
					myHandbookResourceCategoryUrl = topicCategory.MyHandbookResourceCategoryUrl ;
					myHandbookResourceParentCategoryUrl = topicCategory.MyHandbookResourceParentCategoryUrl;
				}
			}

			resourceInfo.Category.Id = categoryItem;
			resourceInfo.Category.CategoryTitle = resourceCategoryTitle;
			resourceInfo.Category.CategoryUrl = resourceCategoryUrl + sharedUrl;
			resourceInfo.Category.ParentCategoryTitle = resourceParentCategoryTitle;
			resourceInfo.Category.ParentCategoryUrl = resourceParentCategoryUrl + sharedUrl;
			resourceInfo.Category.MyHandBookCategoryUrl = myHandbookResourceCategoryUrl + sharedUrl;
			resourceInfo.Category.MyHandBookParentCategoryUrl = myHandbookResourceParentCategoryUrl + sharedUrl;
			resourceInfo.ResourceType = resourceTypeTitle;
			resourceInfo.ResourceTypeName = resourceTypeName;

			var img = resource.GetRelatedItems<Image>("featuredimage").FirstOrDefault();
			if (img != null)
			{
				resourceInfo.ImageAlt = img.AlternativeText;
				resourceInfo.ImageUrl = img.Url;
			}
			else
			{
				resourceInfo.ImagePlaceholderUrl = resourceTypeImages["Placeholder"].Url;
				resourceInfo.ImagePlaceholderAlt = resourceTypeImages["Placeholder"].AlternativeText;
				if (resourceTypeImages.ContainsKey(resourceTypeTitle))
				{
					resourceInfo.ImageSvgUrl = resourceTypeImages[resourceTypeTitle].Url;
					resourceInfo.ImageSvgAlt = resourceTypeImages[resourceTypeTitle].AlternativeText;
				}
				else
				{
					resourceInfo.ImageSvgUrl = resourceTypeImages["Article"].Url;
					resourceInfo.ImageSvgAlt = resourceTypeImages["Article"].AlternativeText;
				}
			}

			resourceInfo.ResourceLink = resource.GetValue("ResourceLink").ToString();

			var resourceDocuments = resource.GetRelatedItems<Document>("attachfiles").ToList();
			if (resourceDocuments.Any())
			{
				resourceInfo.ResourceDocument = resourceDocuments.First().Url;
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

                if (timeStr != string.Empty)
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
                log.Error($"Can not parse duration time: {timeStr}.");
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
				.Where(d => d.GetValue<IList<Guid>>("feeding").Contains(ProjectVWSARITGuid))
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

        #region GetMyHandBookNext
        public IAFCHandBookMyHandBookModel GetMyHandBookNext(string userId = null)
        {
            var model = new IAFCHandBookMyHandBookModel();
            try
            {
                DynamicContent myHandBookItem = new DynamicContent();
                String sharedUrl = String.Empty;
                if (userId != null)
                {
                    sharedUrl = "/" + userId;
                    var userGuid = Guid.Parse(userId);
					
                    myHandBookItem = GetMyHandBookByID(userGuid);
					if (myHandBookItem==null)
					{
						return null;
					}
					model.SharedUserId = userGuid;
					model.SharedUser = GetUserName(userGuid)+"'s";
                }
                else
                {
                    myHandBookItem = GetOrCreateMyHandBook();
                }
                log.Info("GetMyHandBook:" + myHandBookItem.Id.ToString());
                model.Id = myHandBookItem.Id;
                model.UserId = SecurityManager.GetCurrentUserId();
                log.Info("GetMyHandBook:" + model.UserId);

                var myHandBookResources = myHandBookItem.GetRelatedItems<DynamicContent>("MyResources")
					.Where(d => d.GetValue<IList<Guid>>("feeding").Contains(ProjectVWSARITGuid))
					.ToList();
                var myCompletedHandBookResources = myHandBookItem.GetRelatedItems<DynamicContent>("MyCompletedResources")
					.Where(d => d.GetValue<IList<Guid>>("feeding").Contains(ProjectVWSARITGuid))
					.ToList();

                var myHandBookResourcesItem = new IAFCHandBookMyHandBookResourceModelModel();
                myHandBookResourcesItem.IncompletedResourcesAmount = myHandBookResources.Count();
                myHandBookResourcesItem.CompletedResourcesAmount = myCompletedHandBookResources.Count();

                var hbResTotalDuration = GetTotalDuration(myHandBookResources);
                var hbComplResTotalDuration = GetTotalDuration(myCompletedHandBookResources);
                var totalDuration = new TimeSpan();
                totalDuration = totalDuration.Add(hbResTotalDuration);
                myHandBookResourcesItem.TotalDuration = myHandBookResourcesItem.TotalDuration.Add(totalDuration);

                foreach (var categoryItem in topicParentCategories)
                {
                    var myChildHandBookResourcesItem = new IAFCHandBookMyHandBookResourceModelModel();
                    var category = new IAFCHandBookTopicCategoryModel();
                    var categoryDetails = GetTopicCategories(categoryItem);
                    category.Id = categoryItem;
                    category.MyHandBookCategoryUrl = categoryDetails.MyHandbookResourceCategoryUrl + sharedUrl;
                    category.MyHandBookParentCategoryUrl = categoryDetails.MyHandbookResourceParentCategoryUrl + sharedUrl;
                    category.TopicCategoryImageUrl = categoryDetails.ResourceParentCategoryImageUrl;
					category.CategorySvg = categoryDetails.CategoryImage.Url;
					category.CategoryAlt = categoryDetails.CategoryImage.AlternativeText;

					category.CategoryTitle = categoryDetails.ResourceCategoryTile;
                    category.CategoryDescription = categoryDetails.ResourceCategoryDescription;

                    var childCategoriesList = GetChildCategories(categoryItem);

                    var categoryResources = myHandBookResources
						.Where(d => d.GetValue<IList<Guid>>("feeding").Contains(ProjectVWSARITGuid))
						.Where(i => i.GetValue<IList<Guid>>("Category").Any(c => childCategoriesList.Contains(c)))						
						.ToList();

                    var categoryCompletedResources = myCompletedHandBookResources
						.Where(d => d.GetValue<IList<Guid>>("feeding").Contains(ProjectVWSARITGuid))
						.Where(i => i.GetValue<IList<Guid>>("Category").Any(c => childCategoriesList.Contains(c)))
                        .ToList();

                    var hbCategoryResTotalDuration = GetTotalDuration(categoryResources);
                    var hbCategoryComplResTotalDuration = GetTotalDuration(categoryCompletedResources);
                    var categoryTotalDuration = new TimeSpan();
                    categoryTotalDuration = categoryTotalDuration.Add(hbCategoryResTotalDuration);
                    category.TotalDuration = category.TotalDuration.Add(categoryTotalDuration);

                    foreach (var childItem in childCategoriesList)
                    {
                        var childCategory = new IAFCHandBookTopicCategoryModel();
                        var childCategoryDetails = GetTopicCategories(childItem);

                        childCategory.Id = childItem;
                        childCategory.MyHandBookCategoryUrl = childCategoryDetails.MyHandbookResourceCategoryUrl + sharedUrl;
                        childCategory.MyHandBookParentCategoryUrl = childCategoryDetails.MyHandbookResourceParentCategoryUrl + sharedUrl;
                        childCategory.TopicCategoryImageUrl = childCategoryDetails.ResourceParentCategoryImageUrl;
                        childCategory.CategoryTitle = childCategoryDetails.ResourceCategoryTile;
                        childCategory.CategoryDescription = childCategoryDetails.ResourceCategoryDescription;
						childCategory.CategorySvg = childCategoryDetails.CategoryImage.Url;
						childCategory.CategoryAlt = childCategoryDetails.CategoryImage.AlternativeText;
						category.ChildCategories.Add(childCategory);
                    }

                    category.MyHandBookCompletedResources = myCompletedHandBookResources
						.Where(d => d.GetValue<IList<Guid>>("feeding").Contains(ProjectVWSARITGuid))
						.Where(i => i.GetValue<IList<Guid>>("Category").Any(c => childCategoriesList.Contains(c)))
                        .Count();

                    category.MyHandBookInCompletedResources = myHandBookResources
						.Where(d => d.GetValue<IList<Guid>>("feeding").Contains(ProjectVWSARITGuid))
						.Where(i => i.GetValue<IList<Guid>>("Category").Any(c => childCategoriesList.Contains(c)))
                        .Count();

                    myChildHandBookResourcesItem.Category = category;
                    myHandBookResourcesItem.MyChildHandBookResources.Add(myChildHandBookResourcesItem);
                }
                model.MyHandBookResurces.Add(myHandBookResourcesItem);
                log.Info("GetMyHandBook: End");
            }
            catch (Exception e)
            {
                log.Error($@"{nameof(GetMyHandBookNext)} Error: {e.StackTrace}");
            }

            return model;
        }
        #endregion GetMyHandBookNext

        #region MyHandBookGetResourcesPerCategoryNext
        public IAFCHandBookMyHandBookModel GetMyHandBookResourcesPerCategoryNext(string categoryName, string userId = null)
        {
            var model = new IAFCHandBookMyHandBookModel();
            try
            {
                var myHandBookItem = new DynamicContent();
                String sharedUrl = String.Empty;
                Boolean showAsMyHandBookItem = true;
                var userGuid = Guid.Empty;
                if (userId == null)
                {
                    myHandBookItem = GetOrCreateMyHandBook();
                }
                else
                {
                    sharedUrl = "/" + userId;
                    showAsMyHandBookItem = true;
                    userGuid = Guid.Parse(userId);
                    model.SharedUserId = userGuid;
					model.SharedUser = GetUserName(userGuid) + "'s";
					myHandBookItem = GetMyHandBookByID(userGuid);
					if (myHandBookItem==null)
					{
						return null; 
					}
                }

                model.Id = myHandBookItem.Id;
                model.UserId = SecurityManager.GetCurrentUserId();

                var myHandBookResourcesItem = new IAFCHandBookMyHandBookResourceModelModel();

                var categoryItem = GetCategoryGuidByName(categoryName);
				if(categoryItem==Guid.Empty)
				{
					return null;
				}
				
                var category = new IAFCHandBookTopicCategoryModel();
                var categoryDetails = GetTopicCategories(categoryItem);
                category.Id = categoryItem;
                category.MyHandBookCategoryUrl = categoryDetails.MyHandbookResourceCategoryUrl + sharedUrl;
                category.CategoryTitle = categoryDetails.ResourceCategoryTile;
                myHandBookResourcesItem.Category = category;
				var otherCategories = topicParentCategories.Where(c => c != category.Id);
				foreach(var otherCategoryItem in otherCategories)
				{
					var otherCategory = new IAFCHandBookTopicCategoryModel();
					var otherCategoryDetails = GetTopicCategories(otherCategoryItem);
					otherCategory.Id = otherCategoryItem;
					otherCategory.MyHandBookCategoryUrl = otherCategoryDetails.MyHandbookResourceCategoryUrl + sharedUrl;
					otherCategory.CategoryTitle = otherCategoryDetails.ResourceCategoryTile;
					myHandBookResourcesItem.MoreCategories.Add(otherCategory);

				}

				myHandBookResourcesItem.SharedUserId = userGuid;
				if (userGuid != null && userGuid != Guid.Empty)
				{
					myHandBookResourcesItem.SharedUser = GetUserName(userGuid) + "'s";
				}

				var myHandBookResources = myHandBookItem.GetRelatedItems<DynamicContent>("MyResources")
					.Where(d => d.GetValue<IList<Guid>>("feeding").Contains(ProjectVWSARITGuid))
					.ToList();
                var myCompletedHandBookResources = myHandBookItem.GetRelatedItems<DynamicContent>("MyCompletedResources")
					.Where(d => d.GetValue<IList<Guid>>("feeding").Contains(ProjectVWSARITGuid))
					.ToList();

                var childCategoriesList = GetChildCategories(categoryItem);

                foreach (var childItem in childCategoriesList)
                {
                    var myChildHandBookResourcesItem = new IAFCHandBookMyHandBookResourceModelModel();

                    var childCategory = new IAFCHandBookTopicCategoryModel();
                    var childCategoryDetails = GetTopicCategories(childItem);

                    childCategory.Id = childItem;
                    childCategory.MyHandBookCategoryUrl = childCategoryDetails.MyHandbookResourceCategoryUrl + sharedUrl;
                    childCategory.MyHandBookParentCategoryUrl = childCategoryDetails.MyHandbookResourceParentCategoryUrl + sharedUrl;
                    childCategory.TopicCategoryImageUrl = childCategoryDetails.ResourceParentCategoryImageUrl;
                    childCategory.CategoryTitle = childCategoryDetails.ResourceCategoryTile;
                    childCategory.CategoryDescription = childCategoryDetails.ResourceCategoryDescription;

                    var categoryResources = myHandBookResources
						.Where(d => d.GetValue<IList<Guid>>("feeding").Contains(ProjectVWSARITGuid))
						.Where(d => d.GetValue<IList<Guid>>("Category").Contains(childItem))
                        .ToList();

                    var categoryCompletedResources = myCompletedHandBookResources
						.Where(d => d.GetValue<IList<Guid>>("feeding").Contains(ProjectVWSARITGuid))
						.Where(d => d.GetValue<IList<Guid>>("Category").Contains(childItem))
                        .ToList();

                    var hbCategoryResTotalDuration = GetTotalDuration(categoryResources);
                    var hbCategoryComplResTotalDuration = GetTotalDuration(categoryCompletedResources);
                    var categoryTotalDuration = new TimeSpan();
                    categoryTotalDuration = categoryTotalDuration.Add(hbCategoryResTotalDuration);
                    childCategory.TotalDuration = childCategory.TotalDuration.Add(categoryTotalDuration);
                    childCategory.MyHandBookCompletedResources = categoryCompletedResources.Count();
                    var myHandBookResourcesAmount = categoryResources.Count();
                    childCategory.MyHandBookInCompletedResources = myHandBookResourcesAmount;

                    myChildHandBookResourcesItem.Category = childCategory;
                    myChildHandBookResourcesItem.SharedUserId = userGuid;
					if (userGuid != null && userGuid != Guid.Empty)
					{
						myChildHandBookResourcesItem.SharedUser = GetUserName(userGuid) + "'s";
					}

					var myChildResourceItem = new IAFCHandBookResourceModel();
                    foreach (var resourceItem in categoryResources.OrderByDescending(r => r.DateCreated).Take(5))
                    {
                        myChildResourceItem = GetResourceDetails(resourceItem, childCategory.Id, showAsMyHandBookItem, userId);
                        myChildResourceItem.MoreThen5Resources = (myHandBookResourcesAmount - 5) < 0 ? 0 : myHandBookResourcesAmount - 5;

                        myChildHandBookResourcesItem.MyResources.Add(myChildResourceItem);
                    }

                    var myChildCompletedResourceItem = new IAFCHandBookResourceModel();
                    foreach (var resourceCompletedItem in categoryCompletedResources.OrderByDescending(r => r.DateCreated).Take(5))
                    {
                        myChildCompletedResourceItem = GetResourceDetails(resourceCompletedItem, childCategory.Id, showAsMyHandBookItem, userId);
                        myChildHandBookResourcesItem.MyCompletedResources.Add(myChildCompletedResourceItem);
                    }

                    myHandBookResourcesItem.MyChildHandBookResources.Add(myChildHandBookResourcesItem);
                }

                model.MyHandBookResurces.Add(myHandBookResourcesItem);

            }
            catch (Exception e)
            {
                log.Error($@"{nameof(GetMyHandBookResourcesPerCategoryNext)} Error: {e.StackTrace}");
            }
            return model;
        }
        #endregion MyHandBookGetResourcesPerCategoryNext

        #region GetCategoryResourcesNext
        public IAFCHandBookMyHandBookResourceModelModel GetCategoryResourcesNext(Guid categoryId, bool showAllResources, string userId = null, string orderBy = OrderByMostRecent)
        {
            var model = new IAFCHandBookMyHandBookResourceModelModel();
            try
            {
                var myHandBookItem = new DynamicContent();
                String sharedUrl = String.Empty;
                model.UserId = SecurityManager.GetCurrentUserId();
                Boolean showAsMyHandBookItem = true;
                if (userId == null)
                {
                    myHandBookItem = GetOrCreateMyHandBook();
                }
                else
                {
                    sharedUrl = "/" + userId;
                    showAsMyHandBookItem = true;
                    var userGuid = Guid.Parse(userId);
                    model.SharedUserId = userGuid;
					model.SharedUser = GetUserName(userGuid) + "'s";

					myHandBookItem = GetMyHandBookByID(userGuid);
					if(myHandBookItem==null)
					{
						return null;
					}
                }

                var myHandBookResources = myHandBookItem.GetRelatedItems<DynamicContent>("MyResources")
					.Where(d => d.GetValue<IList<Guid>>("feeding").Contains(ProjectVWSARITGuid))
					.ToList();
                var myCompletedHandBookResources = myHandBookItem.GetRelatedItems<DynamicContent>("MyCompletedResources")
					.Where(d => d.GetValue<IList<Guid>>("feeding").Contains(ProjectVWSARITGuid))
					.ToList();

                var category = new IAFCHandBookTopicCategoryModel();
                var categoryDetails = GetTopicCategories(categoryId);

                category.Id = categoryId;
                category.MyHandBookCategoryUrl = categoryDetails.MyHandbookResourceCategoryUrl + sharedUrl;
                category.MyHandBookParentCategoryUrl = categoryDetails.MyHandbookResourceParentCategoryUrl + sharedUrl;
                category.TopicCategoryImageUrl = categoryDetails.ResourceParentCategoryImageUrl;
                category.CategoryTitle = categoryDetails.ResourceCategoryTile;
                category.CategoryDescription = categoryDetails.ResourceCategoryDescription;
                category.ParentCategoryTitle = categoryDetails.ResourceParentCategoryTitle;

                var categoryResourcesList = myHandBookResources
                    .Where(i => i.GetValue<IList<Guid>>("Category").Contains(categoryId))
					.Where(d => d.GetValue<IList<Guid>>("feeding").Contains(ProjectVWSARITGuid))
					.ToList();

                var categoryCompletedResourcesList = myCompletedHandBookResources
                    .Where(i => i.GetValue<IList<Guid>>("Category").Contains(categoryId))
					.Where(d => d.GetValue<IList<Guid>>("feeding").Contains(ProjectVWSARITGuid))
					.ToList();

                var categoryResources = new List<DynamicContent>();
                var categoryCompletedResources = new List<DynamicContent>();
				var srtOrderBy = orderBy;
				if(orderBy== null)
				{
					srtOrderBy = OrderByMostRecent;
				}
                if (srtOrderBy == OrderByMostRecent)
                {
                    categoryResources = categoryResourcesList
                        .OrderByDescending(r => r.DateCreated)
                        .ToList();
                    categoryCompletedResources = categoryCompletedResourcesList
                        .OrderByDescending(r => r.DateCreated)
                        .ToList();
                }
                else if (srtOrderBy == OrderByMostPopular)
                {
                    categoryResources = categoryResourcesList
                        .OrderByDescending(r => r.GetValue<decimal?>("AmountOfLikes"))
                        .ToList();
                    categoryCompletedResources = categoryCompletedResourcesList
                        .OrderByDescending(r => r.GetValue<decimal?>("AmountOfLikes"))
                        .ToList();
                }
                else if (srtOrderBy == OrderByAlphabeticalAZ)
                {
                    categoryResources = categoryResourcesList
                        .OrderBy(r => r.GetValue<Lstring>("Title"))
                        .ToList();
                    categoryCompletedResources = categoryCompletedResourcesList
                        .OrderBy(r => r.GetValue<Lstring>("Title"))
                        .ToList();
                }
                else if (srtOrderBy == OrderByAlphabeticalZA)
                {
                    categoryResources = categoryResourcesList
                        .OrderByDescending(r => r.GetValue<Lstring>("Title"))
                        .ToList();
                    categoryCompletedResources = categoryCompletedResourcesList
                        .OrderByDescending(r => r.GetValue<Lstring>("Title"))
                        .ToList();
                }

                var hbCategoryResTotalDuration = GetTotalDuration(categoryResources);
                var hbCategoryComplResTotalDuration = GetTotalDuration(categoryCompletedResources);
                var categoryTotalDuration = new TimeSpan();
                categoryTotalDuration = categoryTotalDuration.Add(hbCategoryResTotalDuration);
                category.TotalDuration = category.TotalDuration.Add(categoryTotalDuration);
                category.MyHandBookCompletedResources = categoryCompletedResources.Count();
                var myHandBookResourcesAmount = categoryResources.Count();
                category.MyHandBookInCompletedResources = myHandBookResourcesAmount;

                model.Category = category;
                model.MoreCategories = GetMoreCategories(categoryId, sharedUrl);

                var myChildResourceItem = new IAFCHandBookResourceModel();
                foreach (var resourceItem in showAllResources ? categoryResources : categoryResources.Take(5))
                {
                    myChildResourceItem = GetResourceDetails(resourceItem, categoryId, showAsMyHandBookItem, userId, srtOrderBy);

                    myChildResourceItem.MoreThen5Resources = (myHandBookResourcesAmount - 5) < 0 ? 0 : myHandBookResourcesAmount - 5;
                    model.MyResources.Add(myChildResourceItem);
                }
				if (userId !=null)
				{
					var myChildCompletedResourceForSharedPageItem = new IAFCHandBookResourceModel();
					foreach (var resourceCompletedItem in showAllResources ? categoryCompletedResources : categoryCompletedResources.Take(5))
					{
						myChildCompletedResourceForSharedPageItem = GetResourceDetails(resourceCompletedItem, categoryId, showAsMyHandBookItem, userId, srtOrderBy);
						model.MyResources.Add(myChildCompletedResourceForSharedPageItem);
					}


					if (srtOrderBy == OrderByMostRecent)
					{
						model.MyResources.OrderByDescending(r => r.DateCreated)
							.ToList();
						
					}
					else if (srtOrderBy == OrderByMostPopular)
					{
						model.MyResources
							.OrderByDescending(r => r.Likes.Likes)
							.ToList();
					}
					else if (srtOrderBy == OrderByAlphabeticalAZ)
					{
						model.MyResources
							.OrderBy(r => r.ResourceTitle)
							.ToList();
						
					}
					else if (srtOrderBy == OrderByAlphabeticalZA)
					{
						model.MyResources
							.OrderByDescending(r => r.ResourceTitle)
							.ToList();
						
					}
				}

                var myChildCompletedResourceItem = new IAFCHandBookResourceModel();
                foreach (var resourceCompletedItem in showAllResources ? categoryCompletedResources : categoryCompletedResources.Take(5))
                {
                    myChildCompletedResourceItem = GetResourceDetails(resourceCompletedItem, categoryId, showAsMyHandBookItem, userId);
                    model.MyCompletedResources.Add(myChildCompletedResourceItem);
                }

                var orderByList = InitOrderBy(srtOrderBy);
                model.OrderBy = orderByList;
            }
            catch (Exception e)
            {
                log.Error($@"{nameof(GetCategoryResourcesNext)} Error: {e.StackTrace}");
            }
            return model;
        }
        #endregion GetCategoryResourcesNext

        #region GetMyHandBookCategoryResourcesListNext
        public List<IAFCHandBookResourceModel> GetMyHandBookCategoryResourcesListNext(Guid categoryId)
        {
            var myHandBookItem = GetOrCreateMyHandBook();

            var myHandBookCategoryResources = myHandBookItem.GetRelatedItems<DynamicContent>("MyResources")
				.Where(d => d.GetValue<IList<Guid>>("feeding").Contains(ProjectVWSARITGuid))
				.Where(i => i.GetValue<IList<Guid>>("Category").Contains(categoryId))
                .ToList();

            var model = GetMyHandBookCategoryResourcesDetails(myHandBookCategoryResources);

            return model;
        }
        #endregion GetMyHandBookCategoryResourcesListNext

        #region Get My Hnadbook ResourceDetails by Name Next
        public IAFCHandBookResourceModel GetMyHnadbookResourceDetailsNext(string name, Guid? categoryId, string userId=null, string orderBy= OrderByMostPopular)
        {
            DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();

			var model = new IAFCHandBookResourceModel();
			var myHandBookItem = new DynamicContent();
			if (userId !=null)
			{				
					myHandBookItem = GetMyHandBookByID(Guid.Parse(userId));
					if (myHandBookItem == null)
					{
						return null;
					}
				model.SharedUser = GetUserName(Guid.Parse(userId)) + "'s";
				model.SharedUserId = Guid.Parse(userId);
			}
			else
			{
				myHandBookItem = GetOrCreateMyHandBook();
			}
			
            var resourceItem = dynamicModuleManager.GetDataItems(handBookResourcesType)
                .Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live)
                .Where(d => d.UrlName == name)
                .FirstOrDefault();			
			if (resourceItem == null)
			{
				return null;
			}

			var myHandBookResources = myHandBookItem.GetRelatedItems<DynamicContent>("MyResources")
					.Where(d => d.GetValue<IList<Guid>>("feeding").Contains(ProjectVWSARITGuid))
					.ToList();
			var myCompletedHandBookResources = myHandBookItem.GetRelatedItems<DynamicContent>("MyCompletedResources")
				.Where(d => d.GetValue<IList<Guid>>("feeding").Contains(ProjectVWSARITGuid))
				.ToList();

			var myHandBookResourcesExisxt = myHandBookResources.Where(r => r.Id == resourceItem.Id).Any();
			var myCompletedHandBookResourcesExisxt = myCompletedHandBookResources.Where(r => r.Id == resourceItem.Id).Any();

			if (!myHandBookResourcesExisxt 	&& !myCompletedHandBookResourcesExisxt)
			{
				return null;
			}

			model = GetResourceDetails(resourceItem, categoryId, true, userId, orderBy);			
            model.MoreResources = GetMyHandBookMoreResources(resourceItem.Id, model.ResourceDetails.Category.Id, userId);
            model.Comments = GetResourceComments(resourceItem);
			
			model.UserId = SecurityManager.GetCurrentUserId();

            return model;
        }
        #endregion Get My Hnadbook ResourceDetails by Name Next

        #region Get My Hand Book MoreResources Next
        public List<IAFCHandBookMoreResourcesModel> GetMyHandBookMoreResourcesNext(Guid resourceId, Guid categoryID, string userId = null)
        {
            List<IAFCHandBookMoreResourcesModel> moreResources = new List<IAFCHandBookMoreResourcesModel>();
            
			var myHandBookItem = new DynamicContent();
			String sharedUrl = String.Empty;
			
			
			if (userId == null)
			{
				myHandBookItem = GetOrCreateMyHandBook();
			}
			else
			{
				sharedUrl = "/" + userId;
				
				
				myHandBookItem = GetMyHandBookByID(Guid.Parse(userId));
				if (myHandBookItem == null)
				{
					return null;
				}
			}

			var myHandBookResources = myHandBookItem.GetRelatedItems<DynamicContent>("MyResources")
                .Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live)
				.Where(d => d.GetValue<IList<Guid>>("feeding").Contains(ProjectVWSARITGuid))
				.Where(d => d.Id != resourceId);

            var myCompletedHandBookResources = myHandBookItem.GetRelatedItems<DynamicContent>("MyCompletedResources")
                .Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live)
				.Where(d => d.GetValue<IList<Guid>>("feeding").Contains(ProjectVWSARITGuid))
				.Where(d => d.Id != resourceId);

            var allMyHandBookResources = myHandBookResources.Union(myCompletedHandBookResources);

            var moreResourcesItemsArray = allMyHandBookResources
                .Where(i => i.GetValue<IList<Guid>>("Category").Contains(categoryID))
				.Where(d => d.GetValue<IList<Guid>>("feeding").Contains(ProjectVWSARITGuid))
				.OrderByDescending(r => r.DateCreated)
                .Take(5)
                .ToArray();

            foreach (var resItem in moreResourcesItemsArray)
            {
                var moreResourcesItem = new IAFCHandBookMoreResourcesModel();
                moreResourcesItem.Id = resItem.Id;
                moreResourcesItem.ResourceDetails = GetResourceDetailsInfo(resItem, categoryID,true);

                var likesModel = new IAFCHandBookLikesModel
                {
                    Likes = Convert.ToInt32(resItem.GetValue("AmountOfLikes")),
                    Dislikes = Convert.ToInt32(resItem.GetValue("AmountOfDislikes")),
					IsResourceLiked = IsResourceLiked(resItem.GetRelatedItems("Likes").First().Id),
					IsResourceDisliked = IsResourceDisliked(resItem.GetRelatedItems("Likes").First().Id)

				};
                moreResourcesItem.Likes = likesModel;

                moreResourcesItem.ResourceUrl = moreResourcesItem.ResourceDetails.Category.MyHandBookCategoryUrl + "/resourcedetails/" + resItem.UrlName.ToString();
				if(moreResourcesItem.ResourceDetails.IsResourceHasMoreThen1Category)
				{
					TaxonomyManager taxonomyManager = TaxonomyManager.GetManager();
					var categoryName = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(t => t.Id == categoryID).Select(t => t.Name).FirstOrDefault();
					moreResourcesItem.ResourceUrl = moreResourcesItem.ResourceUrl + "," + categoryName;
				}
				moreResourcesItem.ResourceUrl = moreResourcesItem.ResourceUrl + sharedUrl;
				moreResources.Add(moreResourcesItem);
            }

            return moreResources;
        }
        #endregion Get My Hand Book MoreResources Next

        #region GetTotalDurationNext
        public TimeSpan GetTotalDurationNext(List<DynamicContent> resources)
        {
            var totalDuration = new TimeSpan();

            foreach (var res in resources)
            {
                var durationStr = res.GetValue("Time").ToString();
                var duration = ParseTime(durationStr);
                totalDuration = totalDuration.Add(duration);
            }

            return totalDuration;
        }
        #endregion GetTotalDurationNext
    }
}