﻿using System;
using System.Collections.Generic;
using System.Linq;
using MatrixGroup.Sitefinity.Config.AppSettings;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Services.Search;
using Telerik.Sitefinity.Services.Search.Data;
using Telerik.Sitefinity.Web.UI.Components;

namespace SitefinityWebApp.Custom.News
{
	public class KeyIssuesHelper
	{
		//public static IResultSet GetNewsItems(string[] providers, out int hitCount, Guid[] categories = null, string terms = null, int skip = 0, int take = 0)
		public static IResultSet GetNewsItems(out int hitCount, Guid[] categories = null, string terms = null, int skip = 0, int take = 0)
		{
			var searchService = ServiceBus.ResolveService<ISearchService>();

			var searchQuery = BuildQuery(categories);
			var catalogName = AppSettingsUtility.GetValue<string>("KeyIssuesCatalogName");
			var orderBy = new[] { "PublishDate DESC" };
			IResultSet resultSet;

			if (skip == 0 && take == 0)
			{
				resultSet = searchService.Search(catalogName, searchQuery, null, orderBy, new object[0]);
			}
			else
			{
				resultSet = searchService.Search(catalogName, searchQuery, null, skip, take, orderBy, new object[0]);
			}

			hitCount = resultSet.HitCount;

			return resultSet;
		}

		//public static string BuildQuery(Guid[] categories, string[] providers, string terms = null)
		public static string BuildQuery(Guid[] categories, string terms = null)
		{
			var queryGroups = new List<string>();
			/*
			//**** PROVIDERS *****
			if (providers != null && providers.Any())
			{
				var typeFormat = "Provider:{0}";
				var typeQueries = providers.Select(t => string.Format(typeFormat, t));
				var formattedTypes = "(" + string.Join(" OR ", typeQueries) + ")";

				queryGroups.Add(formattedTypes);
			}
			*/

			//**** CATEGORIES ****
			if (categories != null && categories.Any())
			{
				var topicFormat = "CategoryIds:{0}";
				var topicQueries = categories.Select(t => string.Format(topicFormat, t.ToString().Replace("-", "")));
				var formattedTopics = "(" + string.Join(" AND ", topicQueries) + ")";

				queryGroups.Add(formattedTopics);
			}

			if (!string.IsNullOrWhiteSpace(terms))
			{
				//build the format string e.g. (Title:{0} AND Content:{0})
				var searchFields = new[] { "Title", "Content", "Summary" };
				var formats = searchFields.Select(f => f + ":{0}");
				var fullFormat = "(" + string.Join(" ", formats) + ")";

				//add wildcards to the query terms
				var queryTerms = terms;
				var tokenized = queryTerms.Split(' ').Where(t => !string.IsNullOrWhiteSpace(t)).Select(term => term + "*");

				//build the query groups
				var formattedTerms = string.Join(" AND ", tokenized.Select(t => string.Format(fullFormat, t)));

				queryGroups.Add(formattedTerms);
			}

			if (!queryGroups.Any())
			{
				var searchFields = new[] { "Title" };
				var searchTerm = "a* b* c* d* e* f* g* h* i* j* k* l* m* n* o* p* q* r* s* t* u* v* w* x* y* z*";

				//**** TERMS ****
				if (!string.IsNullOrEmpty(searchTerm))
				{
					//build the format string e.g. (Title:{0} AND Content:{0})
					var formats = searchFields.Select(f => f + ":{0}");
					var fullFormat = "(" + string.Join(" ", formats) + ")";

					//add wildcards to the query terms
					var queryTerms = searchTerm;
					var tokenized = queryTerms.Split(' ').Where(t => !string.IsNullOrWhiteSpace(t));

					//build the query groups
					var formattedTerms = string.Join(" OR ", tokenized.Select(t => string.Format(fullFormat, t)));

					queryGroups.Add(formattedTerms);
				}
			}

			return string.Join(" AND ", queryGroups);
		}
	}
}