using SitefinityWebApp.Mvc.Models;
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
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Services.Notifications;

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

		private const string OrderByTopic = "ByTopic";
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
		private const string MainPage = "/iafchandbookhome";
		private const string TopicCommynityUrl = MainPage + "/topics/community";
		private const string TopicCommynityCrisisCommunicationUrl = MainPage + "/topics/community/crisis-communication";
		private const string TopicCommynityCustomerServiceUrl = MainPage + "/topics/community/customer-service";
		private const string TopicCommynityMarketingAndMediaUrl = MainPage + "/topics/community/marketing-and-media";
		private const string TopicCommynityPoliticsUrl = MainPage + "/topics/community/politics";
		private const string TopicLeadershipUrl = MainPage + "/topics/leadership";
		private const string TopicLeadershipEthicsUrl = MainPage + "/topics/leadership/ethics";
		private const string TopicLeadershipGenerationalDifferencesUrl = MainPage + "/topics/leadership/generational-differences";
		private const string TopicLeadershipLeadershipStylesUrl = MainPage + "/topics/leadership/leadership-styles";
		private const string TopicLeadershipMotivatingPeopleUrl = MainPage + "/topics/leadership/motivating-people";
		private const string TopicLeadershipStrategyUrl = MainPage + "/topics/leadership/strategy";
		private const string TopicFinanceUrl = MainPage + "/topics/finance";
		private const string TopicFinanceBudgetingUrl = MainPage + "/topics/finance/budgeting";
		private const string TopicFinanceFundraisingUrl = MainPage + "/topics/finance/fundraising";
		private const string TopicFinanceLegalIssuesUrl = MainPage + "/topics/finance/legal-issues";
		private const string TopicPersonnelUrl = MainPage + "/topics/personnel";
		private const string TopicPersonnelInsuranceUrl = MainPage + "/topics/personnel/insurance";
		private const string TopicPersonnelLegalIssuesUrl = MainPage + "/topics/personnel/legal-issues";
		private const string TopicPersonnelRecruitmentUrl = MainPage + "/topics/personnel/recruitment";
		private const string TopicPersonnelRetentionUrl = MainPage + "/topics/personnel/retention";
		private const string TopicPersonnelVolunteerCareerRelationsUrl = MainPage + "/topics/personnel/volunteer-career-relations";

		private const string ResourceDetailsUrl = MainPage + "/iafcresourcedetails/";

		private const string TopicCommynityImageUrl = "/Sitefinity/WebsiteTemplates/IAFCHandBook/App_Themes/IAFCHandBook/images/community-multiply.svg";
		private const string TopicLeadershipImageUrl = "/Sitefinity/WebsiteTemplates/IAFCHandBook/App_Themes/IAFCHandBook/images/leadership-multiply.svg";
		private const string TopicFinanceImageUrl = "/Sitefinity/WebsiteTemplates/IAFCHandBook/App_Themes/IAFCHandBook/images/finance-multiply.svg";
		private const string TopicPersonnelImageUrl = "/Sitefinity/WebsiteTemplates/IAFCHandBook/App_Themes/IAFCHandBook/images/personnel-multiply.svg";

		private const string DefaultPodcastImgUrl = "/images/default-source/icons/podcast.svg";
		private const string DefaultChartImgUrl = "/images/default-source/icons/chart.svg";
		private const string DefaultVideoImgUrl = "images/default-source/icons/video.svg";
		private const string DefaultLinkImgUrl = "images/default-source/icons/link.svg";
		private const string DefaultImageImgUrl = "images/default-source/icons/image.svg";
		private const string DefaultWebinarImgUrl = "images/default-source/icons/webinar.svg";
		private const string DefaultArticleImgUrl = "images/default-source/icons/article.svg";
		private const string DefaultAudioImgUrl = "images/default-source/icons/audio.svg";
		private const string DefaultBookImgUrl = "images/default-source/icons/book.svg";
		private const string DefaultPaceholderImgUrl = "images/default-source/icons/resource-placeholder.svg";
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
		Dictionary<string, string> resourceTypeImages = new Dictionary<string, string>();
		private bool isUserAuthorized = false;
		#endregion Variables

		#region Constructor
		public IAFCHandBookHelper()
		{
			isUserAuthorized = IsUserAuthorized();
			InitResourceTypeImages();
			InitCategoriesGuid();
			InitCategoriesLists();
			InitCategoryDictionary();

		}
		#endregion Constructor

		#region IsUserAuthorized
		private Boolean IsUserAuthorized()
		{
			Boolean returnValue = false;
			var identity = ClaimsManager.GetCurrentIdentity();
			var currentUserGuid = identity.UserId;

			if (currentUserGuid != Guid.Empty)
			{
				returnValue = true;
			}
			return returnValue;
		}
		#endregion IsUserAuthorized

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
		public List<IAFCHandBookTopicOrderBy> InitOrderBy(String orderBy, String url = "", bool addByTopic = false)
		{
			var orderByList = new List<IAFCHandBookTopicOrderBy>();

			var orderByItem = new IAFCHandBookTopicOrderBy();
			if (addByTopic)
			{
				orderByItem.Url = url + "/" + OrderByTopic;
				orderByItem.Title = OrderByTopic;
				if (orderBy == OrderByTopic)
				{
					orderByItem.Selected = true;
				}
				orderByList.Add(orderByItem);
			}

			orderByItem = new IAFCHandBookTopicOrderBy();
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

		public IAFCHandBookResourceDetailsModel GetResourceDetailsInfo(DynamicContent resource, Guid? categoryId, bool isMyHandBookItem = false)
		{
			return GetResourceDetailsInfoNext(resource, categoryId, isMyHandBookItem);
		}

		#endregion GetResourceDetailsInfo

		#region GetResourceDetails

		#region GetResourceDetails by Item

		public IAFCHandBookResourceModel GetResourceDetails(DynamicContent resource, Guid? categoryId, bool isMyHandBookItem = false)
		{
			return GetResourceDetailsNext(resource, categoryId, isMyHandBookItem);
		}

		#endregion GetResourceDetails by Item

		#region GetResourceDetails by Name
		public IAFCHandBookResourceModel GetResourceDetails(String name, Guid? categoryId = null)
		{

			DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();
			var resourceItem = dynamicModuleManager.GetDataItems(handBookResourcesType).
						Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live && d.UrlName == name).
						First();

			var model = GetResourceDetails(resourceItem, categoryId);
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
			return GetMoreResourcesNext(resourceId, categoryID);
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
			try
			{
				if (comment != null && comment.Trim() != String.Empty)
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
			}
			catch (Exception e)
			{
				log.Error("Create Comment Error: " + e.Message);
			}

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


						var userFullName = String.Empty;
						userFullName = GetUserName(currentUserGuid);
						
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
			return GetMyHandBookNext(userId);
		}
		#endregion GetMyHandBook

		#region GetTotalDuration
		public TimeSpan GetTotalDuration(List<DynamicContent> resources)
		{
			return GetTotalDurationNext(resources);
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

		#region AddAllToMyHandBook
		public Boolean AddAllToMyHandBook(Guid categoryId)
		{
			Boolean returnData = false;
			try
			{
				var myHandBookItem = GetOrCreateMyHandBook();

				DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();
				var resourceList = dynamicModuleManager.GetDataItems(handBookResourcesType).
							Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live)
							.Where(r => r.GetValue<IList<Guid>>("Category").Contains(categoryId))
							.ToArray();



				var masterHandBook = dynamicModuleManager.Lifecycle.GetMaster(myHandBookItem);

				foreach (var resourceItem in resourceList)
				{
					if (!IsResourceAddedToMyHandBook(resourceItem.Id) && !IsResourceMarkedAsComplete(resourceItem.Id))
					{
						var masterResourcrItem = dynamicModuleManager.Lifecycle.GetMaster(resourceItem);
						masterHandBook.CreateRelation(masterResourcrItem, "MyResources");
					}
				}

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

		#endregion AddAllToMyHandBook

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
		public Boolean RemoveResource(Guid resourceId, String fieldName = "MyResources")
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
			return GetMyHandBookResourcesPerCategoryNext(categoryName, userId);
		}
		#endregion MyHandBookGetResourcesPerCategory

		#region GetCategoryResources
		public IAFCHandBookMyHandBookResourceModelModel GetCategoryResources(Guid categoryId, bool showAllResources, string userId = null, string orderBy = OrderByMostRecent)
		{
			return GetCategoryResourcesNext(categoryId, showAllResources, userId, orderBy);
		}
		#endregion GetCategoryResources

		#region GetMyHandBookCategoryResources
		public IAFCHandBookMyHandBookResourceModelModel GetMyHandBookCategoryResourcesByName(String categoryName, String userId = null, String orderBy = OrderByMostRecent)
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
			if (resourcesAmount != 0)
			{
				resources.Take(resourcesAmount);
			}

			foreach (var resourceItem in resources)
			{
				resourceItemModel = GetResourceDetails(resourceItem, null, true);
				model.Add(resourceItemModel);
			}

			return model;
		}

		public List<IAFCHandBookResourceModel> GetMyHandBookCategoryResourcesList(Guid categoryId)
		{
			return GetMyHandBookCategoryResourcesListNext(categoryId);
		}


		#endregion GetMyHandBookCategoryResources

		#region Get My Hnadbook ResourceDetails by Name
		public IAFCHandBookResourceModel GetMyHnadbookResourceDetails(string name, Guid? categoryId = null)
		{
			return GetMyHnadbookResourceDetailsNext(name, categoryId);
		}
		#endregion Get My Hnadbook ResourceDetails by Name

		#region Get My Hand Book MoreResources
		public List<IAFCHandBookMoreResourcesModel> GetMyHandBookMoreResources(Guid resourceId, Guid categoryID)
		{
			return GetMyHandBookMoreResourcesNext(resourceId, categoryID);
		}
		#endregion Get My Hand Book MoreResources

		#region generateSharedUrl
		public string GenerateSharedUrl(String url)
		{
			var returnUrl = String.Empty;
			returnUrl = url + "/" + SecurityManager.GetCurrentUserId().ToString();
			return returnUrl;
		}

		#endregion generateSharedUrl

		#endregion MyHandBook

		#region Menu
		public IAFCHandBookMyHandBookMenuModel GetMenu(String urlPath)
		{
			IAFCHandBookMyHandBookMenuModel model = new IAFCHandBookMyHandBookMenuModel();
			var topicMenuItem = new IAFCHandBookMyHandBookMenuItemModel();
			topicMenuItem.Title = "Topics";
			topicMenuItem.Url = String.Empty;
			topicMenuItem.Visible = true;
			foreach (var categoryItem in topicParentCategories)
			{
				var menuItem = new IAFCHandBookMyHandBookMenuItemModel();
				var categoryDetails = GetTopicCategories(categoryItem);

				menuItem.Title = categoryDetails.ResourceCategoryTile;
				menuItem.Visible = true;
				menuItem.Url = String.Empty;
				foreach (var childCategoryItem in GetChildCategories(categoryItem))
				{
					var childMenuItem = new IAFCHandBookMyHandBookMenuItemModel();
					var childCategoryDetails = GetTopicCategories(childCategoryItem);
					childMenuItem.Title = childCategoryDetails.ResourceCategoryTile;
					childMenuItem.Url = childCategoryDetails.ResourceCategoryUrl;
					childMenuItem.Visible = true;
					menuItem.ChildMenuItem.Add(childMenuItem);
				}
				topicMenuItem.ChildMenuItem.Add(menuItem);
			}
			model.Menu.Add(topicMenuItem);

			var isUserSignIn = IsUserAuthorized();
			var otherMenuItem = new IAFCHandBookMyHandBookMenuItemModel();
			otherMenuItem.Title = "My HandBook";
			otherMenuItem.Url = MainPage + "/my-handbook/";
			otherMenuItem.Visible = isUserSignIn;
			model.Menu.Add(otherMenuItem);

			otherMenuItem = new IAFCHandBookMyHandBookMenuItemModel();
			otherMenuItem.Title = "Account";
			otherMenuItem.Url = MainPage + "/account/";
			otherMenuItem.Visible = isUserSignIn;
			model.Menu.Add(otherMenuItem);

			otherMenuItem = new IAFCHandBookMyHandBookMenuItemModel();
			otherMenuItem.Title = "SignIn";
			otherMenuItem.Url = "/Mxg/AuthService/SignInByHelix/?ReturnUrl=" + urlPath;
			otherMenuItem.Visible = !isUserSignIn;
			model.Menu.Add(otherMenuItem);

			otherMenuItem = new IAFCHandBookMyHandBookMenuItemModel();
			otherMenuItem.Title = "LogOut";
			otherMenuItem.Url = "/Mxg/AuthService/SignOut/?ReturnUrl=" + urlPath;
			otherMenuItem.Visible = isUserSignIn;
			model.Menu.Add(otherMenuItem);

			return model;
		}

		#endregion Menu

		#region GetSearchedResources

		public IAFCHandBookSearchedResourcesModel GetSearchedResourcres(string searchText, string orderBy = OrderByMostRecent)
		{
			var model = new IAFCHandBookSearchedResourcesModel();
			model.SearchText = searchText;

			DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();

			var searchedResources = dynamicModuleManager.GetDataItems(handBookResourcesType)
				.Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live && d.GetValue<string>("Title").Contains(searchText));

			var searchedResourcesList = new List<DynamicContent>();
			if (orderBy == OrderByTopic)
			{
				searchedResourcesList = searchedResources.ToList()
					.OrderBy(r => categoriesDictionaly[r.GetValue<IList<Guid>>("Category").Where(c => topicCategories.Contains(c)).First()].ResourceCategoryTile).ToList();
			}
			else if (orderBy == OrderByMostRecent)
			{
				searchedResourcesList = searchedResources.OrderByDescending(r => r.DateCreated).ToList(); ;
			}
			else if (orderBy == OrderByMostPopular)
			{
				searchedResourcesList = searchedResources.OrderByDescending(r => r.GetValue<decimal?>("AmountOfLikes")).ToList(); ;
			}
			else if (orderBy == OrderByAlphabeticalAZ)
			{
				searchedResourcesList = searchedResources.OrderBy(r => r.GetValue<string>("Title")).ToList(); ;
			}
			else if (orderBy == OrderByAlphabeticalZA)
			{
				searchedResourcesList = searchedResources.OrderByDescending(r => r.GetValue<string>("Title")).ToList(); ;
			}

			var listOfMyResources = new List<IAFCHandBookResourceModel>();

			foreach (var res in searchedResourcesList)
			{
				var handBookResource = GetResourceDetailsNext(res, null);
				listOfMyResources.Add(handBookResource);
			}
			model.Resources = listOfMyResources;
			var orderByList = InitOrderBy(orderBy, "", true);
			model.OrderBy = orderByList;

			return model;
		}


		#endregion GetSearchedResources		

		#region SendEmails
		private void SendEmails(List<ISubscriberRequest> subscribers)
		{
			try
			{
				var ns = SystemManager.GetNotificationService();
				var context = new ServiceContext("myNotificationAccount", "MyCustomModule");

				var contextDictionary = new Dictionary<string, string>();
				contextDictionary.Add("MergeData.Time", DateTime.UtcNow.ToString());

				var profileName = "IAFCHandBookNotification"; //Name of an existing profile
				var subjectTemplate = "Test notification";
				var bodyTemplate = "Hi {|Subscriber.FirstName|} {|Subscriber.LastName|}, the time is: {|MergeData.Time|}";
				var tmpl = new MessageTemplateRequestProxy() { Subject = subjectTemplate, BodyHtml = bodyTemplate };

				IMessageJobRequest job = new MessageJobRequestProxy()
				{
					MessageTemplate = tmpl,
					Subscribers = subscribers,
					SenderProfileName = profileName
				};

				var messageJobId = ns.SendMessage(context, job, contextDictionary);

			}
			catch (Exception e)
			{
				log.Error("Sent Email Error:" + e.Message);
			}

		}
		#endregion SendEmails

		#region FollowCategory
		public Boolean FollowCategory(Guid categoryId)
		{
			Boolean returnData = false;
			try
			{
				var myHandBookItem = GetOrCreateMyHandBook();

				DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();
				TaxonomyManager taxonomyManager = TaxonomyManager.GetManager();


				var masterHandBook = dynamicModuleManager.Lifecycle.GetMaster(myHandBookItem);
				var checkOutHandBook = dynamicModuleManager.Lifecycle.CheckOut(masterHandBook) as DynamicContent;

				checkOutHandBook.Organizer.AddTaxa("Category", categoryId);

				masterHandBook = dynamicModuleManager.Lifecycle.CheckIn(checkOutHandBook) as DynamicContent;
				dynamicModuleManager.Lifecycle.Publish(masterHandBook);
				dynamicModuleManager.SaveChanges();

				returnData = true;
			}
			catch (Exception e)
			{
				log.Error("FollowCategory Error: " + e.Message);
			}
			return returnData;
		}

		#endregion FollowCategory

		#region IsCategoryFollowed
		public Boolean IsCategoryFollowed(Guid categoryId)
		{

			Boolean returnData = false;
			var i = 0;
			try
			{
				var myHandBookItem = GetOrCreateMyHandBook();

				var providerName = String.Empty;
				var transactionName = "myHandBookTransaction";

				DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager(providerName, transactionName);

				var categoryFollowed = myHandBookItem.GetValue<IList<Guid>>("Category").Contains(categoryId);

				returnData = categoryFollowed;
			}
			catch (Exception e)
			{
				log.Error("IsResourceAddedToMyHandBook Error: " + e.Message);
			}
			return returnData;
		}
		#endregion IsCategoryFollowed

		#region SendNotification
		private void SendNotification(Guid resourceId)
		{
			try
			{
				var myHandBookItem = GetOrCreateMyHandBook();

				DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();
				UserManager userManager = UserManager.GetManager();
				UserProfileManager profileManager = UserProfileManager.GetManager();
				List<ISubscriberRequest> subscribers = new List<ISubscriberRequest>();
				var resourceCategory = dynamicModuleManager.GetDataItems(handBookResourcesType)
					.Where(d => d.Id == resourceId)
					.ToArray()
					.Select(r => r.GetValue<IList<Guid>>("Category").Where(c => topicCategories.Contains(c)).First())
					.First();

				var handBookList = dynamicModuleManager.GetDataItems(myHandBookType)
					.Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live)
					.ToArray()
					.Where(h => h.GetValue<IList<Guid>>("Category").Contains(resourceCategory))
					.ToArray();

				var users = userManager.GetUsers();
				foreach (var handBook in handBookList)
				{
					var userId = handBook.GetValue<Guid>("UserId");
					User user = userManager.GetUser(userId);

					var profile = profileManager.GetUserProfile<SitefinityProfile>(user);
					var key = String.Empty;
					var firstName = String.Empty;
					var lastName = String.Empty;
					var email = String.Empty;
					if (profile != null)
					{

						email = profile.User.Email;
						firstName = profile.FirstName;
						lastName = profile.LastName;
						key = profile.GetKey();
					}
					else
					{
						firstName = user.FirstName;
						lastName = user.LastName;
						email = user.Email;
					}
					var subscriber = new SubscriberRequestProxy()
					{
						Email = email,
						FirstName = firstName,
						LastName = lastName,
						ResolveKey = userId.ToString()
					};
					subscribers.Add(subscriber);
				}

				SendEmails(subscribers);
			}
			catch (Exception e)
			{
				log.Error("Send Notification Error: " + e.Message);
			}
		}
		#endregion SendNotification

		#region GetAccount
		public IAFCHandBookAccount GetAccount()
		{
			IAFCHandBookAccount model = new IAFCHandBookAccount();

			var myHandBookItem = GetOrCreateMyHandBook();
			model.WeeklyUpdates = Convert.ToBoolean(myHandBookItem.GetValue("WeeklyUpdates"));
			model.MonthlyUpdates = Convert.ToBoolean(myHandBookItem.GetValue("MonthlyUpdates"));

			var foollowedCategories = myHandBookItem.GetValue<IList<Guid>>("Category").Where(c => topicCategories.Contains(c));

			foreach (var categoryId in foollowedCategories)
			{
				var category = new IAFCHandBookTopicCategoryModel();
				var topicCategoryDetails = GetTopicCategories(categoryId);

				category.Id = categoryId;
				category.CategoryTitle = topicCategoryDetails.ResourceCategoryTile;
				category.ParentCategoryTitle = topicCategoryDetails.ResourceParentCategoryTitle;

				model.FollowedCategories.Add(category);
			}

			return model;
		}
		#endregion GetAccount

		#region WeeklyUpdates
		public void WeeklyUpdates(bool value)
		{
			DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();
			var myHandBookItem = GetOrCreateMyHandBook();
			var masterHandBookItem = dynamicModuleManager.Lifecycle.GetMaster(myHandBookItem);
			DynamicContent checkOutHandBookItem = dynamicModuleManager.Lifecycle.CheckOut(masterHandBookItem) as DynamicContent;
			checkOutHandBookItem.SetValue("WeeklyUpdates", value);
			ILifecycleDataItem checkInHandBookItem = dynamicModuleManager.Lifecycle.CheckIn(checkOutHandBookItem);
			dynamicModuleManager.Lifecycle.Publish(checkInHandBookItem);
			dynamicModuleManager.SaveChanges();
		}
		#endregion WeeklyUpdates

		#region MonthlyUpdates
		public void MonthlyUpdates(bool value)
		{
			DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();
			var myHandBookItem = GetOrCreateMyHandBook();
			var masterHandBookItem = dynamicModuleManager.Lifecycle.GetMaster(myHandBookItem);
			DynamicContent checkOutHandBookItem = dynamicModuleManager.Lifecycle.CheckOut(masterHandBookItem) as DynamicContent;
			checkOutHandBookItem.SetValue("MonthlyUpdates", value);
			ILifecycleDataItem checkInHandBookItem = dynamicModuleManager.Lifecycle.CheckIn(checkOutHandBookItem);
			dynamicModuleManager.Lifecycle.Publish(checkInHandBookItem);
			dynamicModuleManager.SaveChanges();
		}
		#endregion MonthlyUpdates

		#region Unfollow
		public void Unfollow(Guid categoryId)
		{
			DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();
			var myHandBookItem = GetOrCreateMyHandBook();
			var categories = myHandBookItem.GetValue<TrackedList<Guid>>("Category")
				.ToArray()
				.Where(c => c != categoryId)
				.ToArray();

			var masterHandBookItem = dynamicModuleManager.Lifecycle.GetMaster(myHandBookItem);
			DynamicContent checkOutHandBookItem = dynamicModuleManager.Lifecycle.CheckOut(masterHandBookItem) as DynamicContent;
			checkOutHandBookItem.Organizer.Clear("Category");
			checkOutHandBookItem.Organizer.AddTaxa("Category", categories);
			ILifecycleDataItem checkInHandBookItem = dynamicModuleManager.Lifecycle.CheckIn(checkOutHandBookItem);
			dynamicModuleManager.Lifecycle.Publish(checkInHandBookItem);
			dynamicModuleManager.SaveChanges();
		}
		#endregion Unfollow

		#region getDefaultImageUrl
		public string GetDefaultImageUrl(string resourceType)
		{
			string imgUrl = String.Empty;

			return imgUrl;
		}
		#endregion getDefaultImageUrl

		public void InitResourceTypeImages()
		{
			resourceTypeImages.Add("Video", DefaultVideoImgUrl);
			resourceTypeImages.Add("Webinar", DefaultWebinarImgUrl);
			resourceTypeImages.Add("Article", DefaultArticleImgUrl);

			/*DefaultPodcastImgUrl;
			DefaultChartImgUrl;		
			DefaultLinkImgUrl;
			DefaultImageImgUrl;				
			DefaultAudioImgUrl;
			DefaultBookImgUrl;*/
		}

		public string GetUserName(Guid userId)
		{
			string userName = string.Empty;
			UserProfileManager profileManager = UserProfileManager.GetManager();
			UserManager userManager = UserManager.GetManager();

			User user = userManager.GetUser(userId);
			SitefinityProfile profile = null;

			if (user != null)
			{
				profile = profileManager.GetUserProfile<SitefinityProfile>(user);
				if (profile != null)
				{
					userName = profile.Nickname;
				}
				else
				{

					userName = user.FirstName + " " + user.LastName;
				}
			}
			return userName;
		}
	
	}
}



	