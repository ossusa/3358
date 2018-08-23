﻿using SitefinityWebApp.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Telerik.OpenAccess;
using Telerik.Sitefinity;
using Telerik.Sitefinity.DynamicModules;
using Telerik.Sitefinity.DynamicModules.Model;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Lifecycle;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Publishing;
using Telerik.Sitefinity.RelatedData;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Model;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Taxonomies.Model;
using Telerik.Sitefinity.Utilities.TypeConverters;
using Telerik.Sitefinity.Libraries.Model;
using Telerik.Sitefinity.Data.Linq.Dynamic;
using Telerik.Sitefinity.Data;
using ServiceStack.Logging;
using System.Threading.Tasks;
using Telerik.Sitefinity.Security.Claims;

namespace SitefinityWebApp.Custom.IAFCHandBook
{

	public partial class IAFCHandBookHelper
	{
		private ILog log = LogManager.GetLogger(typeof(IAFCHandBookHelper));

		#region Struct
		private struct Categories
		{
			public Categories(String resourceCategoryTile,
							  String resourceCategoryDescription,
							  String resourceParentCategoryDescription,
							  String resourceParentCategoryUrl,
							  String resourceParentCategoryTitle,
							  String resourceParentCategoryImageUrl,
							  String resourceCategoryUrl,
							  String myHandbookResourceParentCategoryUrl,
							  String myHandbookResourceCategoryUrl)
			{			
				ResourceCategoryTile = resourceCategoryTile;
				ResourceCategoryUrl = resourceCategoryUrl;
				ResourceParentCategoryUrl = resourceParentCategoryUrl;
				ResourceParentCategoryTitle = resourceParentCategoryTitle;
				ResourceParentCategoryImageUrl = resourceParentCategoryImageUrl;
				MyHandbookResourceCategoryUrl = myHandbookResourceCategoryUrl;
				MyHandbookResourceParentCategoryUrl = myHandbookResourceParentCategoryUrl;
				ResourceCategoryDescription = resourceCategoryDescription;
				ResourceParentCategoryDescription = resourceParentCategoryDescription;
			}

			public String ResourceCategoryTile { get; set; }
			public String ResourceCategoryUrl { get; set; }
			public String ResourceParentCategoryUrl { get; set; }
			public String ResourceParentCategoryTitle { get; set; }
			public String ResourceParentCategoryImageUrl { get; set; }
			public String MyHandbookResourceCategoryUrl { get; set; }
			public String MyHandbookResourceParentCategoryUrl { get; set; }
			public String ResourceCategoryDescription { get; set; }
			public String ResourceParentCategoryDescription { get; set; }

		}
		#endregion Struct

		#region Constants

		private const string UrlNameCharsToReplace = @"[^\w\-\!\$\'\(\)\=\@\d_]+";
		private const string UrlNameReplaceString = "-";
		private const string durationFormat = "hh\\:mm:\\ss";

		private const string LeadershipCategoryTitle = "Leadership";
		private const string CommunityCategoryTitle = "Community";
		private const string FinanceCategoryTitle = "Finance";
		private const string PersonnelCategoryTitle = "Personnel";

		private const string OrderByMostPopular = "MostPopular";
		private const string OrderByMostRecent = "MostRecent";
		private const string OrderByAlphabeticalAZ = "AlphabeticalAZ";
		private const string OrderByAlphabeticalZA = "AlphabeticalZA";

		private const string commentResource = "Comment";
		private const string resourceResource = "Resource";
		private const string topicMenuUrl = "topics";
		private const string MyHandBookMenuUrl = "my-handbook";

		#region DynamicTypes
		private Type handBookResourcesType = TypeResolutionService.ResolveType("Telerik.Sitefinity.DynamicTypes.Model.IAFCHandBookResourcesData.Iafchandbookresourcesdata");
		private Type externalResourcesType = TypeResolutionService.ResolveType("Telerik.Sitefinity.DynamicTypes.Model.HandbookResources.ResourcesExternal");
		private Type resourceType = TypeResolutionService.ResolveType("Telerik.Sitefinity.DynamicTypes.Model.Resources.Resource");
		private Type resourceLikesType = TypeResolutionService.ResolveType("Telerik.Sitefinity.DynamicTypes.Model.IAFCHandBookLikes.Iafchandbooklikes");
		private Type resourceCommentsType = TypeResolutionService.ResolveType("Telerik.Sitefinity.DynamicTypes.Model.IAFCHandBookComments.Iafchandbookcomments");
		private Type myHandBookType = TypeResolutionService.ResolveType("Telerik.Sitefinity.DynamicTypes.Model.IAFCMyHandBook.Iafcmyhandbook");
		#endregion DynamicTypes

		#region CategoriesName
		public const String LeadershipMotivatingPeopleName = "motivating-people";
		public const String LeadershipLeadershipStylesName = "leadership-styles";
		public const String LeadershipEthicsName = "ethics";
		public const String LeadershipGenerationalDifferencesName = "generational-differences";
		public const String LeadershipStrategyName = "strategy";
		public const String PersonnelVolunteerCareerRelationsName = "volunteer-career-relations";
		public const String PersonnelRecruitmentName = "recruitment2";
		public const String PersonnelRetentionName = "retention2";
		public const String PersonnelLegalIssuesName = "legal-issues-personnel";
		public const String PersonnelInsuranceName = "insurance";
		public const String FinanceBudgetingName = "budgeting";
		public const String FinanceFundraisingName = "fundraising";
		public const String FinanceLegalIssuesName = "legal-issues-finance";
		public const String CommunityRelationsCustomerServiceName = "customer-service";
		public const String CommunityRelationsMarketingMediaName = "marketing-media";
		public const String CommunityRelationsPoliticsName = "politics";
		public const String CommunityRelationsCrisisCommunicationName = "crisis-communication";

		public const String Featured_VWS_A_RITName = "featured-vws-a-rit";
		public const String LeadershipName = "leadership";
		public const String PersonnelName = "personnel";
		public const String FinanceName = "finance";
		public const String CommunityName = "community-relations";
		public const String DepartmentAdministrationName = "department-administration";
		#endregion CategoriesName

		#region Urls
		//Urls
		private const string TopicCommynityUrl = "/iafchandbookhome/topics/community";
		private const string TopicCommynityCrisisCommunicationUrl = "/iafchandbookhome/topics/community/crisis-communication";
		private const string TopicCommynityCustomerServiceUrl = "/iafchandbookhome/topics/community/customer-service";
		private const string TopicCommynityMarketingAndMediaUrl = "/iafchandbookhome/topics/community/marketing-and-media";
		private const string TopicCommynityPoliticsUrl = "/iafchandbookhome/topics/community/politics";
		private const string TopicLeadershipUrl = "/iafchandbookhome/topics/leadership";
		private const string TopicLeadershipEthicsUrl = "/iafchandbookhome/topics/leadership/ethics";
		private const string TopicLeadershipGenerationalDifferencesUrl = "/iafchandbookhome/topics/leadership/generational-differences";
		private const string TopicLeadershipLeadershipStylesUrl = "/iafchandbookhome/topics/leadership/leadership-styles";
		private const string TopicLeadershipMotivatingPeopleUrl = "/iafchandbookhome/topics/leadership/motivating-people";
		private const string TopicLeadershipStrategyUrl = "/iafchandbookhome/topics/leadership/strategy";
		private const string TopicFinanceUrl = "/iafchandbookhome/topics/finance";
		private const string TopicFinanceBudgetingUrl = "/iafchandbookhome/topics/finance/budgeting";
		private const string TopicFinanceFundraisingUrl = "/iafchandbookhome/topics/finance/fundraising";
		private const string TopicFinanceLegalIssuesUrl = "/iafchandbookhome/topics/finance/legal-issues";
		private const string TopicPersonnelUrl = "/iafchandbookhome/topics/personnel";
		private const string TopicPersonnelInsuranceUrl = "/iafchandbookhome/topics/personnel/insurance";
		private const string TopicPersonnelLegalIssuesUrl = "/iafchandbookhome/topics/personnel/legal-issues";
		private const string TopicPersonnelRecruitmentUrl = "/iafchandbookhome/topics/personnel/recruitment";
		private const string TopicPersonnelRetentionUrl = "/iafchandbookhome/topics/personnel/retention";
		private const string TopicPersonnelVolunteerCareerRelationsUrl = "/iafchandbookhome/topics/personnel/volunteer-career-relations";

		private const string ResourceDetailsUrl = "/iafchandbookhome/iafcresourcedetails/";

		private const string TopicCommynityImageUrl = "/Sitefinity/WebsiteTemplates/IAFCHandBook/App_Themes/IAFCHandBook/images/community-multiply.svg";
		private const string TopicLeadershipImageUrl = "/Sitefinity/WebsiteTemplates/IAFCHandBook/App_Themes/IAFCHandBook/images/leadership-multiply.svg";
		private const string TopicFinanceImageUrl = "/Sitefinity/WebsiteTemplates/IAFCHandBook/App_Themes/IAFCHandBook/images/finance-multiply.svg";
		private const string TopicPersonnelImageUrl = "/Sitefinity/WebsiteTemplates/IAFCHandBook/App_Themes/IAFCHandBook/images/personnel-multiply.svg";
		#endregion Urls

		#endregion Constants

		#region Variables

		#region Guids
		public Guid Featured_VWS_A_RIT = Guid.Empty;

		public Guid LeadershipMotivatingPeople = Guid.Empty;
		public Guid LeadershipLeadershipStyles = Guid.Empty;
		public Guid LeadershipEthics = Guid.Empty;
		public Guid LeadershipGenerationalDifferences = Guid.Empty;
		public Guid LeadershipStrategy = Guid.Empty;

		public Guid PersonnelVolunteerCareerRelations = Guid.Empty;
		public Guid PersonnelRecruitment = Guid.Empty;
		public Guid PersonnelRetention = Guid.Empty;
		public Guid PersonnelLegalIssues = Guid.Empty;
		public Guid PersonnelInsurance = Guid.Empty;

		public Guid FinanceBudgeting = Guid.Empty;
		public Guid FinanceFundraising = Guid.Empty;
		public Guid FinanceLegalIssues = Guid.Empty;

		public Guid CommunityRelationsCustomerService = Guid.Empty;
		public Guid CommunityRelationsMarketingMedia = Guid.Empty;
		public Guid CommunityRelationsPolitics = Guid.Empty;
		public Guid CommunityRelationsCrisisCommunication = Guid.Empty;

		//Parent Categories GUID
		public Guid LeadershipCategory = Guid.Empty;
		public Guid PersonnelCategory = Guid.Empty;
		public Guid FinanceCategory = Guid.Empty;
		public Guid CommunityCategory = Guid.Empty;
		public Guid DepartmentAdministrationCategory = Guid.Empty;
		#endregion Guids

		private List<Guid> topicParentCategories = new List<Guid>();
		private List<Guid> topicCategories = new List<Guid>();
		private List<Guid> topicFinanceCategories = new List<Guid>();
		private List<Guid> topicLeadershipCategories = new List<Guid>();
		private List<Guid> topicPersonnelCategories = new List<Guid>();
		private List<Guid> topicCommunityRelationsCategories = new List<Guid>();
		Dictionary<Guid, Categories> categoriesDictionaly = new Dictionary<Guid, Categories>();
		
