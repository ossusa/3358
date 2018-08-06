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
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Taxonomies.Model;
using Telerik.Sitefinity.Utilities.TypeConverters;
using Telerik.Sitefinity.Libraries.Model;
using Telerik.Sitefinity.Data.Linq.Dynamic;
using Telerik.Sitefinity.Data;
using ServiceStack.Logging;

namespace SitefinityWebApp.Custom.IAFCHandBook
{		

	public class IAFCHandBookHelper
	{
		private ILog log = LogManager.GetLogger(typeof(IAFCHandBookHelper));

		#region Structs
		private struct Categories
		{			
			public String ResourceCategoryUrl { get; set; }
			public String ResourceParentCategoryUrl { get; set; }			
			public String ResourceParentCategoryTitle { get; set; }
			public String ResourceParentCategoryImageUrl { get; set; }
		}
		#endregion Structs
				
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

		#region DynamicTypes
		Type handBookResourcesType = TypeResolutionService.ResolveType("Telerik.Sitefinity.DynamicTypes.Model.IAFCHandBookResourcesData.Iafchandbookresourcesdata");																	   
		Type externalResourcesType = TypeResolutionService.ResolveType("Telerik.Sitefinity.DynamicTypes.Model.HandbookResources.ResourcesExternal");																		
		Type resourcesType = TypeResolutionService.ResolveType("Telerik.Sitefinity.DynamicTypes.Model.Resources.Resource");
		#endregion DynamicTypes

		#region CategoriesName
		public const String LeadershipMotivatingPeopleName =  "motivating-people";
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

		private const string TopicCommynityImageUrl = "/Sitefinity/WebsiteTemplates/IAFCHandBook/App_Themes/IAFCHandBook/images/community.jpg";
		private const string TopicLeadershipImageUrl = "/Sitefinity/WebsiteTemplates/IAFCHandBook/App_Themes/IAFCHandBook/images/leadership.jpg";
		private const string TopicFinanceImageUrl = "/Sitefinity/WebsiteTemplates/IAFCHandBook/App_Themes/IAFCHandBook/images/finance.jpg";
		private const string TopicPersonnelImageUrl = "/Sitefinity/WebsiteTemplates/IAFCHandBook/App_Themes/IAFCHandBook/images/personnel.jpg";
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
		public Guid Leadership = Guid.Empty;
		public Guid Personnel = Guid.Empty;
		public Guid Finance = Guid.Empty;
		public Guid Community = Guid.Empty;
		public Guid DepartmentAdministration = Guid.Empty;
		#endregion Guids

		public List<Guid> topicCategories = new List<Guid>();
		public List<Guid> topicFinanceCategories = new List<Guid>();
		public List<Guid> topicLeadershipCategories = new List<Guid>();
		public List<Guid> topicPersonnelCategories = new List<Guid>();
		public List<Guid> topicCommunityRelationsCategories = new List<Guid>();

		#endregion Variables

		#region Constructor
		public IAFCHandBookHelper()
		{
			InitCategoriesGuid();
			InitCategoriesLists();
			
		}
		#endregion Constructor

		#region InitCategories