		#endregion Variables

		#region Constructor
		public IAFCHandBookHelper()
		{
			
			InitCategoriesGuid();
			InitCategoriesLists();
			InitCategoryDictionary();

		}
		#endregion Constructor

		#region InitCategories

		#region InitCategoriesGuid
		public void InitCategoriesGuid()
		{
			TaxonomyManager taxonomyManager = TaxonomyManager.GetManager();
			//Leadership
			var currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == LeadershipMotivatingPeopleName).First();
			LeadershipMotivatingPeople = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == LeadershipLeadershipStylesName).First();
			LeadershipLeadershipStyles = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == LeadershipEthicsName).First();
			LeadershipEthics = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == LeadershipGenerationalDifferencesName).First();
			LeadershipGenerationalDifferences = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == LeadershipStrategyName).First();
			LeadershipStrategy = currentResourceCategory.Id;

			//Personnel
			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == PersonnelVolunteerCareerRelationsName).First();
			PersonnelVolunteerCareerRelations = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == PersonnelRecruitmentName).First();
			PersonnelRecruitment = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == PersonnelRetentionName).First();
			PersonnelRetention = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == PersonnelLegalIssuesName).First();
			PersonnelLegalIssues = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == PersonnelInsuranceName).First();
			PersonnelInsurance = currentResourceCategory.Id;

			//Finance
			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == FinanceBudgetingName).First();
			FinanceBudgeting = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == FinanceFundraisingName).First();
			FinanceFundraising = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == FinanceLegalIssuesName).First();
			FinanceLegalIssues = currentResourceCategory.Id;

			//Community Relations
			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == CommunityRelationsCustomerServiceName).First();
			CommunityRelationsCustomerService = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == CommunityRelationsMarketingMediaName).First();
			CommunityRelationsMarketingMedia = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == CommunityRelationsPoliticsName).First();
			CommunityRelationsPolitics = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == CommunityRelationsCrisisCommunicationName).First();
			CommunityRelationsCrisisCommunication = currentResourceCategory.Id;

			//Other
			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == Featured_VWS_A_RITName).First();
			Featured_VWS_A_RIT = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == LeadershipName).First();
			LeadershipCategory = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == PersonnelName).First();
			PersonnelCategory = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == FinanceName).First();
			FinanceCategory = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == CommunityName).First();
			CommunityCategory = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == DepartmentAdministrationName).First();
			DepartmentAdministrationCategory = currentResourceCategory.Id;
		}
		#endregion InitCategoriesGuid

		#region GetCategoryGuidByName
		public Guid GetCategoryGuidByName(string categoryName)
		{
			Guid categoryID = Guid.Empty;

			switch (categoryName)
			{
				case LeadershipMotivatingPeopleName:
					categoryID = LeadershipMotivatingPeople;
					break;
				case LeadershipLeadershipStylesName:
					categoryID = LeadershipLeadershipStyles;
					break;
				case LeadershipEthicsName:
					categoryID = LeadershipEthics;
					break;
				case LeadershipGenerationalDifferencesName:
					categoryID = LeadershipGenerationalDifferences;
					break;
				case LeadershipStrategyName:
					categoryID = LeadershipStrategy;
					break;
				case PersonnelVolunteerCareerRelationsName:
					categoryID = PersonnelVolunteerCareerRelations;
					break;
				case PersonnelRecruitmentName:
					categoryID = PersonnelRecruitment;
					break;
				case PersonnelRetentionName:
					categoryID = PersonnelRetention;
					break;
				case PersonnelLegalIssuesName:
					categoryID = PersonnelLegalIssues;
					break;
				case PersonnelInsuranceName:
					categoryID = PersonnelInsurance;
					break;
				case FinanceBudgetingName:
					categoryID = FinanceBudgeting;
					break;
				case FinanceFundraisingName:
					categoryID = FinanceFundraising;
					break;
				case FinanceLegalIssuesName:
					categoryID = FinanceLegalIssues;
					break;
				case CommunityRelationsCustomerServiceName:
					categoryID = CommunityRelationsCustomerService;
					break;
				case CommunityRelationsMarketingMediaName:
					categoryID = CommunityRelationsMarketingMedia;
					break;
				case CommunityRelationsPoliticsName:
					categoryID = CommunityRelationsPolitics;
					break;
				case CommunityRelationsCrisisCommunicationName:
					categoryID = CommunityRelationsCrisisCommunication;
					break;
				case Featured_VWS_A_RITName:
					categoryID = Featured_VWS_A_RIT;
					break;
				case LeadershipName:
					categoryID = LeadershipCategory;
					break;
				case PersonnelName:
					categoryID = PersonnelCategory;
					break;
				case FinanceName:
					categoryID = FinanceCategory;
					break;
				case CommunityName:
					categoryID = CommunityCategory;
					break;
				case DepartmentAdministrationName:
					categoryID = DepartmentAdministrationCategory;
					break;
			}
			return categoryID;
		}
		#endregion GetCategoryGuidByName

		#region InitCategoriesLists
		public void InitCategoriesLists()
		{
			InitTopicParentCategories();
			InitTopicCategories();
			InitTopicLeadershipCategories();
			InitTopicPersonnelCategoriesLists();
			InitTopicFinanceCategories();
			InitTopicCommunityRelationsCategories();
		}
		#endregion InitCategoriesLists

		#region InitTopicParentCategories
		public void InitTopicParentCategories()
		{
			//CommunityRelations
			topicParentCategories.Add(CommunityCategory);
			//Leadership
			topicParentCategories.Add(LeadershipCategory);
			//Finance
			topicParentCategories.Add(FinanceCategory);
			//Personnel
			topicParentCategories.Add(PersonnelCategory);

		}
		#endregion InitTopicParentCategories

		#region InitTopicCategories
		public void InitTopicCategories()
		{
			//Leadership
			topicCategories.Add(LeadershipMotivatingPeople);
			topicCategories.Add(LeadershipLeadershipStyles);
			topicCategories.Add(LeadershipEthics);
			topicCategories.Add(LeadershipGenerationalDifferences);
			topicCategories.Add(LeadershipStrategy);
			//Personnel
			topicCategories.Add(PersonnelVolunteerCareerRelations);
			topicCategories.Add(PersonnelRecruitment);
			topicCategories.Add(PersonnelRetention);
			topicCategories.Add(PersonnelLegalIssues);
			topicCategories.Add(PersonnelInsurance);
			//Finance
			topicCategories.Add(FinanceBudgeting);
			topicCategories.Add(FinanceFundraising);
			topicCategories.Add(FinanceLegalIssues);
			//CommunityRelations
			topicCategories.Add(CommunityRelationsCustomerService);
			topicCategories.Add(CommunityRelationsMarketingMedia);
			topicCategories.Add(CommunityRelationsPolitics);
			topicCategories.Add(CommunityRelationsCrisisCommunication);
		}
		#endregion InitTopicCategories

		#region InitTopicLeadershipCategories
		public void InitTopicLeadershipCategories()
		{
			//Leadership
			topicLeadershipCategories.Add(LeadershipMotivatingPeople);
			topicLeadershipCategories.Add(LeadershipLeadershipStyles);
			topicLeadershipCategories.Add(LeadershipEthics);
			topicLeadershipCategories.Add(LeadershipGenerationalDifferences);
			topicLeadershipCategories.Add(LeadershipStrategy);
		}
		#endregion InitTopicLeadershipCategories

		#region InitTopicPersonnelCategoriesLists
		public void InitTopicPersonnelCategoriesLists()
		{
			//Personnel
			topicPersonnelCategories.Add(PersonnelVolunteerCareerRelations);
			topicPersonnelCategories.Add(PersonnelRecruitment);
			topicPersonnelCategories.Add(PersonnelRetention);
			topicPersonnelCategories.Add(PersonnelLegalIssues);
			topicPersonnelCategories.Add(PersonnelInsurance);
		}
		#endregion InitTopicPersonnelCategoriesLists

		#region InitTopicFinanceCategories
		public void InitTopicFinanceCategories()
		{
			//Finance
			topicFinanceCategories.Add(FinanceBudgeting);
			topicFinanceCategories.Add(FinanceFundraising);
			topicFinanceCategories.Add(FinanceLegalIssues);
		}
		#endregion InitTopicFinanceCategories

		#region InitTopicCommunityRelationsCategories
		public void InitTopicCommunityRelationsCategories()
		{
			//CommunityRelations
			topicCommunityRelationsCategories.Add(CommunityRelationsCustomerService);
			topicCommunityRelationsCategories.Add(CommunityRelationsMarketingMedia);
			topicCommunityRelationsCategories.Add(CommunityRelationsPolitics);
			topicCommunityRelationsCategories.Add(CommunityRelationsCrisisCommunication);
		}
		#endregion InitTopicCommunityRelationsCategories

		#region InitCategoryDictionary
		private void InitCategoryDictionary()
		{
			TaxonomyManager taxonomyManager = TaxonomyManager.GetManager();
			var resourceCategoryItem = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Id == LeadershipCategory).First();
			var resourceCategoryTitle = resourceCategoryItem.Title.ToString();
			var resourceCategoryDescription = resourceCategoryItem.Description.ToString();
			var resourceParenCategoryDescription = resourceCategoryItem.Description.ToString();

			var category = new Categories(resourceCategoryTitle,
				resourceCategoryDescription,
				String.Empty,
				String.Empty,
				String.Empty,
				TopicLeadershipImageUrl,
				TopicLeadershipUrl,
				String.Empty,
				TopicLeadershipUrl.Replace(topicMenuUrl, MyHandBookMenuUrl));
			categoriesDictionaly.Add(LeadershipCategory, category);

			resourceCategoryItem = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Id == LeadershipMotivatingPeople).First();
			resourceCategoryTitle = resourceCategoryItem.Title.ToString();
			resourceCategoryDescription = resourceCategoryItem.Description.ToString();
			category = new Categories(resourceCategoryTitle,
				resourceCategoryDescription,
				resourceParenCategoryDescription,
				TopicLeadershipUrl,
				LeadershipCategoryTitle,
				TopicLeadershipImageUrl,
				TopicLeadershipMotivatingPeopleUrl,
				TopicLeadershipUrl.Replace(topicMenuUrl, MyHandBookMenuUrl),
				TopicLeadershipMotivatingPeopleUrl.Replace(topicMenuUrl, MyHandBookMenuUrl));
			categoriesDictionaly.Add(LeadershipMotivatingPeople, category);

			resourceCategoryItem = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Id == LeadershipLeadershipStyles).First();
			resourceCategoryTitle = resourceCategoryItem.Title.ToString();
			resourceCategoryDescription = resourceCategoryItem.Description.ToString();
			category = new Categories(resourceCategoryTitle,
				resourceCategoryDescription,
				resourceParenCategoryDescription,
				TopicLeadershipUrl,
				LeadershipCategoryTitle,
				TopicLeadershipImageUrl,
				TopicLeadershipLeadershipStylesUrl,
				TopicLeadershipUrl.Replace(topicMenuUrl, MyHandBookMenuUrl),
				TopicLeadershipLeadershipStylesUrl.Replace(topicMenuUrl, MyHandBookMenuUrl));
			categoriesDictionaly.Add(LeadershipLeadershipStyles, category);

			resourceCategoryItem = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Id == LeadershipEthics).First();
			resourceCategoryTitle = resourceCategoryItem.Title.ToString();
			resourceCategoryDescription = resourceCategoryItem.Description.ToString();
			category = new Categories(resourceCategoryTitle,
				resourceCategoryDescription,
				resourceParenCategoryDescription,
				TopicLeadershipUrl,
				LeadershipCategoryTitle,
				TopicLeadershipImageUrl,
				TopicLeadershipEthicsUrl,
				TopicLeadershipUrl.Replace(topicMenuUrl, MyHandBookMenuUrl),
				TopicLeadershipEthicsUrl.Replace(topicMenuUrl, MyHandBookMenuUrl));
			categoriesDictionaly.Add(LeadershipEthics, category);

			resourceCategoryItem = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Id == LeadershipGenerationalDifferences).First();
			resourceCategoryTitle = resourceCategoryItem.Title.ToString();
			resourceCategoryDescription = resourceCategoryItem.Description.ToString();
			category = new Categories(resourceCategoryTitle,
				resourceCategoryDescription,
				resourceParenCategoryDescription,
				TopicLeadershipUrl,
				LeadershipCategoryTitle,
				TopicLeadershipImageUrl,
				TopicLeadershipGenerationalDifferencesUrl,
				TopicLeadershipUrl.Replace(topicMenuUrl, MyHandBookMenuUrl),
				TopicLeadershipGenerationalDifferencesUrl.Replace(topicMenuUrl, MyHandBookMenuUrl));
			categoriesDictionaly.Add(LeadershipGenerationalDifferences, category);

			resourceCategoryItem = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Id == LeadershipStrategy).First();
			resourceCategoryTitle = resourceCategoryItem.Title.ToString();
			resourceCategoryDescription = resourceCategoryItem.Description.ToString();
			category = new Categories(resourceCategoryTitle,
				resourceCategoryDescription,
				resourceParenCategoryDescription,
				TopicLeadershipUrl,
				LeadershipCategoryTitle,
				TopicLeadershipImageUrl,
				TopicLeadershipStrategyUrl,
				TopicLeadershipUrl.Replace(topicMenuUrl, MyHandBookMenuUrl),
				TopicLeadershipStrategyUrl.Replace(topicMenuUrl, MyHandBookMenuUrl));
			categoriesDictionaly.Add(LeadershipStrategy, category);

			resourceCategoryItem = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Id == PersonnelCategory).First();
			resourceCategoryTitle = resourceCategoryItem.Title.ToString();
			resourceCategoryDescription = resourceCategoryItem.Description.ToString();
			resourceParenCategoryDescription = resourceCategoryItem.Description.ToString();
			category = new Categories(resourceCategoryTitle,
				resourceCategoryDescription,
				String.Empty,
				String.Empty,
				String.Empty,
				TopicPersonnelImageUrl,
				TopicPersonnelUrl,
				String.Empty,
				TopicPersonnelUrl.Replace(topicMenuUrl, MyHandBookMenuUrl));
			categoriesDictionaly.Add(PersonnelCategory, category);

			resourceCategoryItem = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Id == PersonnelVolunteerCareerRelations).First();
			resourceCategoryTitle = resourceCategoryItem.Title.ToString();
			resourceCategoryDescription = resourceCategoryItem.Description.ToString();			
			category = new Categories(resourceCategoryTitle,
				resourceCategoryDescription,
				resourceParenCategoryDescription,
				TopicPersonnelUrl,
				PersonnelCategoryTitle,
				TopicPersonnelImageUrl,
				TopicPersonnelVolunteerCareerRelationsUrl,
				TopicPersonnelUrl.Replace(topicMenuUrl, MyHandBookMenuUrl),
				TopicPersonnelVolunteerCareerRelationsUrl.Replace(topicMenuUrl, MyHandBookMenuUrl));
			categoriesDictionaly.Add(PersonnelVolunteerCareerRelations, category);

			resourceCategoryItem = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Id == PersonnelRecruitment).First();
			resourceCategoryTitle = resourceCategoryItem.Title.ToString();
			resourceCategoryDescription = resourceCategoryItem.Description.ToString();
			category = new Categories(resourceCategoryTitle,
				resourceCategoryDescription,
				resourceParenCategoryDescription,
				TopicPersonnelUrl,
				PersonnelCategoryTitle,
				TopicPersonnelImageUrl,
				TopicPersonnelRecruitmentUrl,
				TopicPersonnelUrl.Replace(topicMenuUrl, MyHandBookMenuUrl),
				TopicPersonnelRecruitmentUrl.Replace(topicMenuUrl, MyHandBookMenuUrl));
			categoriesDictionaly.Add(PersonnelRecruitment, category);

			resourceCategoryItem = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Id == PersonnelRetention).First();
			resourceCategoryTitle = resourceCategoryItem.Title.ToString();
			resourceCategoryDescription = resourceCategoryItem.Description.ToString();
			category = new Categories(resourceCategoryTitle,
				resourceCategoryDescription,
				resourceParenCategoryDescription,
				TopicPersonnelUrl,
				PersonnelCategoryTitle,
				TopicPersonnelImageUrl,
				TopicPersonnelRetentionUrl,
				TopicPersonnelUrl.Replace(topicMenuUrl, MyHandBookMenuUrl),
				TopicPersonnelRetentionUrl.Replace(topicMenuUrl, MyHandBookMenuUrl));
			categoriesDictionaly.Add(PersonnelRetention, category);

			resourceCategoryItem = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Id == PersonnelLegalIssues).First();
			resourceCategoryTitle = resourceCategoryItem.Title.ToString();
			resourceCategoryDescription = resourceCategoryItem.Description.ToString();
			category = new Categories(resourceCategoryTitle,
				resourceCategoryDescription,
				resourceParenCategoryDescription,
				TopicPersonnelUrl,
				PersonnelCategoryTitle,
				TopicPersonnelImageUrl,
				TopicPersonnelLegalIssuesUrl,
				TopicPersonnelUrl.Replace(topicMenuUrl, MyHandBookMenuUrl),
				TopicPersonnelLegalIssuesUrl.Replace(topicMenuUrl, MyHandBookMenuUrl));
			categoriesDictionaly.Add(PersonnelLegalIssues, category);

			resourceCategoryItem = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Id == PersonnelInsurance).First();
			resourceCategoryTitle = resourceCategoryItem.Title.ToString();
			resourceCategoryDescription = resourceCategoryItem.Description.ToString();
			category = new Categories(resourceCategoryTitle,
				resourceCategoryDescription,
				resourceParenCategoryDescription,
				TopicPersonnelUrl,
				PersonnelCategoryTitle,
				TopicPersonnelImageUrl,
				TopicPersonnelInsuranceUrl,
				TopicPersonnelUrl.Replace(topicMenuUrl, MyHandBookMenuUrl),
				TopicPersonnelInsuranceUrl.Replace(topicMenuUrl, MyHandBookMenuUrl));
			categoriesDictionaly.Add(PersonnelInsurance, category);

			resourceCategoryItem = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Id == FinanceCategory).First();
			resourceCategoryTitle = resourceCategoryItem.Title.ToString();
			resourceCategoryDescription = resourceCategoryItem.Description.ToString();
			resourceParenCategoryDescription = resourceCategoryItem.Description.ToString();
			category = new Categories(resourceCategoryTitle,
				resourceCategoryDescription,
				String.Empty,
				String.Empty,
				String.Empty,
				TopicFinanceImageUrl,
				TopicFinanceUrl,
				String.Empty,
				TopicFinanceUrl.Replace(topicMenuUrl, MyHandBookMenuUrl));
			categoriesDictionaly.Add(FinanceCategory, category);

			resourceCategoryItem = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Id == FinanceBudgeting).First();
			resourceCategoryTitle = resourceCategoryItem.Title.ToString();
			resourceCategoryDescription = resourceCategoryItem.Description.ToString();
			category = new Categories(resourceCategoryTitle,
				resourceCategoryDescription,
				resourceParenCategoryDescription,
				TopicFinanceUrl,
				FinanceCategoryTitle,
				TopicFinanceImageUrl,
				TopicFinanceBudgetingUrl,
				TopicFinanceUrl.Replace(topicMenuUrl, MyHandBookMenuUrl),
				TopicFinanceBudgetingUrl.Replace(topicMenuUrl, MyHandBookMenuUrl));
			categoriesDictionaly.Add(FinanceBudgeting, category);

			resourceCategoryItem = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Id == FinanceFundraising).First();
			resourceCategoryTitle = resourceCategoryItem.Title.ToString();
			resourceCategoryDescription = resourceCategoryItem.Description.ToString();
			category = new Categories(resourceCategoryTitle,
				resourceCategoryDescription,
				resourceParenCategoryDescription,
				TopicFinanceUrl,
				FinanceCategoryTitle,
				TopicFinanceImageUrl,
				TopicFinanceFundraisingUrl,
				TopicFinanceUrl.Replace(topicMenuUrl, MyHandBookMenuUrl),
				TopicFinanceFundraisingUrl.Replace(topicMenuUrl, MyHandBookMenuUrl));
			categoriesDictionaly.Add(FinanceFundraising, category);

			resourceCategoryItem = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Id == FinanceLegalIssues).First();
			resourceCategoryTitle = resourceCategoryItem.Title.ToString();
			resourceCategoryDescription = resourceCategoryItem.Description.ToString();
			category = new Categories(resourceCategoryTitle,
				resourceCategoryDescription,
				resourceParenCategoryDescription,
				TopicFinanceUrl,
				FinanceCategoryTitle,
				TopicFinanceImageUrl,
				TopicFinanceLegalIssuesUrl,
				TopicFinanceUrl.Replace(topicMenuUrl, MyHandBookMenuUrl),
				TopicFinanceLegalIssuesUrl.Replace(topicMenuUrl, MyHandBookMenuUrl));
			categoriesDictionaly.Add(FinanceLegalIssues, category);

			resourceCategoryItem = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Id == CommunityCategory).First();
			resourceCategoryTitle = CommunityCategoryTitle;
			resourceCategoryDescription = resourceCategoryItem.Description.ToString();
			resourceParenCategoryDescription = resourceCategoryItem.Description.ToString();
			category = new Categories(resourceCategoryTitle,
				resourceCategoryDescription,
				String.Empty,
				String.Empty,
				String.Empty,
				TopicCommynityImageUrl,
				TopicCommynityUrl,
				String.Empty,
				TopicCommynityUrl.Replace(topicMenuUrl, MyHandBookMenuUrl));
			categoriesDictionaly.Add(CommunityCategory, category);

			resourceCategoryItem = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Id == CommunityRelationsCustomerService).First();
			resourceCategoryTitle = resourceCategoryItem.Title.ToString();
			resourceCategoryDescription = resourceCategoryItem.Description.ToString();
			category = new Categories(resourceCategoryTitle,
				resourceCategoryDescription,
				resourceParenCategoryDescription,
				TopicCommynityUrl,
				CommunityCategoryTitle,
				TopicCommynityImageUrl,
				TopicCommynityCustomerServiceUrl,
				TopicCommynityUrl.Replace(topicMenuUrl, MyHandBookMenuUrl),
				TopicCommynityCustomerServiceUrl.Replace(topicMenuUrl, MyHandBookMenuUrl));
			categoriesDictionaly.Add(CommunityRelationsCustomerService, category);

			resourceCategoryItem = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Id == CommunityRelationsMarketingMedia).First();
			resourceCategoryTitle = resourceCategoryItem.Title.ToString();
			resourceCategoryDescription = resourceCategoryItem.Description.ToString();
			category = new Categories(resourceCategoryTitle,
				resourceCategoryDescription,
				resourceParenCategoryDescription,
				TopicCommynityUrl,
				CommunityCategoryTitle,
				TopicCommynityImageUrl,
				TopicCommynityMarketingAndMediaUrl,
				TopicCommynityUrl.Replace(topicMenuUrl, MyHandBookMenuUrl),
				TopicCommynityMarketingAndMediaUrl.Replace(topicMenuUrl, MyHandBookMenuUrl));
			categoriesDictionaly.Add(CommunityRelationsMarketingMedia, category);

			resourceCategoryItem = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Id == CommunityRelationsPolitics).First();
			resourceCategoryTitle = resourceCategoryItem.Title.ToString();
			resourceCategoryDescription = resourceCategoryItem.Description.ToString();
			category = new Categories(resourceCategoryTitle,
				resourceCategoryDescription,
				resourceParenCategoryDescription,
				TopicCommynityUrl,
				CommunityCategoryTitle,
				TopicCommynityImageUrl,
				TopicCommynityPoliticsUrl,
				TopicCommynityUrl.Replace(topicMenuUrl, MyHandBookMenuUrl),
				TopicCommynityPoliticsUrl.Replace(topicMenuUrl, MyHandBookMenuUrl));
			categoriesDictionaly.Add(CommunityRelationsPolitics, category);

			resourceCategoryItem = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Id == CommunityRelationsCrisisCommunication).First();
			resourceCategoryTitle = resourceCategoryItem.Title.ToString();
			resourceCategoryDescription = resourceCategoryItem.Description.ToString();
			category = new Categories(resourceCategoryTitle,
				resourceCategoryDescription,
				resourceParenCategoryDescription,
				TopicCommynityUrl,
				CommunityCategoryTitle,
				TopicCommynityImageUrl,
				TopicCommynityCrisisCommunicationUrl,
				TopicCommynityUrl.Replace(topicMenuUrl, MyHandBookMenuUrl),
				TopicCommynityCrisisCommunicationUrl.Replace(topicMenuUrl, MyHandBookMenuUrl));
			categoriesDictionaly.Add(CommunityRelationsCrisisCommunication, category);
		}
		#endregion InitCategoryDictionary

		#region GetCategories
		private Categories GetTopicCategories(Guid categoryID)
		{
			return categoriesDictionaly[categoryID];
		}
		#endregion GetCategories

		#region GetChildCategories
		public List<Guid> GetChildCategories(Guid categoryID)
		{
			var childCategories = new List<Guid>();

			if (categoryID == CommunityCategory)
			{
				childCategories = topicCommunityRelationsCategories;
			}
			else if (categoryID == LeadershipCategory)
			{
				childCategories = topicLeadershipCategories;
			}
			else if (categoryID == FinanceCategory)
			{
				childCategories = topicFinanceCategories;
			}
			else if (categoryID == PersonnelCategory)
			{
				childCategories = topicPersonnelCategories;
			}
			return childCategories;
		}
		#endregion GetChildCategories

		#endregion InitCategories	

		#region GetResourceLikesInfo
		public IAFCHandBookLikesModel GetResourceLikesInfo(DynamicContent resource, string resourceType = resourceResource)
		{
			DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();
			var resourceLike = new IAFCHandBookLikesModel();

			try
			{
				string commentFieldText = "Likes";
				if (resourceType == commentResource)
				{
					commentFieldText = "CommentLikes";
				}
				var likeModuleData = resource.GetRelatedItems(commentFieldText).Cast<DynamicContent>();
				var like = new DynamicContent();
				if (likeModuleData.Any())
				{
					like = resource.GetRelatedItems(commentFieldText).Cast<DynamicContent>().First();
				}
				else
				{
					//Create like for Resource					
					like = dynamicModuleManager.CreateDataItem(resourceLikesType);
					var liketitle = resource.GetValue("Title").ToString() + "_like";
					like.SetValue("Title", liketitle);
					like.SetValue("AmountOfLikes", 0);
					like.SetValue("AmountOfDislikes", 0);
					like.SetString("UrlName", new Lstring(Regex.Replace(liketitle, UrlNameCharsToReplace, UrlNameReplaceString)));
					like.SetValue("Owner", SecurityManager.GetCurrentUserId());
					like.SetValue("PublicationDate", DateTime.UtcNow);

					dynamicModuleManager.Lifecycle.Publish(like);
					like.SetWorkflowStatus(dynamicModuleManager.Provider.ApplicationName, "Published");

					//Add Like to resource						
					var resourceTypeItem = handBookResourcesType;
					if (resourceType == commentResource)
					{
						resourceTypeItem = resourceCommentsType;
					}

					var liveResource = dynamicModuleManager.GetDataItem(resourceTypeItem, resource.Id);
					var masterResource = dynamicModuleManager.Lifecycle.GetMaster(liveResource);

					DynamicContent checkOutResourceItem = dynamicModuleManager.Lifecycle.CheckOut(masterResource) as DynamicContent;
					checkOutResourceItem.CreateRelation(like, commentFieldText);
					ILifecycleDataItem checkInMyresourcesItem = dynamicModuleManager.Lifecycle.CheckIn(checkOutResourceItem);
					dynamicModuleManager.Lifecycle.Publish(checkInMyresourcesItem);

				}


				resourceLike.LikeTitle = like.GetValue("Title").ToString();
				resourceLike.Likes = Convert.ToInt32(like.GetValue("AmountOfLikes"));
				resourceLike.Dislikes = Convert.ToInt32(like.GetValue("AmountOfDislikes"));


				dynamicModuleManager.SaveChanges();
			}
			catch (Exception e)
			{
				log.Error("Can not get Likes for Resource ID = '" + resource.Id.ToString() + "' " + e.Message);
			}

			return resourceLike;
		}
		#endregion GetResourceLikesInfo

		#region Init Order By
		public List<IAFCHandBookTopicOrderBy> InitOrderBy(String orderBy, String url = "")
		{
			var orderByList = new List<IAFCHandBookTopicOrderBy>();
			var orderByItem = new IAFCHandBookTopicOrderBy();
			orderByItem.Url = url + "/" + OrderByMostPopular;
			orderByItem.Title = OrderByMostPopular;
			if (orderBy == OrderByMostPopular)
			{
				orderByItem.Selected = true;
			}
			orderByList.Add(orderByItem);

			orderByItem = new IAFCHandBookTopicOrderBy();
			orderByItem.Url = url + "/" + OrderByMostRecent;
			orderByItem.Title = OrderByMostRecent;
			if (orderBy == OrderByMostRecent)
			{
				orderByItem.Selected = true;
			}
			orderByList.Add(orderByItem);

			orderByItem = new IAFCHandBookTopicOrderBy();
			orderByItem.Url = url + "/" + OrderByAlphabeticalAZ;
			orderByItem.Title = OrderByAlphabeticalAZ;
			if (orderBy == OrderByAlphabeticalAZ)
			{
				orderByItem.Selected = true;
			}
			orderByList.Add(orderByItem);

			orderByItem = new IAFCHandBookTopicOrderBy();
			orderByItem.Url = url + "/" + OrderByAlphabeticalZA;
			orderByItem.Title = OrderByAlphabeticalZA;
			if (orderBy == OrderByAlphabeticalZA)
			{
				orderByItem.Selected = true;
			}
			orderByList.Add(orderByItem);
			return orderByList;
		}
		#endregion Init Order By

		#region GetResourceComments
		public List<IAFCHandBookCommentModel> GetResourceComments(Guid resourceId, string resourceType = resourceResource)
		{
			var comments = new List<IAFCHandBookCommentModel>();

			var resourceTypeItem = handBookResourcesType;
			if (resourceType == commentResource)
			{
				resourceTypeItem = resourceCommentsType;
			}

			DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();
			var resourceItem = dynamicModuleManager.GetDataItems(resourceTypeItem).
						Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live && d.Id == resourceId).
						First();
			comments = GetResourceComments(resourceItem, resourceType);


			return comments;
		}

		public List<IAFCHandBookCommentModel> GetResourceComments(DynamicContent resource, string resourceType = resourceResource)
		{
			DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();
			UserProfileManager profileManager = UserProfileManager.GetManager();
			UserManager userManager = UserManager.GetManager();



			string commentFieldName = "Comment";
			if (resourceType == commentResource)
			{
				commentFieldName = "Reply";
			}
			var resourceComments = resource.GetRelatedItems(commentFieldName).Cast<DynamicContent>().ToArray();
			var resourceCommentList = new List<IAFCHandBookCommentModel>();
			foreach (var commentItem in resourceComments.OrderByDescending(c => c.DateCreated))
			{
				var commentDetails = new IAFCHandBookCommentModel();
				commentDetails.Id = commentItem.Id;
				commentDetails.CommentText = commentItem.GetValue("CommentText").ToString();
				commentDetails.DateCreated = commentItem.DateCreated;
				commentDetails.Author.Id = commentItem.Owner;
				User user = userManager.GetUser(commentItem.Owner);
				SitefinityProfile profile = null;


				if (user != null)
				{
					profile = profileManager.GetUserProfile<SitefinityProfile>(user);
					if (profile != null)
					{
						commentDetails.Author.UserName = profile.FirstName + " " + profile.LastName;
					}
					else
					{
						
						commentDetails.Author.UserName = user.FirstName + " " + user.LastName;
					}
				}
				commentDetails.Likes = GetResourceLikesInfo(commentItem, commentResource);


				var replyComments = commentItem.GetRelatedItems("Reply").Cast<DynamicContent>().ToArray();
				var replyCommentList = new List<IAFCHandBookCommentModel>();
				foreach (var replyCommentItem in replyComments.OrderByDescending(c => c.DateCreated))
				{
					var replyCommentDetails = new IAFCHandBookCommentModel();
					replyCommentDetails.Id = replyCommentItem.Id;
					replyCommentDetails.CommentText = replyCommentItem.GetValue("CommentText").ToString();
					replyCommentDetails.DateCreated = replyCommentItem.DateCreated;
					replyCommentDetails.Author.Id = replyCommentItem.Owner;
					user = userManager.GetUser(replyCommentItem.Owner);
					profile = null;

					if (user != null)
					{
						profile = profileManager.GetUserProfile<SitefinityProfile>(user);
						if (profile != null)
						{
							commentDetails.Author.UserName = profile.FirstName + " " + profile.LastName;
						}
						else
						{
							commentDetails.Author.UserName = user.FirstName + " " + user.LastName;
						}
					}
					replyCommentDetails.Likes = GetResourceLikesInfo(commentItem, commentResource);

					replyCommentList.Add(replyCommentDetails);

				}

				commentDetails.RepliedComments = replyCommentList;

				resourceCommentList.Add(commentDetails);
			}

			return resourceCommentList;
		}
		#endregion GetResourceComments

		#region GetResourceDetailsInfo

		public IAFCHandBookResourceDetailsModel GetResourceDetailsInfo(DynamicContent resource, bool isMyHandBookItem = false)
		{
            return GetResourceDetailsInfoNext(resource, isMyHandBookItem);
		}

        #endregion GetResourceDetailsInfo

        #region GetResourceDetails

        #region GetResourceDetails by Item

        public IAFCHandBookResourceModel GetResourceDetails(DynamicContent resource, bool isMyHandBookItem = false)
        {
            return GetResourceDetailsNext(resource, isMyHandBookItem);
        }

		#endregion GetResourceDetails by Item

		#region GetResourceDetails by Name
		public IAFCHandBookResourceModel GetResourceDetails(String name)
		{

			DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();
			var resourceItem = dynamicModuleManager.GetDataItems(handBookResourcesType).
						Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live && d.UrlName == name).
						First();

			var model = GetResourceDetails(resourceItem);
			model.MoreResources = GetMoreResources(resourceItem.Id, model.ResourceDetails.Category.Id);
			model.Comments = GetResourceComments(resourceItem);

			return model;
		}
		#endregion GetResourceDetails by Name

		#endregion GetResourceDetails

		#region GetFeaturedResources

		public IAFCHandBookResourceModel GetFeaturedResourcesData()
		{
            return GetFeaturedResourcesDataNext();
		}

		#endregion GetFeaturedResources

		#region GetRecentlyAddedResources

		public List<IAFCHandBookResourceModel> GetRecentlyAddedResources()
		{
            return GetRecentlyAddedResourcesNext();
		}

		#endregion GetRecentlyAddedResources

		#region GetResourcesPerCategory

		public IAFCHandBookResourcesPerCatergoryModel GetResourcesPerCategory(string categoryName, string orderBy)
		{
            return GetResourcesPerCategoryNext(categoryName, orderBy);
		}

		#endregion GetResourcesPerCategory		

		#region GetMoreResources
		public List<IAFCHandBookMoreResourcesModel> GetMoreResources(Guid resourceId, Guid categoryID)
		{
			List<IAFCHandBookMoreResourcesModel> moreResources = new List<IAFCHandBookMoreResourcesModel>();


			DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();
			var moreResourcesArray = dynamicModuleManager.GetDataItems(handBookResourcesType).
							Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live && d.Id != resourceId).
							ToArray();

			var moreResourcesItems = moreResourcesArray.Where(i => (((i.GetValue<DynamicContent>("ExternalResources") != null) &&
										(i.GetValue<DynamicContent>("ExternalResources").GetValue<IList<Guid>>("Category").Contains(categoryID)))
										|| ((i.GetValue<DynamicContent>("Resources") != null) &&
										(i.GetValue<DynamicContent>("Resources").GetValue<IList<Guid>>("Category").Contains(categoryID))))).
							OrderByDescending(r => r.DateCreated).Take(5);

			foreach (var item in moreResourcesItems)
			{
				var moreResourcesItem = new IAFCHandBookMoreResourcesModel();
				moreResourcesItem.Id = item.Id;
				moreResourcesItem.ResourceDetails = GetResourceDetailsInfo(item);
				moreResourcesItem.Likes = GetResourceLikesInfo(item);
				moreResourcesItem.ResourceUrl = moreResourcesItem.ResourceDetails.Category.CategoryUrl + "/resourcedetails/" + item.UrlName.ToString();
				moreResources.Add(moreResourcesItem);
			}

			return moreResources;
		}
		#endregion GetMoreResources

		#region Categories Methods

		#region GetMoreCategories
		private List<IAFCHandBookTopicCategoryModel> GetMoreCategories(Guid categoryId)
		{
			TaxonomyManager taxonomyManager = TaxonomyManager.GetManager();
			var moreTopicCategories = new List<IAFCHandBookTopicCategoryModel>();
			var moreTopicCategoriesID = new List<Guid>();
			var currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Id == categoryId).First();

			var topicCategory = GetTopicCategories(categoryId);
			var parentCategoryTitle = topicCategory.ResourceParentCategoryTitle;

			switch (parentCategoryTitle)
			{
				case LeadershipCategoryTitle:
					moreTopicCategoriesID = topicLeadershipCategories.Where(c => c != categoryId).ToList();
					break;
				case CommunityCategoryTitle:
					moreTopicCategoriesID = topicCommunityRelationsCategories.Where(c => c != categoryId).ToList();
					break;
				case FinanceCategoryTitle:
					moreTopicCategoriesID = topicFinanceCategories.Where(c => c != categoryId).ToList();
					break;
				case PersonnelCategoryTitle:
					moreTopicCategoriesID = topicPersonnelCategories.Where(c => c != categoryId).ToList();
					break;
			}

			foreach (var id in moreTopicCategoriesID)
			{
				var categoryResourceAmount = GetResourcesAmountPerCategory(id);
				var newTopicCategory = new IAFCHandBookTopicCategoryModel();
				

				var topicCategoryDetails = GetTopicCategories(id);

				newTopicCategory.Id = id;
				newTopicCategory.CategoryDescription = topicCategoryDetails.ResourceCategoryDescription;
				newTopicCategory.CategoryTitle = topicCategoryDetails.ResourceCategoryTile;
				newTopicCategory.CategoryUrl = topicCategoryDetails.ResourceCategoryUrl;
				newTopicCategory.TopicCategoryImageUrl = topicCategoryDetails.ResourceParentCategoryImageUrl;
				newTopicCategory.ResourcesAmount = categoryResourceAmount;
				newTopicCategory.MyHandBookCategoryUrl = topicCategoryDetails.MyHandbookResourceCategoryUrl;
				newTopicCategory.MyHandBookParentCategoryUrl = topicCategoryDetails.MyHandbookResourceParentCategoryUrl;
				moreTopicCategories.Add(newTopicCategory);
			}


			return moreTopicCategories;
		}
		#endregion GetMoreCategories

		#region GetResourcesAmountPerCategory

		private int GetResourcesAmountPerCategory(Guid categoryId)
		{
            return GetResourcesAmountPerCategoryNext(categoryId);
		}

		#endregion GetResourcesAmountPerCategory

		#endregion Categories Methods

		#region Likes
		#region Add Like

		public int AddLikeForResource(Guid resourceID, string resourceType)
		{
			int currentLikes = 0;

			try
			{
				DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();

                string commentFieldText = "Likes";
				var resourceTypeItem = handBookResourcesType;

				if (resourceType == commentResource)
				{
					resourceTypeItem = resourceCommentsType;
					commentFieldText = "CommentLikes";
				}

				var resource = dynamicModuleManager.GetDataItem(resourceTypeItem, resourceID);
				var resourceLike = resource.GetRelatedItems(commentFieldText).Cast<DynamicContent>().First();
				var masterResourceLike = dynamicModuleManager.Lifecycle.GetMaster(resourceLike);

				currentLikes = Convert.ToInt32(resourceLike.GetValue("AmountOfLikes")) + 1;

				DynamicContent checkOutLikeItem = dynamicModuleManager.Lifecycle.CheckOut(masterResourceLike) as DynamicContent;
				checkOutLikeItem.SetValue("AmountOfLikes", currentLikes);
				ILifecycleDataItem checkInLikeItem = dynamicModuleManager.Lifecycle.CheckIn(checkOutLikeItem);
				dynamicModuleManager.Lifecycle.Publish(checkInLikeItem);

                if (resourceTypeItem == handBookResourcesType)
                {
                    var resourceMaster = dynamicModuleManager.Lifecycle.GetMaster(resource);
                    var resourceTemp = dynamicModuleManager.Lifecycle.CheckOut(resourceMaster) as DynamicContent;

                    resourceTemp.SetValue("AmountOfLikes", currentLikes);

                    resourceMaster = dynamicModuleManager.Lifecycle.CheckIn(resourceTemp);
                    dynamicModuleManager.Lifecycle.Publish(resourceMaster);
                }

				dynamicModuleManager.SaveChanges();
			}
			catch (Exception e)
			{
				log.Error("add like error " + e.Message);
			}

			return currentLikes;
		}

		#endregion Add Like

		#region Add Dislike

		public int AddDislikeForResource(Guid resourceID, string resourceType)
		{
			int currentDislikes = 0;

			try
			{
				DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();

				string commentFieldText = "Likes";
				var resourceTypeItem = handBookResourcesType;

				if (resourceType == commentResource)
				{
					resourceTypeItem = resourceCommentsType;
					commentFieldText = "CommentLikes";
				}

				//Add DisLike to resource
				var resource = dynamicModuleManager.GetDataItem(resourceTypeItem, resourceID);
				var resourceDislike = resource.GetRelatedItems(commentFieldText).Cast<DynamicContent>().First();
				var masterResourceDislike = dynamicModuleManager.Lifecycle.GetMaster(resourceDislike);

				currentDislikes = Convert.ToInt32(resourceDislike.GetValue("AmountOfDislikes")) + 1;

				DynamicContent checkOutDislikeItem = dynamicModuleManager.Lifecycle.CheckOut(masterResourceDislike) as DynamicContent;
				checkOutDislikeItem.SetValue("AmountOfDislikes", currentDislikes);
				ILifecycleDataItem checkInDislikeItem = dynamicModuleManager.Lifecycle.CheckIn(checkOutDislikeItem);
				dynamicModuleManager.Lifecycle.Publish(checkInDislikeItem);

                if (resourceTypeItem == handBookResourcesType)
                {
                    var resourceMaster = dynamicModuleManager.Lifecycle.GetMaster(resource);
                    var resourceTemp = dynamicModuleManager.Lifecycle.CheckOut(resourceMaster) as DynamicContent;

                    resourceTemp.SetValue("AmountOfDislikes", currentDislikes);

                    resourceMaster = dynamicModuleManager.Lifecycle.CheckIn(resourceTemp);
                    dynamicModuleManager.Lifecycle.Publish(resourceMaster);
                }

                dynamicModuleManager.SaveChanges();
			}
			catch (Exception e)
			{
				log.Error("add dislike error " + e.Message);
			}

			return currentDislikes;
		}

		#endregion Add Dislike
		#endregion Likes

		#region Comments
		public void CreateNewCommentForResource(Guid resourceID, string comment, string resourceType = resourceResource)
		{
			var providerName = String.Empty;
			var transactionName = "commentTransaction";

			string commentType = "Comment";
			var resourceTypeItem = handBookResourcesType;

			if (resourceType == commentResource)
			{
                commentType = "Reply";
                resourceTypeItem = resourceCommentsType;
			}

			DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager(providerName, transactionName);

			var resource = dynamicModuleManager.GetDataItem(resourceTypeItem, resourceID);
			var commentsAmount = resource.GetRelatedItems(commentType).Cast<DynamicContent>().Count() + 1;
			var commentTtitle = resource.GetValue("Title").ToString() + "_comment_" + commentsAmount.ToString();
			var likeTtitle = resource.GetValue("Title").ToString() + "_comment_" + commentsAmount.ToString() + "_like";

			//Create like for comment			
			DynamicContent mylikesmoduleItem = dynamicModuleManager.CreateDataItem(resourceLikesType);

			mylikesmoduleItem.SetValue("Title", likeTtitle);
			mylikesmoduleItem.SetValue("AmountOfLikes", 0);
			mylikesmoduleItem.SetValue("AmountOfDislikes", 0);
			mylikesmoduleItem.SetString("UrlName", new Lstring(Regex.Replace(likeTtitle, UrlNameCharsToReplace, UrlNameReplaceString)));
			mylikesmoduleItem.SetValue("Owner", SecurityManager.GetCurrentUserId());
			mylikesmoduleItem.SetValue("PublicationDate", DateTime.UtcNow);

			dynamicModuleManager.Lifecycle.Publish(mylikesmoduleItem);
			mylikesmoduleItem.SetWorkflowStatus(dynamicModuleManager.Provider.ApplicationName, "Published");

			//Create new Comment
			DynamicContent mycommentsItem = dynamicModuleManager.CreateDataItem(resourceCommentsType);
			mycommentsItem.SetValue("Title", commentTtitle);
			mycommentsItem.SetValue("CommentText", comment);
			mycommentsItem.SetString("UrlName", new Lstring(Regex.Replace(commentTtitle, UrlNameCharsToReplace, UrlNameReplaceString)));
			mycommentsItem.SetValue("Owner", SecurityManager.GetCurrentUserId());
			mycommentsItem.SetValue("PublicationDate", DateTime.UtcNow);

			mycommentsItem.CreateRelation(mylikesmoduleItem, "CommentLikes");

			ILifecycleDataItem publishedMycommentsItem = dynamicModuleManager.Lifecycle.Publish(mycommentsItem);
			mycommentsItem.SetWorkflowStatus(dynamicModuleManager.Provider.ApplicationName, "Published");

			//Add Comment to resource
			var masterResource = dynamicModuleManager.Lifecycle.GetMaster(resource);

			DynamicContent checkOutResourceItem = dynamicModuleManager.Lifecycle.CheckOut(masterResource) as DynamicContent;
			checkOutResourceItem.CreateRelation(mycommentsItem, commentType);

            if (resourceTypeItem == handBookResourcesType)
            {
                checkOutResourceItem.SetValue("AmountOfComments", commentsAmount);
            }

            ILifecycleDataItem checkInMyresourcesItem = dynamicModuleManager.Lifecycle.CheckIn(checkOutResourceItem);
			dynamicModuleManager.Lifecycle.Publish(checkInMyresourcesItem);

			TransactionManager.CommitTransaction(transactionName);
		}

		#endregion Comments

		#region MyHandBook

		#region GetCreateMyHandBook
		public DynamicContent GetOrCreateMyHandBook()
		{
			DynamicContent myHandBook = new DynamicContent();
			var providerName = String.Empty;
			var transactionName = "myHandBookTransaction";

			var model = new IAFCHandBookMyHandBookModel();
			var identity = ClaimsManager.GetCurrentIdentity();

			var currentUserGuid = identity.UserId;
			log.Info("GetOrCreateMyHandBook:" + currentUserGuid.ToString());
			try
			{
				if (currentUserGuid != Guid.Empty)
				{
					DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager(providerName, transactionName);

					var myHandBookModuleArray = dynamicModuleManager.GetDataItems(myHandBookType).
								Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live).ToArray();
					var myHandBookModuleData = myHandBookModuleArray.Where(u => u.GetValue<Guid>("UserId") == currentUserGuid);
					if (myHandBookModuleData.Any())
					{
						myHandBook = myHandBookModuleData.First();
					}
					else
					{
						DynamicContent myHandBookItem = dynamicModuleManager.CreateDataItem(myHandBookType);

						UserProfileManager profileManager = UserProfileManager.GetManager();
						UserManager userManager = UserManager.GetManager();
						User user = userManager.GetUser(currentUserGuid);
						var profile = profileManager.GetUserProfile<SitefinityProfile>(user);
						var userFullName = String.Empty;
						if (profile != null)
						{
							userFullName = profile.Nickname;
						}
						else
						{
							userFullName = user.FirstName + " " + user.LastName;
						}

						var myHandBookTitle = "MyHandBook_" + userFullName;

						myHandBookItem.SetValue("Title", myHandBookTitle);
						myHandBookItem.SetValue("UserId", currentUserGuid);
						myHandBookItem.SetString("UrlName", new Lstring(Regex.Replace(myHandBookTitle, UrlNameCharsToReplace, UrlNameReplaceString)));
						myHandBookItem.SetValue("Owner", SecurityManager.GetCurrentUserId());
						myHandBookItem.SetValue("PublicationDate", DateTime.UtcNow);

						dynamicModuleManager.Lifecycle.Publish(myHandBookItem);
						myHandBookItem.SetWorkflowStatus(dynamicModuleManager.Provider.ApplicationName, "Published");
						TransactionManager.CommitTransaction(transactionName);

						myHandBook = myHandBookItem;
					}
				}
			}
			catch (Exception e)
			{
				log.Error("MyHandbook" + e.Message);
			}

			return myHandBook;
		}
		#endregion GetCreateMyHandBook

		#region GetMyHandBookByID
		public DynamicContent GetMyHandBookByID(Guid userId)
		{
			DynamicContent myHandBook = new DynamicContent();
			var providerName = String.Empty;
			var transactionName = "myHandBookTransaction";

			var model = new IAFCHandBookMyHandBookModel();
			try
			{
				DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager(providerName, transactionName);

				var myHandBookModuleArray = dynamicModuleManager.GetDataItems(myHandBookType).
							Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live).ToArray();
				var myHandBookModuleData = myHandBookModuleArray.Where(u => u.GetValue<Guid>("UserId") == userId);
				if (myHandBookModuleData.Any())
				{
					myHandBook = myHandBookModuleData.First();
				}
			}
			catch (Exception e)
			{
				log.Error("MyHandbook" + e.Message);
			}

			return myHandBook;
		}
		#endregion GetMyHandBookByID

		#region GetMyHandBook
		public IAFCHandBookMyHandBookModel GetMyHandBook(String userId = null)
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
					model.SharedUserId = userGuid;
				}
				else
				{ 
					myHandBookItem = GetOrCreateMyHandBook();
				}
				log.Info("GetMyHandBook:" + myHandBookItem.Id.ToString());
				model.Id = myHandBookItem.Id;
				model.UserId = SecurityManager.GetCurrentUserId();
				log.Info("GetMyHandBook:" + model.UserId);

				var myHandBookResources = myHandBookItem.GetRelatedItems("MyResources").Cast<DynamicContent>().ToList();
				var myCompletedHandBookResources = myHandBookItem.GetRelatedItems("MyCompletedResources").Cast<DynamicContent>().ToList();

				var myHandBookResourcesItem = new IAFCHandBookMyHandBookResourceModelModel();
				myHandBookResourcesItem.IncompletedResourcesAmount = myHandBookResources.Count();
				myHandBookResourcesItem.CompletedResourcesAmount = myCompletedHandBookResources.Count();
				
				var hbResTotalDuration = GetTotalDuration(myHandBookResources);
				var hbComplResTotalDuration = GetTotalDuration(myCompletedHandBookResources);
				var totalDuration = new TimeSpan();
				totalDuration = totalDuration.Add(hbResTotalDuration).Add(hbComplResTotalDuration);
				myHandBookResourcesItem.TotalDuration = myHandBookResourcesItem.TotalDuration.Add(totalDuration);

				foreach (var categoryItem in topicParentCategories)
				{
					var myChildHandBookResourcesItem = new IAFCHandBookMyHandBookResourceModelModel();
					var category = new IAFCHandBookTopicCategoryModel();
					var categoryDetails = GetTopicCategories(categoryItem);
					category.Id = categoryItem;
					category.MyHandBookCategoryUrl = categoryDetails.MyHandbookResourceCategoryUrl+ sharedUrl;
					category.MyHandBookParentCategoryUrl = categoryDetails.MyHandbookResourceParentCategoryUrl+ sharedUrl;
					category.TopicCategoryImageUrl = categoryDetails.ResourceParentCategoryImageUrl;
					category.CategoryTitle = categoryDetails.ResourceCategoryTile;
					category.CategoryDescription = categoryDetails.ResourceCategoryDescription;

					var childCategoriesList = GetChildCategories(categoryItem);
					var categoryCompletedResources = myCompletedHandBookResources.
						Where(i => (((i.GetValue<DynamicContent>("ExternalResources") != null) &&
									(i.GetValue<DynamicContent>("ExternalResources").GetValue<IList<Guid>>("Category").Where(c => childCategoriesList.Contains(c)).Any()))
									|| ((i.GetValue<DynamicContent>("Resources") != null) &&
									(i.GetValue<DynamicContent>("Resources").GetValue<IList<Guid>>("Category").Where(c => childCategoriesList.Contains(c)).Any())))).ToList();

					var categoryResources = myHandBookResources.
						Where(i => (((i.GetValue<DynamicContent>("ExternalResources") != null) &&
									(i.GetValue<DynamicContent>("ExternalResources").GetValue<IList<Guid>>("Category").Where(c => childCategoriesList.Contains(c)).Any()))
									|| ((i.GetValue<DynamicContent>("Resources") != null) &&
									(i.GetValue<DynamicContent>("Resources").GetValue<IList<Guid>>("Category").Where(c => childCategoriesList.Contains(c)).Any())))).ToList()
									;
					var hbCategoryResTotalDuration = GetTotalDuration(categoryResources);
					var hbCategoryComplResTotalDuration = GetTotalDuration(categoryCompletedResources);
					var categoryTotalDuration = new TimeSpan();
					categoryTotalDuration = categoryTotalDuration.Add(hbCategoryResTotalDuration).Add(hbCategoryComplResTotalDuration);
					category.TotalDuration = category.TotalDuration.Add(categoryTotalDuration);
					
					foreach (var childItem in childCategoriesList)
					{
						var childCategory = new IAFCHandBookTopicCategoryModel();
						var childCategoryDetails = GetTopicCategories(childItem);

						childCategory.Id = childItem;
						childCategory.MyHandBookCategoryUrl = childCategoryDetails.MyHandbookResourceCategoryUrl+ sharedUrl;
						childCategory.MyHandBookParentCategoryUrl = childCategoryDetails.MyHandbookResourceParentCategoryUrl+ sharedUrl;
						childCategory.TopicCategoryImageUrl = childCategoryDetails.ResourceParentCategoryImageUrl;
						childCategory.CategoryTitle = childCategoryDetails.ResourceCategoryTile;
						childCategory.CategoryDescription = childCategoryDetails.ResourceCategoryDescription;
						category.ChildCategories.Add(childCategory);
					}

					category.MyHandBookCompletedResources = myCompletedHandBookResources.
						Where(i => (((i.GetValue<DynamicContent>("ExternalResources") != null) &&
									(i.GetValue<DynamicContent>("ExternalResources").GetValue<IList<Guid>>("Category").Where(c => childCategoriesList.Contains(c)).Any()))
									|| ((i.GetValue<DynamicContent>("Resources") != null) &&
									(i.GetValue<DynamicContent>("Resources").GetValue<IList<Guid>>("Category").Where(c => childCategoriesList.Contains(c)).Any())))).
						Count();

					category.MyHandBookInCompletedResources = myHandBookResources.
						Where(i => (((i.GetValue<DynamicContent>("ExternalResources") != null) &&
									(i.GetValue<DynamicContent>("ExternalResources").GetValue<IList<Guid>>("Category").Where(c => childCategoriesList.Contains(c)).Any()))
									|| ((i.GetValue<DynamicContent>("Resources") != null) &&
									(i.GetValue<DynamicContent>("Resources").GetValue<IList<Guid>>("Category").Where(c => childCategoriesList.Contains(c)).Any())))).
						Count();

					myChildHandBookResourcesItem.Category = category;
					myHandBookResourcesItem.MyChildHandBookResources.Add(myChildHandBookResourcesItem);
				}
				model.MyHandBookResurces.Add(myHandBookResourcesItem);
				log.Info("GetMyHandBook: End");
			}
			catch (Exception e)
			{
				log.Error("GetMyHandBook Error: " + e.Message);
			}
			
			return model;
		}
		#endregion GetMyHandBook

		#region GetTotalDuration
		public TimeSpan GetTotalDuration(List<DynamicContent> resources)
		{
			var totalDuration = new TimeSpan();
			foreach (var res in resources)
			{
				var externalResourceModuleData = res.GetRelatedItems("ExternalResources").Cast<DynamicContent>();
				if (externalResourceModuleData.Any())
				{
					var externalResourceItem = externalResourceModuleData.First();
					TimeSpan duration = new TimeSpan(0, 0, 0);
					try
					{
						var durationList = externalResourceItem.GetValue("Time").ToString().Split(',');
						switch (durationList.Count())
						{
							case 1:
								duration = new TimeSpan(0, 0, int.Parse(durationList[0]));
								break;
							case 2:
								duration = new TimeSpan(0, int.Parse(durationList[0]), int.Parse(durationList[1]));

								break;
							case 3:
								duration = new TimeSpan(int.Parse(durationList[0]), int.Parse(durationList[1]), int.Parse(durationList[2]));

								break;
						}
					}
					catch (Exception e)
					{
						log.Error("GetTotalDuration cant parse duration time = '" + externalResourceItem.GetValue("Time").ToString() + "'");
					}

					totalDuration = totalDuration.Add(duration);
				}
			}
			return totalDuration;
		}
		#endregion GetTotalDuration

		#region AddToMyHandBook
		public Boolean AddToMyHandBook(Guid resourceId)
		{
			Boolean returnData = false;
			try
			{
				var myHandBookItem = GetOrCreateMyHandBook();
				
				DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();
				var resource = dynamicModuleManager.GetDataItems(handBookResourcesType).
							Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live && d.Id == resourceId).First();

				var masterResource = dynamicModuleManager.Lifecycle.GetMaster(resource);
				var masterHandBook = dynamicModuleManager.Lifecycle.GetMaster(myHandBookItem);

				masterHandBook.CreateRelation(masterResource, "MyResources");

				dynamicModuleManager.Lifecycle.Publish(masterHandBook);
				dynamicModuleManager.SaveChanges();

				returnData = true;
			}
			catch (Exception e)
			{
				log.Error("AddToMyHandBook Error: " + e.Message);
			}
			return returnData;
		}

		#endregion AddToMyHandBook

		#region MarkAsComplete
		public Boolean MarkAsComplete(Guid resourceId)
		{
			Boolean returnData = false;
			try
			{
				var myHandBookItem = GetOrCreateMyHandBook();								
				DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();

				var resource = dynamicModuleManager.GetDataItems(handBookResourcesType).
							Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live && d.Id == resourceId).First();

				var masterResource = dynamicModuleManager.Lifecycle.GetMaster(resource);
				var masterHandBook = dynamicModuleManager.Lifecycle.GetMaster(myHandBookItem);

				masterHandBook.CreateRelation(masterResource, "MyCompletedResources");
				masterHandBook.DeleteRelation(masterResource, "MyResources");

				dynamicModuleManager.Lifecycle.Publish(masterHandBook);
				dynamicModuleManager.SaveChanges();
				
				returnData = true;
			}
			catch (Exception e)
			{
				log.Error("MarkAsComplete Error: " + e.Message);
			}
			return returnData;
		}
		#endregion MarkAsComplete

		#region RemoveResource
		public Boolean RemoveResource(Guid resourceId, String fieldName= "MyResources")
		{
			Boolean returnData = false;
			try
			{				
				var myHandBookItem = GetOrCreateMyHandBook();
				DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();

				var resource = dynamicModuleManager.GetDataItems(handBookResourcesType).
							Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live && d.Id == resourceId).First();

				var masterResource = dynamicModuleManager.Lifecycle.GetMaster(resource);
				var masterHandBook = dynamicModuleManager.Lifecycle.GetMaster(myHandBookItem);

				
				masterHandBook.DeleteRelation(masterResource, fieldName);

				dynamicModuleManager.Lifecycle.Publish(masterHandBook);
				dynamicModuleManager.SaveChanges();

				returnData = true;
			}
			catch (Exception e)
			{
				log.Error("RemoveResource Error: " + e.Message);
			}
			return returnData;
		}
		#endregion RemoveResource

		#region IsResourceAddedToMyHandBook
		public Boolean IsResourceAddedToMyHandBook(Guid resourceId)
		{

			Boolean returnData = false;
			var i = 0;
			try
			{
				var myHandBookItem = GetOrCreateMyHandBook();

				var providerName = String.Empty;
				var transactionName = "myHandBookTransaction";

				DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager(providerName, transactionName);
				
				var myHandBookResources = myHandBookItem.GetRelatedItems("MyResources").Cast<DynamicContent>().ToArray();

				var handBookResourceAdded = myHandBookResources.Where(r => r.Id == resourceId).Any();
				returnData = handBookResourceAdded;
			}
			catch (Exception e)
			{
				log.Error("IsResourceAddedToMyHandBook Error: " + e.Message);
			}
			return returnData;
		}
		#endregion IsResourceAddedToMyHandBook

		#region IsResourceMarkedAsComplete
		public Boolean IsResourceMarkedAsComplete(Guid resourceId)
		{

			Boolean returnData = false;
			var i = 0;
			try
			{
				var myHandBookItem = GetOrCreateMyHandBook();

				var providerName = String.Empty;
				var transactionName = "myHandBookTransaction";

				DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager(providerName, transactionName);

				var myHandBookResources = myHandBookItem.GetRelatedItems("MyCompletedResources").Cast<DynamicContent>().ToArray();

				var handBookResourceAdded = myHandBookResources.Where(r => r.Id == resourceId).Any();
				returnData = handBookResourceAdded;
			}
			catch (Exception e)
			{
				log.Error("IsResourceMarkedAsComplete Error: " + e.Message);
			}
			return returnData;
		}
		#endregion IsResourceMarkedAsComplete

		#region MyHandBookGetResourcesPerCategory
		public IAFCHandBookMyHandBookModel GetMyHandBookResourcesPerCategory(String categoryName, String userId = null)
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
					showAsMyHandBookItem = false;
					userGuid = Guid.Parse(userId);
					model.SharedUserId = userGuid;
					myHandBookItem = GetMyHandBookByID(userGuid);					
				}

				model.Id = myHandBookItem.Id;
				model.UserId = SecurityManager.GetCurrentUserId();


				var myHandBookResourcesItem = new IAFCHandBookMyHandBookResourceModelModel();

				var categoryItem = GetCategoryGuidByName(categoryName);

				var category = new IAFCHandBookTopicCategoryModel();
				var categoryDetails = GetTopicCategories(categoryItem);
				category.Id = categoryItem;
				category.MyHandBookCategoryUrl = categoryDetails.MyHandbookResourceCategoryUrl+sharedUrl;
				category.CategoryTitle = categoryDetails.ResourceCategoryTile;
				myHandBookResourcesItem.Category = category;
				myHandBookResourcesItem.SharedUserId = userGuid;

				var myHandBookResources = myHandBookItem.GetRelatedItems("MyResources").Cast<DynamicContent>().ToList();
				var myCompletedHandBookResources = myHandBookItem.GetRelatedItems("MyCompletedResources").Cast<DynamicContent>().ToList();

				var childCategoriesList = GetChildCategories(categoryItem);

				foreach (var childItem in childCategoriesList)
				{
					var myChildHandBookResourcesItem = new IAFCHandBookMyHandBookResourceModelModel();

					var childCategory = new IAFCHandBookTopicCategoryModel();
					var childCategoryDetails = GetTopicCategories(childItem);

					childCategory.Id = childItem;
					childCategory.MyHandBookCategoryUrl = childCategoryDetails.MyHandbookResourceCategoryUrl+sharedUrl;
					childCategory.MyHandBookParentCategoryUrl = childCategoryDetails.MyHandbookResourceParentCategoryUrl+sharedUrl;
					childCategory.TopicCategoryImageUrl = childCategoryDetails.ResourceParentCategoryImageUrl;
					childCategory.CategoryTitle = childCategoryDetails.ResourceCategoryTile;
					childCategory.CategoryDescription = childCategoryDetails.ResourceCategoryDescription;


					var categoryCompletedResources = myCompletedHandBookResources.
						Where(i => (((i.GetValue<DynamicContent>("ExternalResources") != null) &&
									(i.GetValue<DynamicContent>("ExternalResources").GetValue<IList<Guid>>("Category").Contains(childItem)))
									|| ((i.GetValue<DynamicContent>("Resources") != null) &&
									(i.GetValue<DynamicContent>("Resources").GetValue<IList<Guid>>("Category").Contains(childItem))))).ToList();

					var categoryResources = myHandBookResources.
						Where(i => (((i.GetValue<DynamicContent>("ExternalResources") != null) &&
									(i.GetValue<DynamicContent>("ExternalResources").GetValue<IList<Guid>>("Category").Contains(childItem)))
									|| ((i.GetValue<DynamicContent>("Resources") != null) &&
									(i.GetValue<DynamicContent>("Resources").GetValue<IList<Guid>>("Category").Contains(childItem))))).ToList()
									;
					var hbCategoryResTotalDuration = GetTotalDuration(categoryResources);
					var hbCategoryComplResTotalDuration = GetTotalDuration(categoryCompletedResources);
					var categoryTotalDuration = new TimeSpan();
					categoryTotalDuration = categoryTotalDuration.Add(hbCategoryResTotalDuration).Add(hbCategoryComplResTotalDuration);
					childCategory.TotalDuration = childCategory.TotalDuration.Add(categoryTotalDuration);
					childCategory.MyHandBookCompletedResources = categoryCompletedResources.Count();
					var myHandBookResourcesAmount = categoryResources.Count();
					childCategory.MyHandBookInCompletedResources = myHandBookResourcesAmount;

					myChildHandBookResourcesItem.Category = childCategory;
					myChildHandBookResourcesItem.SharedUserId = userGuid;

					var myChildResourceItem = new IAFCHandBookResourceModel();
					foreach (var resourceItem in categoryResources.OrderByDescending(r => r.DateCreated).Take(5))
					{						
						myChildResourceItem = GetResourceDetails(resourceItem, showAsMyHandBookItem);						
						myChildResourceItem.MoreThen5Resources = (myHandBookResourcesAmount - 5) < 0 ? 0 : myHandBookResourcesAmount - 5;
						
						myChildHandBookResourcesItem.MyResources.Add(myChildResourceItem);
					}

					var myChildCompletedResourceItem = new IAFCHandBookResourceModel();
					foreach (var resourceCompletedItem in categoryCompletedResources.OrderByDescending(r => r.DateCreated).Take(5))
					{
						myChildCompletedResourceItem = GetResourceDetails(resourceCompletedItem, showAsMyHandBookItem);
						myChildHandBookResourcesItem.MyCompletedResources.Add(myChildCompletedResourceItem);
						
					}
					
					
					myHandBookResourcesItem.MyChildHandBookResources.Add(myChildHandBookResourcesItem);
				}
				
				model.MyHandBookResurces.Add(myHandBookResourcesItem);

			}
			catch (Exception e)
			{
				log.Error("GetMyHandBookResourcesPerCategory Error: " + e.Message);
			}
			return model;
		}
		#endregion MyHandBookGetResourcesPerCategory

		#region GetCategoryResources
		public IAFCHandBookMyHandBookResourceModelModel GetCategoryResources(Guid categoryId, Boolean showAllResources, String userId= null, String orderBy= OrderByMostRecent)
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
					showAsMyHandBookItem = false;
					var userGuid = Guid.Parse(userId);
					model.SharedUserId = userGuid;
					
					myHandBookItem = GetMyHandBookByID(userGuid);
				}

				var myHandBookResources = myHandBookItem.GetRelatedItems("MyResources").Cast<DynamicContent>().ToList();
				var myCompletedHandBookResources = myHandBookItem.GetRelatedItems("MyCompletedResources").Cast<DynamicContent>().ToList();
				
				var category = new IAFCHandBookTopicCategoryModel();
				var categoryDetails = GetTopicCategories(categoryId);

				category.Id = categoryId;
				category.MyHandBookCategoryUrl = categoryDetails.MyHandbookResourceCategoryUrl+sharedUrl;
				category.MyHandBookParentCategoryUrl = categoryDetails.MyHandbookResourceParentCategoryUrl+sharedUrl;
				category.TopicCategoryImageUrl = categoryDetails.ResourceParentCategoryImageUrl;
				category.CategoryTitle = categoryDetails.ResourceCategoryTile;
				category.CategoryDescription = categoryDetails.ResourceCategoryDescription;
				category.ParentCategoryTitle = categoryDetails.ResourceParentCategoryTitle;
				
				var categoryResourcesList = myHandBookResources.
					Where(i => (((i.GetValue<DynamicContent>("ExternalResources") != null) &&
								(i.GetValue<DynamicContent>("ExternalResources").GetValue<IList<Guid>>("Category").Contains(categoryId)))
								|| ((i.GetValue<DynamicContent>("Resources") != null) &&
								(i.GetValue<DynamicContent>("Resources").GetValue<IList<Guid>>("Category").Contains(categoryId))))).ToList();

				var categoryCompletedResourcesList = myCompletedHandBookResources.
					Where(i => (((i.GetValue<DynamicContent>("ExternalResources") != null) &&
								(i.GetValue<DynamicContent>("ExternalResources").GetValue<IList<Guid>>("Category").Contains(categoryId)))
								|| ((i.GetValue<DynamicContent>("Resources") != null) &&
								(i.GetValue<DynamicContent>("Resources").GetValue<IList<Guid>>("Category").Contains(categoryId))))).ToList();


				var categoryResources = new List<DynamicContent>();
				var categoryCompletedResources = new List<DynamicContent>();
				if (orderBy == OrderByMostRecent)
				{
					categoryResources = categoryResourcesList.OrderByDescending(r => r.DateCreated).ToList();
					categoryCompletedResources = categoryCompletedResourcesList.OrderByDescending(r => r.DateCreated).ToList();
				}
				else if (orderBy == OrderByMostPopular)
				{
					categoryResources = categoryResourcesList.OrderByDescending(r => int.Parse(r.GetValue<DynamicContent>("Likes").GetValue("AmountOfLikes").ToString())).
							ToList();
					categoryCompletedResources = categoryCompletedResourcesList.OrderByDescending(r => int.Parse(r.GetValue<DynamicContent>("Likes").GetValue("AmountOfLikes").ToString())).
							ToList();
				}
				else if (orderBy == OrderByAlphabeticalAZ)
				{
					categoryResources = categoryResourcesList.OrderBy(r => r.GetValue("Title").ToString()).ToList();
					categoryCompletedResources = categoryCompletedResourcesList.OrderBy(r => r.GetValue("Title").ToString()).ToList();
				}
				else if (orderBy == OrderByAlphabeticalZA)
				{
					categoryResources = categoryResourcesList.OrderByDescending(r => r.GetValue("Title").ToString()).ToList();
					categoryCompletedResources = categoryCompletedResourcesList.OrderByDescending(r => r.GetValue("Title").ToString()).ToList();
				}

				var hbCategoryResTotalDuration = GetTotalDuration(categoryResources);
				var hbCategoryComplResTotalDuration = GetTotalDuration(categoryCompletedResources);
				var categoryTotalDuration = new TimeSpan();
				categoryTotalDuration = categoryTotalDuration.Add(hbCategoryResTotalDuration).Add(hbCategoryComplResTotalDuration);
				category.TotalDuration = category.TotalDuration.Add(categoryTotalDuration);
				category.MyHandBookCompletedResources = categoryCompletedResources.Count();
				var myHandBookResourcesAmount = categoryResources.Count();
				category.MyHandBookInCompletedResources = myHandBookResourcesAmount;

				model.Category = category;
				model.MoreCategories = GetMoreCategories(categoryId);

				var myChildResourceItem = new IAFCHandBookResourceModel();
				foreach (var resourceItem in showAllResources? categoryResources: categoryResources.Take(5))
				{
					myChildResourceItem = GetResourceDetails(resourceItem, showAsMyHandBookItem);
					
					myChildResourceItem.MoreThen5Resources = (myHandBookResourcesAmount - 5) < 0 ? 0 : myHandBookResourcesAmount - 5;
					model.MyResources.Add(myChildResourceItem);
				}

				var myChildCompletedResourceItem = new IAFCHandBookResourceModel();
				foreach (var resourceCompletedItem in showAllResources? categoryCompletedResources: categoryCompletedResources.Take(5))
				{
					myChildCompletedResourceItem = GetResourceDetails(resourceCompletedItem, showAsMyHandBookItem);										
					model.MyCompletedResources.Add(myChildCompletedResourceItem);
				}

				var orderByList = InitOrderBy(orderBy);
				model.OrderBy = orderByList;
			}
			catch (Exception e)
			{
				log.Error("GetCategoryResources Error: " + e.Message);
			}
			return model;
		}
		#endregion GetCategoryResources

		#region GetMyHandBookCategoryResources
		public IAFCHandBookMyHandBookResourceModelModel GetMyHandBookCategoryResourcesByName(String  categoryName, String userId = null, String orderBy = OrderByMostRecent)
		{
			var categoryId = GetCategoryGuidByName(categoryName);
			var model = new IAFCHandBookMyHandBookResourceModelModel();
			model = GetCategoryResources(categoryId, false, userId, orderBy);
			return model;
		}
		#endregion GetMyHandBookCategoryResources

		#region GetMyHandBookCategoryResources
		public List<IAFCHandBookResourceModel> GetMyHandBookCategoryResourcesDetails(List<DynamicContent> resourcesList, int resourcesAmount = 0)
		{			
			var model = new List<IAFCHandBookResourceModel>();
			var resourceItemModel = new IAFCHandBookResourceModel();
			
			var resources = resourcesList.OrderByDescending(r => r.DateCreated);
			if (resourcesAmount !=0)
			{
				resources.Take(resourcesAmount);
			}

			foreach (var resourceItem in resources)
			{
				resourceItemModel = GetResourceDetails(resourceItem, true);				
				model.Add(resourceItemModel);
			}

			return model;
		}

		public List<IAFCHandBookResourceModel> GetMyHandBookCategoryResourcesList(Guid categoryId)
		{
			var myHandBookItem = GetOrCreateMyHandBook();

			var myHandBookResources = myHandBookItem.GetRelatedItems("MyResources").Cast<DynamicContent>().ToList();
						
			var categoryResources = myHandBookResources.
				Where(i => (((i.GetValue<DynamicContent>("ExternalResources") != null) &&
							(i.GetValue<DynamicContent>("ExternalResources").GetValue<IList<Guid>>("Category").Contains(categoryId)))
							|| ((i.GetValue<DynamicContent>("Resources") != null) &&
							(i.GetValue<DynamicContent>("Resources").GetValue<IList<Guid>>("Category").Contains(categoryId))))).ToList();

			var model = GetMyHandBookCategoryResourcesDetails(categoryResources);
			
			
			return model;
		}


		#endregion GetMyHandBookCategoryResources

		#region Get My Hnadbook ResourceDetails by Name
		public IAFCHandBookResourceModel GetMyHnadbookResourceDetails(String name)
		{

			DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();
			var resourceItem = dynamicModuleManager.GetDataItems(handBookResourcesType).
						Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live && d.UrlName == name).
						First();

			var model = GetResourceDetails(resourceItem, true);
			model.MoreResources = GetMyHandBookMoreResources(resourceItem.Id, model.ResourceDetails.Category.Id);
			model.Comments = GetResourceComments(resourceItem);

			return model;
		}
		#endregion Get My Hnadbook ResourceDetails by Name

		#region Get My Hand Book MoreResources
		public List<IAFCHandBookMoreResourcesModel> GetMyHandBookMoreResources(Guid resourceId, Guid categoryID)
		{
			List<IAFCHandBookMoreResourcesModel> moreResources = new List<IAFCHandBookMoreResourcesModel>();

			var myHandBookItem = GetOrCreateMyHandBook();
			var myHandBookResources = myHandBookItem.GetRelatedItems("MyResources").Cast<DynamicContent>().
				Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live && d.Id != resourceId).
				ToArray();
			var myCompletedHandBookResources = myHandBookItem.GetRelatedItems("MyCompletedResources").Cast<DynamicContent>().
				Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live && d.Id != resourceId).
				ToArray();
			
			var allMyHandBookResources = myHandBookResources.Union(myCompletedHandBookResources);

			var moreResourcesItems = allMyHandBookResources.Where(i => (((i.GetValue<DynamicContent>("ExternalResources") != null) &&
										(i.GetValue<DynamicContent>("ExternalResources").GetValue<IList<Guid>>("Category").Contains(categoryID)))
										|| ((i.GetValue<DynamicContent>("Resources") != null) &&
										(i.GetValue<DynamicContent>("Resources").GetValue<IList<Guid>>("Category").Contains(categoryID))))).
							OrderByDescending(r => r.DateCreated).Take(5);

			foreach (var item in moreResourcesItems)
			{
				var moreResourcesItem = new IAFCHandBookMoreResourcesModel();
				moreResourcesItem.Id = item.Id;
				moreResourcesItem.ResourceDetails = GetResourceDetailsInfo(item, true);
				moreResourcesItem.Likes = GetResourceLikesInfo(item);
				moreResourcesItem.ResourceUrl = moreResourcesItem.ResourceDetails.Category.MyHandBookCategoryUrl + "/resourcedetails/" + item.UrlName.ToString();
				moreResources.Add(moreResourcesItem);
			}

			return moreResources;
		}
		#endregion Get My Hand Book MoreResources

		#region generateSharedUrl
		public string GenerateSharedUrl(String url)
		{
			var returnUrl=String.Empty;
			returnUrl = url + "/" + SecurityManager.GetCurrentUserId().ToString();
			return returnUrl;
		}

		#endregion generateSharedUrl
		#endregion MyHandBook

	}
}



	