		#region InitCategoriesGuid
		public void InitCategoriesGuid()
		{
			TaxonomyManager taxonomyManager = TaxonomyManager.GetManager();
			//Leadership
			var currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == LeadershipMotivatingPeopleName).FirstOrDefault();
			LeadershipMotivatingPeople = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == LeadershipLeadershipStylesName).FirstOrDefault();
			LeadershipLeadershipStyles = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == LeadershipEthicsName).FirstOrDefault();
			LeadershipEthics = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == LeadershipGenerationalDifferencesName).FirstOrDefault();
			LeadershipGenerationalDifferences = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == LeadershipStrategyName).FirstOrDefault();
			LeadershipStrategy = currentResourceCategory.Id;

			//Personnel
			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == PersonnelVolunteerCareerRelationsName).FirstOrDefault();
			PersonnelVolunteerCareerRelations = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == PersonnelRecruitmentName).FirstOrDefault();
			PersonnelRecruitment = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == PersonnelRetentionName).FirstOrDefault();
			PersonnelRetention = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == PersonnelLegalIssuesName).FirstOrDefault();
			PersonnelLegalIssues = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == PersonnelInsuranceName).FirstOrDefault();
			PersonnelInsurance = currentResourceCategory.Id;

			//Finance
			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == FinanceBudgetingName).FirstOrDefault();
			FinanceBudgeting = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == FinanceFundraisingName).FirstOrDefault();
			FinanceFundraising = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == FinanceLegalIssuesName).FirstOrDefault();
			FinanceLegalIssues = currentResourceCategory.Id;

			//Community Relations
			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == CommunityRelationsCustomerServiceName).FirstOrDefault();
			CommunityRelationsCustomerService = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == CommunityRelationsMarketingMediaName).FirstOrDefault();
			CommunityRelationsMarketingMedia = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == CommunityRelationsPoliticsName).FirstOrDefault();
			CommunityRelationsPolitics = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == CommunityRelationsCrisisCommunicationName).FirstOrDefault();
			CommunityRelationsCrisisCommunication = currentResourceCategory.Id;

			//Other
			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == Featured_VWS_A_RITName).FirstOrDefault();
			Featured_VWS_A_RIT = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == LeadershipName).FirstOrDefault();
			Leadership = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == PersonnelName).FirstOrDefault();
			Personnel = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == FinanceName).FirstOrDefault();
			Finance = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == CommunityName).FirstOrDefault();
			Community = currentResourceCategory.Id;

			currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Name == DepartmentAdministrationName).FirstOrDefault();
			DepartmentAdministration = currentResourceCategory.Id;
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
					categoryID = Leadership;
					break;
				case PersonnelName:
					categoryID = Personnel;
					break;
				case FinanceName:
					categoryID = Finance;
					break;
				case CommunityName:
					categoryID = Community;
					break;
				case DepartmentAdministrationName:
					categoryID = DepartmentAdministration;
					break;
			}
			return categoryID;
		}
		#endregion GetCategoryGuidByName

		#region InitCategoriesLists
		public void InitCategoriesLists()
		{
			InitTopicCategories();
			InitTopicLeadershipCategories();
			InitTopicPersonnelCategoriesLists();
			InitTopicFinanceCategories();
			InitTopicCommunityRelationsCategories();
		}
		#endregion InitCategoriesLists

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

		#endregion InitCategories

		#region GetCategories
		private Categories GetTopicCategories(Guid categoryID)
		{
			String ResourceParentCategoryTitle = String.Empty;
			String ResourceParentCategoryUrl = String.Empty;
			String ResourceParentCategoryImageUrl = String.Empty;
			String ResourceCategoryUrl = String.Empty;
			

			if (categoryID == LeadershipMotivatingPeople)
			{				
				ResourceParentCategoryUrl = TopicLeadershipUrl;				
				ResourceParentCategoryTitle = LeadershipCategoryTitle;
				ResourceParentCategoryImageUrl = TopicLeadershipImageUrl;
				ResourceCategoryUrl = TopicLeadershipMotivatingPeopleUrl;
			}
			else if (categoryID == LeadershipLeadershipStyles)
			{
				ResourceParentCategoryUrl = TopicLeadershipUrl;				
				ResourceParentCategoryTitle = LeadershipCategoryTitle;
				ResourceParentCategoryImageUrl = TopicLeadershipImageUrl;
				ResourceCategoryUrl = TopicLeadershipLeadershipStylesUrl;
			}
			else if (categoryID == LeadershipEthics)
			{
				ResourceParentCategoryUrl = TopicLeadershipUrl;				
				ResourceParentCategoryTitle = LeadershipCategoryTitle;
				ResourceParentCategoryImageUrl = TopicLeadershipImageUrl;
				ResourceCategoryUrl = TopicLeadershipEthicsUrl;
			}
			else if (categoryID == LeadershipGenerationalDifferences)
			{
				ResourceParentCategoryUrl = TopicLeadershipUrl;				
				ResourceParentCategoryTitle = LeadershipCategoryTitle;
				ResourceParentCategoryImageUrl = TopicLeadershipImageUrl;
				ResourceCategoryUrl = TopicLeadershipGenerationalDifferencesUrl;
			}
			else if (categoryID == LeadershipStrategy)
			{
				ResourceParentCategoryUrl = TopicLeadershipUrl;				
				ResourceParentCategoryTitle = LeadershipCategoryTitle;
				ResourceParentCategoryImageUrl = TopicLeadershipImageUrl;
				ResourceCategoryUrl = TopicLeadershipStrategyUrl;
			}
			else if (categoryID == PersonnelVolunteerCareerRelations)
			{
				ResourceParentCategoryUrl = TopicPersonnelUrl;				
				ResourceParentCategoryTitle = PersonnelCategoryTitle;
				ResourceParentCategoryImageUrl = TopicPersonnelImageUrl;
				ResourceCategoryUrl = TopicPersonnelVolunteerCareerRelationsUrl;
			}
			else if (categoryID == PersonnelRecruitment)
			{
				ResourceParentCategoryUrl = TopicPersonnelUrl;				
				ResourceParentCategoryTitle = PersonnelCategoryTitle;
				ResourceParentCategoryImageUrl = TopicPersonnelImageUrl;
				ResourceCategoryUrl = TopicPersonnelRecruitmentUrl;
			}
			else if (categoryID == PersonnelRetention)
			{
				ResourceParentCategoryUrl = TopicPersonnelUrl;				
				ResourceParentCategoryTitle = PersonnelCategoryTitle;
				ResourceParentCategoryImageUrl = TopicPersonnelImageUrl;
				ResourceCategoryUrl = TopicPersonnelRetentionUrl;
			}
			else if (categoryID == PersonnelLegalIssues)
			{
				ResourceParentCategoryUrl = TopicPersonnelUrl;				
				ResourceParentCategoryTitle = PersonnelCategoryTitle;
				ResourceParentCategoryImageUrl = TopicPersonnelImageUrl;
				ResourceCategoryUrl = TopicPersonnelLegalIssuesUrl;
			}
			else if (categoryID == PersonnelInsurance)
			{
				ResourceParentCategoryUrl = TopicPersonnelUrl;				
				ResourceParentCategoryTitle = PersonnelCategoryTitle;
				ResourceParentCategoryImageUrl = TopicPersonnelImageUrl;
				ResourceCategoryUrl = TopicPersonnelInsuranceUrl;
			}
			else if (categoryID == FinanceBudgeting)
			{
				ResourceParentCategoryUrl = TopicFinanceUrl;				
				ResourceParentCategoryTitle = FinanceCategoryTitle;
				ResourceParentCategoryImageUrl = TopicFinanceImageUrl;
				ResourceCategoryUrl = TopicFinanceBudgetingUrl;
			}
			else if (categoryID == FinanceFundraising)
			{
				ResourceParentCategoryUrl = TopicFinanceUrl;				
				ResourceParentCategoryTitle = FinanceCategoryTitle;
				ResourceParentCategoryImageUrl = TopicFinanceImageUrl;
				ResourceCategoryUrl = TopicFinanceFundraisingUrl;
			}
			else if (categoryID == FinanceLegalIssues)
			{
				ResourceParentCategoryUrl = TopicFinanceUrl;				
				ResourceParentCategoryTitle = FinanceCategoryTitle;
				ResourceParentCategoryImageUrl = TopicFinanceImageUrl;
				ResourceCategoryUrl = TopicFinanceLegalIssuesUrl;
			}
			else if (categoryID == CommunityRelationsCustomerService)
			{
				ResourceParentCategoryUrl = TopicCommynityUrl;				
				ResourceParentCategoryTitle = CommunityCategoryTitle;
				ResourceParentCategoryImageUrl = TopicCommynityImageUrl;
				ResourceCategoryUrl = TopicCommynityCustomerServiceUrl;
			}
			else if (categoryID == CommunityRelationsMarketingMedia)
			{
				ResourceParentCategoryUrl = TopicCommynityUrl;				
				ResourceParentCategoryTitle = CommunityCategoryTitle;
				ResourceParentCategoryImageUrl = TopicCommynityImageUrl;
				ResourceCategoryUrl = TopicCommynityMarketingAndMediaUrl;
			}
			else if (categoryID == CommunityRelationsPolitics)
			{
				ResourceParentCategoryUrl = TopicCommynityUrl;				
				ResourceParentCategoryTitle = CommunityCategoryTitle;
				ResourceParentCategoryImageUrl = TopicCommynityImageUrl;
				ResourceCategoryUrl = TopicCommynityPoliticsUrl;
			}
			if (categoryID == CommunityRelationsCrisisCommunication)
			{
				ResourceParentCategoryUrl = TopicCommynityUrl;				
				ResourceParentCategoryTitle = CommunityCategoryTitle;
				ResourceParentCategoryImageUrl = TopicCommynityImageUrl;
				ResourceCategoryUrl = TopicCommynityCrisisCommunicationUrl;
			}

			Categories categories = new Categories();
			categories.ResourceParentCategoryTitle = ResourceParentCategoryTitle;
			categories.ResourceParentCategoryUrl= ResourceParentCategoryUrl;
			categories.ResourceParentCategoryImageUrl = ResourceParentCategoryImageUrl;
			categories.ResourceCategoryUrl = ResourceCategoryUrl;

			return categories;
		}
		#endregion GetCategories

		#region GetResourceDetailsInfo
		public IAFCHandBookResourceDetailsModel GetResourceDetailsInfo(DynamicContent resource)
		{
			var resourceInfo = new IAFCHandBookResourceDetailsModel();
			String resourceTypeTitle = String.Empty;
			String resourceCategoryTitle = String.Empty;
			String resourceCategoryUrl = String.Empty;
			String resourceParentCategoryTitle = String.Empty;
			String resourceParentCategoryUrl = String.Empty;

			#region GetResource

			var externalResourcesExist = resource.GetRelatedItems("ExternalResources").Cast<DynamicContent>().Count();
			if (externalResourcesExist > 0)
			{
				var externalResourceItem = resource.GetRelatedItems("ExternalResources").Cast<DynamicContent>().First();

				//Get single field
				resourceInfo.id = externalResourceItem.Id;
				resourceInfo.ResourceTitle = externalResourceItem.GetValue("Title").ToString();
				resourceInfo.ResourceText = externalResourceItem.GetValue("shortsummary").ToString();

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
					log.Error("GetResourceDetails cant parse duration time = '" + externalResourceItem.GetValue("Time").ToString() + "'");
				}


				resourceInfo.Duration = duration;
				resourceInfo.DurationStr = duration.ToString();
				resourceInfo.VideoEmbedCode = externalResourceItem.GetValue("VideoEmbedCode").ToString();

				TaxonomyManager taxonomyManager = TaxonomyManager.GetManager();
				//get first resource type
				var resourceTypesID = externalResourceItem.GetPropertyValue<TrackedList<Guid>>("resourcetypes").FirstOrDefault();
				if (resourceTypesID != null)
				{
					resourceTypeTitle = taxonomyManager.GetTaxon(resourceTypesID).Title.ToString();
				}

				//get first category
				var resourceCategoriesIDs = externalResourceItem.GetPropertyValue<TrackedList<Guid>>("Category");
				var categoryItem = resourceCategoriesIDs.Where(c => topicCategories.Contains(c)).FirstOrDefault();
				if (categoryItem != null)
				{
					var resourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Id == categoryItem).FirstOrDefault();
					resourceCategoryTitle = resourceCategory.Title.ToString();


					var topicCategory = GetTopicCategories(categoryItem);
					resourceParentCategoryTitle = topicCategory.ResourceParentCategoryTitle;
					resourceParentCategoryUrl = topicCategory.ResourceParentCategoryUrl;
					resourceCategoryUrl = topicCategory.ResourceCategoryUrl;

				}


				var img = externalResourceItem.GetRelatedItems<Image>("featuredimage").SingleOrDefault();

				resourceInfo.Category.Id = categoryItem;
				resourceInfo.Category.CategoryTitle = resourceCategoryTitle;
				resourceInfo.Category.CategoryUrl = resourceCategoryUrl;
				resourceInfo.Category.ParentCategoryTitle = resourceParentCategoryTitle;
				resourceInfo.Category.ParentCategoryUrl = resourceParentCategoryUrl;
				resourceInfo.ResourceType = resourceTypeTitle;
				resourceInfo.ImageUrl = img.Url;
				//resourceInfo.handBookResource.ResourceUrl = resourceCategoryUrl + "/resourcedetails/" + resoucre.UrlName.ToString();

			}
			else
			{
				var resourcesExist = resource.GetRelatedItems("Resources").Cast<DynamicContent>().Count();
				if (resourcesExist > 0)
				{

					var resourceItem = resource.GetRelatedItems("Resources").Cast<DynamicContent>().First();

					resourceInfo.id = resourceItem.Id;
					resourceInfo.ResourceTitle = resourceItem.GetValue("Title").ToString();
					resourceInfo.ResourceText = resourceItem.GetValue("shortsummary").ToString();
					resourceInfo.Duration = new TimeSpan();
					resourceInfo.VideoEmbedCode = String.Empty;

					TaxonomyManager taxonomyManager = TaxonomyManager.GetManager();
					//get first resource type
					var resourceTypesID = resourceItem.GetPropertyValue<TrackedList<Guid>>("resourcetypes").FirstOrDefault();
					if (resourceTypesID != null)
					{
						resourceTypeTitle = taxonomyManager.GetTaxon(resourceTypesID).Title.ToString();
					}

					//get first category
					var resourceCategoriesIDs = resourceItem.GetPropertyValue<TrackedList<Guid>>("Category");
					var categoryItem = resourceCategoriesIDs.Where(c => topicCategories.Contains(c)).FirstOrDefault();
					if (categoryItem != null)
					{
						var resourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Id == categoryItem).FirstOrDefault();
						resourceCategoryTitle = resourceCategory.Title.ToString();

						var topicCategory = GetTopicCategories(categoryItem);
						resourceParentCategoryTitle = topicCategory.ResourceParentCategoryTitle;
						resourceParentCategoryUrl = topicCategory.ResourceParentCategoryUrl;
						resourceCategoryUrl = topicCategory.ResourceCategoryUrl;

					}


					var img = resourceItem.GetRelatedItems<Image>("featuredimage").SingleOrDefault();

					resourceInfo.Category.Id = categoryItem;
					resourceInfo.Category.CategoryTitle = resourceCategoryTitle;
					resourceInfo.Category.CategoryUrl = resourceCategoryUrl;
					resourceInfo.Category.ParentCategoryTitle = resourceParentCategoryTitle;
					resourceInfo.Category.ParentCategoryUrl = resourceParentCategoryUrl;
					resourceInfo.ResourceType = resourceTypeTitle;
					resourceInfo.ImageUrl = img.Url;
				}
			}
			#endregion GetResource

			return resourceInfo;

		}
		#endregion GetResourceDetailsInfo

		#region GetResourceLikesInfo
		public IAFCHandBookLikesModel GetResourceLikesInfo(DynamicContent resource)
		{
			DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();
			var resourceLike = new IAFCHandBookLikesModel();

			try
			{
				var likeExists = resource.GetRelatedItems("Likes").Cast<DynamicContent>().Count();
				var like = new DynamicContent();
				if (likeExists > 0)
				{
					like = resource.GetRelatedItems("Likes").Cast<DynamicContent>().First();
				}
				else
				{
					//Create like for Resource
					Type mylikesmoduleType = TypeResolutionService.ResolveType("Telerik.Sitefinity.DynamicTypes.Model.IAFCHandBookLikes.Iafchandbooklikes");
					like = dynamicModuleManager.CreateDataItem(mylikesmoduleType);
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
					var liveResource = dynamicModuleManager.GetDataItem(handBookResourcesType, resource.Id);
					var masterResource = dynamicModuleManager.Lifecycle.GetMaster(liveResource);

					DynamicContent checkOutResourceItem = dynamicModuleManager.Lifecycle.CheckOut(masterResource) as DynamicContent;
					checkOutResourceItem.CreateRelation(like, "Likes");
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
				log.Error("Can not get Likes for Resource ID = '"+ resource.Id.ToString()+ "' " + e.Message );
			}

			return resourceLike;
		}
		#endregion GetResourceLikesInfo

		#region GetResourceDetails
		public IAFCHandBookResourceModel GetResourceDetails(DynamicContent resource)
		{
			DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();
			
			var handBookResource = new IAFCHandBookResourceModel(resource.Id,
												resource.GetValue("Title").ToString(),
												resource.DateCreated);
			
			
			#region GetResource						
			handBookResource.ResourceDetails = GetResourceDetailsInfo(resource);
			handBookResource.ResourceUrl = handBookResource.ResourceDetails.Category.CategoryUrl + "/resourcedetails/" + resource.UrlName.ToString();
			#endregion GetResource

			handBookResource.Likes = GetResourceLikesInfo(resource);
			var resourceCommentsAmount = resource.GetRelatedItems("Comment").Count();
			handBookResource.CommentsAmount = resourceCommentsAmount;
			handBookResource.AddToMyHandBook = false;
			

			return handBookResource;
		}
		#endregion GetResourceDetails

		#region GetFeaturedResources
		public IAFCHandBookResourceModel GetFeaturedResourcesData()
		{

			DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();

			IAFCHandBookResourceModel handBookResource = new IAFCHandBookResourceModel();

			var featuredResource = dynamicModuleManager.GetDataItems(handBookResourcesType).
						Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live).
						OrderByDescending(i=>i.DateCreated).ToList().
						Where(i => (((i.GetValue<DynamicContent>("ExternalResources") != null) &&
									(i.GetValue<DynamicContent>("ExternalResources").GetValue<IList<Guid>>("Category").Contains(Featured_VWS_A_RIT)))
									|| ((i.GetValue<DynamicContent>("Resources") != null) &&
									(i.GetValue<DynamicContent>("Resources").GetValue<IList<Guid>>("Category").Contains(Featured_VWS_A_RIT))))).
						FirstOrDefault();


			if (featuredResource != null)
			{
				handBookResource = GetResourceDetails(featuredResource);
			}

			return handBookResource;
		}
		#endregion GetFeaturedResources

		#region GetRecentlyAddedResources
		public List<IAFCHandBookResourceModel> GetRecentlyAddedResources()
		{

			DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();

			IAFCHandBookResourceModel handBookResource = new IAFCHandBookResourceModel();

			var recentlyAddedResources = dynamicModuleManager.GetDataItems(handBookResourcesType).
						Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live).
						OrderByDescending(i => i.DateCreated).ToList().
						Where(i => (((i.GetValue<DynamicContent>("ExternalResources") != null) &&
									(i.GetValue<DynamicContent>("ExternalResources").GetValue<IList<Guid>>("Category").Where(c=> topicCategories.Contains(c)).Count()>0) )
									|| ((i.GetValue<DynamicContent>("Resources") != null) &&
									(i.GetValue<DynamicContent>("Resources").GetValue<IList<Guid>>("Category").Where(c => topicCategories.Contains(c)).Count() > 0)))).
				Take(6);						
			
			var listOfMyResources = new List<IAFCHandBookResourceModel>();
			foreach (var item in recentlyAddedResources)
			{
				handBookResource = GetResourceDetails(item);
				listOfMyResources.Add(handBookResource);
			}

			return listOfMyResources;
		}
		#endregion GetRecentlyAddedResources

		#region GetResourcesPerCategory
		public IAFCHandBookResourcesPerCatergoryModel GetResourcesPerCategory(String categoryName, String orderBy)
		{			
			Guid categoryID = GetCategoryGuidByName(categoryName);
			IAFCHandBookResourcesPerCatergoryModel model = new IAFCHandBookResourcesPerCatergoryModel();
			DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();

			TaxonomyManager taxonomyManager = TaxonomyManager.GetManager();
			var resourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Id == categoryID).FirstOrDefault();

			var topicCategory = GetTopicCategories(categoryID);			

			model.Category.Id = categoryID;
			model.Category.CategoryTitle = resourceCategory.Title.ToString();
			model.Category.CategoryUrl = topicCategory.ResourceCategoryUrl;
			model.Category.ParentCategoryTitle = topicCategory.ResourceParentCategoryTitle;
			model.Category.CategoryDescription = resourceCategory.Description.ToString();

			var orderByList = new List<IAFCHandBookTopicOrderBy>();
			var orderByItem = new IAFCHandBookTopicOrderBy();
			orderByItem.Url = topicCategory.ResourceCategoryUrl + "/" + OrderByMostPopular;
			orderByItem.Title = OrderByMostPopular;
			if (orderBy==OrderByMostPopular)
			{
				orderByItem.Selected = true; 
			}
			orderByList.Add(orderByItem);

			orderByItem = new IAFCHandBookTopicOrderBy();
			orderByItem.Url = topicCategory.ResourceCategoryUrl + "/"+ OrderByMostRecent;
			orderByItem.Title = OrderByMostRecent;
			if (orderBy == OrderByMostRecent)
			{
				orderByItem.Selected = true;
			}
			orderByList.Add(orderByItem);

			orderByItem = new IAFCHandBookTopicOrderBy();
			orderByItem.Url = topicCategory.ResourceCategoryUrl + "/" + OrderByAlphabeticalAZ;
			orderByItem.Title = OrderByAlphabeticalAZ;
			if (orderBy == OrderByAlphabeticalAZ)
			{
				orderByItem.Selected = true;
			}
			orderByList.Add(orderByItem);

			orderByItem = new IAFCHandBookTopicOrderBy();
			orderByItem.Url = topicCategory.ResourceCategoryUrl + "/" + OrderByAlphabeticalZA;
			orderByItem.Title = OrderByAlphabeticalZA;
			if (orderBy == OrderByAlphabeticalZA)
			{
				orderByItem.Selected = true;
			}
			orderByList.Add(orderByItem);
			model.OrderBy = orderByList;

			var recentlyAddedResources = new List<DynamicContent>();

			if (orderBy == OrderByMostRecent)
			{				
					recentlyAddedResources = dynamicModuleManager.GetDataItems(handBookResourcesType).
							Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live).
							ToList().
							Where(i => (((i.GetValue<DynamicContent>("ExternalResources") != null) &&
										(i.GetValue<DynamicContent>("ExternalResources").GetValue<IList<Guid>>("Category").Contains(categoryID)))
										|| ((i.GetValue<DynamicContent>("Resources") != null) &&
										(i.GetValue<DynamicContent>("Resources").GetValue<IList<Guid>>("Category").Contains(categoryID))))).
							OrderByDescending(r => r.DateCreated).ToList();					
				
			}
			else if (orderBy == OrderByMostPopular)
			{
								
					recentlyAddedResources = dynamicModuleManager.GetDataItems(handBookResourcesType).
							Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live).
							ToList().
							Where(i => (((i.GetValue<DynamicContent>("ExternalResources") != null) &&
										(i.GetValue<DynamicContent>("ExternalResources").GetValue<IList<Guid>>("Category").Contains(categoryID)))
										|| ((i.GetValue<DynamicContent>("Resources") != null) &&
										(i.GetValue<DynamicContent>("Resources").GetValue<IList<Guid>>("Category").Contains(categoryID))))).
							OrderByDescending(r => int.Parse(r.GetValue<DynamicContent>("Likes").GetValue("AmountOfLikes").ToString())).
							ToList();
				
			}
			else if (orderBy == OrderByAlphabeticalAZ)
			{
				recentlyAddedResources = dynamicModuleManager.GetDataItems(handBookResourcesType).
							Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live).
							ToList().
							Where(i => (((i.GetValue<DynamicContent>("ExternalResources") != null) &&
										(i.GetValue<DynamicContent>("ExternalResources").GetValue<IList<Guid>>("Category").Contains(categoryID)))
										|| ((i.GetValue<DynamicContent>("Resources") != null) &&
										(i.GetValue<DynamicContent>("Resources").GetValue<IList<Guid>>("Category").Contains(categoryID))))).
							OrderBy(r => r.GetValue("Title").ToString()).
							ToList();
			}
			else if (orderBy == OrderByAlphabeticalZA)
			{
				recentlyAddedResources = dynamicModuleManager.GetDataItems(handBookResourcesType).
							Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live).
							ToList().
							Where(i => (((i.GetValue<DynamicContent>("ExternalResources") != null) &&
										(i.GetValue<DynamicContent>("ExternalResources").GetValue<IList<Guid>>("Category").Contains(categoryID)))
										|| ((i.GetValue<DynamicContent>("Resources") != null) &&
										(i.GetValue<DynamicContent>("Resources").GetValue<IList<Guid>>("Category").Contains(categoryID))))).
							OrderByDescending(r => r.GetValue("Title").ToString()).
							ToList();
			}

			

			var listOfMyResources = new List<IAFCHandBookResourceModel>();
			TimeSpan totalDuration = new TimeSpan(0, 0, 0);
			int resourcesAmount = 0;
			foreach (var item in recentlyAddedResources)
			{
				var handBookResource = GetResourceDetails(item);
				listOfMyResources.Add(handBookResource);
				totalDuration = totalDuration.Add(handBookResource.ResourceDetails.Duration);
				resourcesAmount++;
			}
			
			model.Resources = listOfMyResources;
			model.Category.ResourcesTotalDuration = totalDuration.ToString();
			model.Category.ResourcesAmount = resourcesAmount;
			model.MoreCategories = GetMoreCategories(model.Category.Id);
			
			return model;
		}
		#endregion GetResourcesPerCategory

		#region GetResourceDetails
		public IAFCHandBookResourceModel GetResourceDetails(String name)
		{
			DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();			
			var resourceItem = dynamicModuleManager.GetDataItems(handBookResourcesType).
						Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live && d.UrlName == name).
						First();

			var model = GetResourceDetails(resourceItem);
			model.MoreResources = GetMoreResources(resourceItem.Id, model.ResourceDetails.Category.Id);
			
			return model;			
		}
		#endregion GetResourceDetails

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

			foreach(var item in moreResourcesItems)
			{
				var moreResourcesItem = new IAFCHandBookMoreResourcesModel();
				moreResourcesItem.Id = item.Id;
				moreResourcesItem.ResourceDetails= GetResourceDetailsInfo(item);
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
			var currentResourceCategory = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Id == categoryId).FirstOrDefault();
				
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

			foreach(var id in moreTopicCategoriesID)
			{
				var categoryResourceAmount = GetResourcesAmountPerCategory(id);
				var newTopicCategory = new IAFCHandBookTopicCategoryModel();
				var topicCategoryItem = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(c => c.Id == id).FirstOrDefault();

				var topicCategoryDetails = GetTopicCategories(id);
				
				newTopicCategory.Id = id;
				newTopicCategory.CategoryDescription = topicCategoryItem.Description;
				newTopicCategory.CategoryTitle = topicCategoryItem.Title.ToString();
				newTopicCategory.CategoryUrl = topicCategoryDetails.ResourceCategoryUrl;
				newTopicCategory.TopicCategoryImageUrl = topicCategoryDetails.ResourceParentCategoryImageUrl;
				newTopicCategory.ResourcesAmount = categoryResourceAmount;
				moreTopicCategories.Add(newTopicCategory);
			}
			
	
			return moreTopicCategories;
		}
		#endregion GetMoreCategories

		#region GetResourcesAmountPerCategory
		private int GetResourcesAmountPerCategory(Guid categoryId)
		{
			int amount = 0;
			DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();
			amount = dynamicModuleManager.GetDataItems(handBookResourcesType).
						Where(d => d.Visible == true && d.Status == ContentLifecycleStatus.Live).
						OrderByDescending(i => i.DateCreated).ToList().
						Where(i => (((i.GetValue<DynamicContent>("ExternalResources") != null) &&
									(i.GetValue<DynamicContent>("ExternalResources").GetValue<IList<Guid>>("Category").Where(c => c == categoryId).Count() > 0))
									|| ((i.GetValue<DynamicContent>("Resources") != null) &&
									(i.GetValue<DynamicContent>("Resources").GetValue<IList<Guid>>("Category").Where(c => c == categoryId).Count() > 0)))).
						Count();
				

			return amount;
		}
		#endregion GetResourcesAmountPerCategory

		#endregion Categories Methods

		#region Likes
		public int AddLikeForResource(Guid resourceID)
		{

			DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();

			//Add Like to resource						      
			var resource = dynamicModuleManager.GetDataItem(handBookResourcesType, resourceID);
			var resourceLike = resource.GetRelatedItems("Likes").Cast<DynamicContent>().First();
			var masterResourceLike = dynamicModuleManager.Lifecycle.GetMaster(resourceLike);

			var currentLikes = Convert.ToInt32(resourceLike.GetValue("AmountOfLikes")) + 1;
			try
			{
				DynamicContent checkOutLikeItem = dynamicModuleManager.Lifecycle.CheckOut(masterResourceLike) as DynamicContent;
				checkOutLikeItem.SetValue("AmountOfLikes", currentLikes);
				ILifecycleDataItem checkInLikeItem = dynamicModuleManager.Lifecycle.CheckIn(checkOutLikeItem);
				dynamicModuleManager.Lifecycle.Publish(checkInLikeItem);
				dynamicModuleManager.SaveChanges();
			}
			catch (Exception e)
			{
				var msg = e.Message;
			}
			
			
			return currentLikes;
			
		}

		public int AddDislikeForResource(Guid resourceID)
		{

			DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();

			//Add DisLike to resource
			var resource = dynamicModuleManager.GetDataItem(handBookResourcesType, resourceID);
			var resourceDislike = resource.GetRelatedItems("Likes").Cast<DynamicContent>().First();
			var masterResourceDislike = dynamicModuleManager.Lifecycle.GetMaster(resourceDislike);

			var currentDislikes = Convert.ToInt32(resourceDislike.GetValue("AmountOfDislikes")) + 1;
			try
			{
				DynamicContent checkOutDislikeItem = dynamicModuleManager.Lifecycle.CheckOut(masterResourceDislike) as DynamicContent;
				checkOutDislikeItem.SetValue("AmountOfLikes", currentDislikes);
				ILifecycleDataItem checkInDislikeItem = dynamicModuleManager.Lifecycle.CheckIn(checkOutDislikeItem);
				dynamicModuleManager.Lifecycle.Publish(checkInDislikeItem);
				dynamicModuleManager.SaveChanges();
				
			}
			catch (Exception e)
			{
				var msg = e.Message;
			}

			return currentDislikes;
		}
		#endregion Likes

		#region Comments
		public void CreateNewCommentForResource(Guid resourceID, string comment)
		{

			var providerName = String.Empty;
			var transactionName = "commentTransaction";
			DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager(providerName, transactionName);

			//Create like for comment
			Type mylikesmoduleType = TypeResolutionService.ResolveType("Telerik.Sitefinity.DynamicTypes.Model.IAFCHandBookLikes.Iafchandbooklikes");
			DynamicContent mylikesmoduleItem = dynamicModuleManager.CreateDataItem(mylikesmoduleType);

			mylikesmoduleItem.SetValue("Title", comment);
			mylikesmoduleItem.SetValue("AmountOfLikes", 0);
			mylikesmoduleItem.SetValue("AmountOfDislikes", 0);
			mylikesmoduleItem.SetString("UrlName", new Lstring(Regex.Replace("Like-" + comment, UrlNameCharsToReplace, UrlNameReplaceString)));
			mylikesmoduleItem.SetValue("Owner", SecurityManager.GetCurrentUserId());
			mylikesmoduleItem.SetValue("PublicationDate", DateTime.UtcNow);

			dynamicModuleManager.Lifecycle.Publish(mylikesmoduleItem);
			mylikesmoduleItem.SetWorkflowStatus(dynamicModuleManager.Provider.ApplicationName, "Published");

			//Create new Comment
			Type mycommentsType = TypeResolutionService.ResolveType("Telerik.Sitefinity.DynamicTypes.Model.IAFCHandBookComments.Iafchandbookcomments");
			DynamicContent mycommentsItem = dynamicModuleManager.CreateDataItem(mycommentsType);

			mycommentsItem.SetValue("Title", comment);
			mycommentsItem.SetValue("CommentText", comment);
			mycommentsItem.SetString("UrlName", new Lstring(Regex.Replace(comment, UrlNameCharsToReplace, UrlNameReplaceString)));
			mycommentsItem.SetValue("Owner", SecurityManager.GetCurrentUserId());
			mycommentsItem.SetValue("PublicationDate", DateTime.UtcNow);

			mycommentsItem.CreateRelation(mylikesmoduleItem, "CommentLikes");

			ILifecycleDataItem publishedMycommentsItem = dynamicModuleManager.Lifecycle.Publish(mycommentsItem);
			mycommentsItem.SetWorkflowStatus(dynamicModuleManager.Provider.ApplicationName, "Published");

			//Add Comment to resource
			Type myResourcesType = TypeResolutionService.ResolveType("Telerik.Sitefinity.DynamicTypes.Model.IAFCHandBookResourcesData.Iafchandbookresourcesdata");
			var resource = dynamicModuleManager.GetDataItem(myResourcesType, resourceID);
			var masterResource = dynamicModuleManager.Lifecycle.GetMaster(resource);

			DynamicContent checkOutResourceItem = dynamicModuleManager.Lifecycle.CheckOut(masterResource) as DynamicContent;
			checkOutResourceItem.CreateRelation(mycommentsItem, "Comment");
			ILifecycleDataItem checkInMyresourcesItem = dynamicModuleManager.Lifecycle.CheckIn(checkOutResourceItem);
			dynamicModuleManager.Lifecycle.Publish(checkInMyresourcesItem);

			TransactionManager.CommitTransaction(transactionName);
		}
		#endregion Comments
		
	}
}