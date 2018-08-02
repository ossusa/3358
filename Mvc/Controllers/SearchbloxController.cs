using SitefinityWebApp.Mvc.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Telerik.Sitefinity.Mvc;

namespace SitefinityWebApp.Mvc.Controllers
{
	[ControllerToolboxItem(Name = "SearchBloxMVC", Title = "SearchBloxMVC", SectionName = "Matrix")]
	public class SearchbloxController : Controller
	{
		public string SearchBloxServerIP { get; set; }
		public string SearchBloxServerPort { get; set; }
		public string CollectionID { get; set; }
		public bool ShowErrors { get; set; }
		public bool ShowNumberOfResults { get; set; }
		public bool ShowSearchBox { get; set; }
		public string SortBy { get; set; }
		public string RemoveFromPageTitle { get; set; }


		public ActionResult Index(Searchblox criteria)
		{
			// widget defaults
			if (this.ShowErrors == null)
				this.ShowErrors = false;
			if (this.ShowNumberOfResults == null)
				this.ShowNumberOfResults = true;
			if (this.ShowSearchBox == null)
				this.ShowSearchBox = true;

			Searchblox results = new Searchblox();
			if (criteria.Page > 1)
				results.Page = criteria.Page;
			results.Query = criteria.Query;

			if (criteria.Query != "")
			{
				results = RunQuery(criteria);
			}

			results.ShowErrors = this.ShowErrors;
			results.ShowNumResults = this.ShowNumberOfResults;
			results.ShowSearchBox = this.ShowSearchBox;
			return View("Index", results);
		}




		private Searchblox RunQuery(Searchblox criteria)
		{
			try
			{
				if (!criteria.Query.IsNullOrEmpty() && !SearchBloxServerIP.IsNullOrEmpty())
				{

					WebClient client = new WebClient();
					client.BaseAddress = Request.Url.ToString();

					// Assemble SearchBlox query string
					string queryParams = "?xsl=xml&page=" + criteria.Page;
					if (!SortBy.IsNullOrEmpty())
					{
						queryParams += "&sort=" + SortBy;
					}
					if (!CollectionID.IsNullOrEmpty())
					{
						var collectionIds = CollectionID.Split(',');
						foreach (var collectionId in collectionIds)
						{
							queryParams += "&col=" + collectionId.Trim();
						}
					}
					if (!String.IsNullOrEmpty(criteria.Query))
					{
						queryParams += "&query=" + criteria.Query;
					}

					Stream data;
					string dataUrl;

					dataUrl = @"http://" + SearchBloxServerIP + (String.IsNullOrEmpty(SearchBloxServerPort) ? "" : ":" + SearchBloxServerPort) + @"/searchblox/servlet/SearchServlet" + queryParams;

					data = client.OpenRead(dataUrl);

					TextReader reader = new StreamReader(data);
					string text = reader.ReadToEnd();

					XDocument xml = XDocument.Parse(text);
					var results = new List<SearchbloxResult>(); // results to pass to view

					// load featured results first
					var xmlFeaturedResults = from xmlFeaturedResult in xml.Descendants("ad") select xmlFeaturedResult;
					foreach (var xmlFeaturedResult in xmlFeaturedResults)
					{
						results.Add(new SearchbloxResult(xmlFeaturedResult, true));
					}

					// load normal search results
					var xmlResults = from xmlResult in xml.Descendants("result") select xmlResult;
					foreach (var xmlResult in xmlResults)
					{
						results.Add(new SearchbloxResult(xmlResult));
					}

					// remove configured string from page title (usually used to remove the site title)
					if (!RemoveFromPageTitle.IsNullOrEmpty())
					{
						foreach (var r in results)
						{
							r.Title = r.Title.Replace(RemoveFromPageTitle, "");
						}
					}
					criteria.Results = results;

					// prep meta data for the results set
					var resultsMetaXML = from xmlMeta in xml.Descendants("results") select xmlMeta;
					var resultsMeta = resultsMetaXML.FirstOrDefault();
					criteria.NumResults = int.Parse(resultsMeta.Attribute("hits").Value);
					criteria.NumPages = int.Parse(resultsMeta.Attribute("lastpage").Value);
					criteria.PageStart = int.Parse(resultsMeta.Attribute("start").Value);
					criteria.PageEnd = int.Parse(resultsMeta.Attribute("end").Value);

					if (criteria.NumPages > 1)
						criteria.PageingUrl = Request.Url.GetLeftPart(UriPartial.Path) + "?query=" + HttpUtility.UrlEncode(criteria.Query) + "&page=";

					//XmlSerializer serializer = new XmlSerializer(typeof(searchdoc));
					//XmlReader xmlReader = XmlReader.Create(new StringReader(text));

					//searchdoc xmlResults = (searchdoc)serializer.Deserialize(xmlReader);

					//searchdocResults resultsObject = (searchdocResults)xmlResults.Items.Where(x => x.GetType() == typeof(searchdocResults)).FirstOrDefault();
					//searchdocAds featuredResultsObject = (searchdocAds)xmlResults.Items.Where(x => x.GetType() == typeof(searchdocAds)).FirstOrDefault();

					//if (featuredResultsObject != null)
					//{
					//	int featuredPosition = 0;

					//	//@TODO test if filtering needed
					//	//foreach (searchdocAdsAD featuredResult in featuredResultsObject.ad.Where(x => x.keywords.ToLower().Contains(resultsObject.query.ToLower().Replace("\"", ""))))
					//	foreach (searchdocAdsAD featuredResult in featuredResultsObject.ad)
					//	{
					//		searchdocResultsResult newResult = new searchdocResultsResult()
					//		{
					//			title = featuredResult.title,
					//			description = featuredResult.description,
					//			url = featuredResult.url
					//		};
					//		resultsObject.result.Insert(featuredPosition, newResult);
					//		featuredPosition++;
					//	}
					//}

					//searchResults.Results = resultsObject.result;


					//int numberOfResults = 0;
					//Int32.TryParse(resultsObject.hits.Replace("\"", ""), out numberOfResults);
					//searchResults.NumResults = numberOfResults;


					//int startResultNumber = (pageNumber - 1) * 10 + 1;  // So we can display result 11 of 26, etc.
					//int endResultNumber = pageNumber * 10;  // So we can display result 11 of 26, etc.

					//int nextPageNumber = -1;
					//int prevPageNumber = -1;
					//if (endResultNumber < numberOfResults)
					//{
					//	nextPageNumber = pageNumber + 1;
					//}
					//if (pageNumber > 1)
					//{
					//	prevPageNumber = pageNumber - 1;
					//}

					//if (numberOfResults > 1)
					//{
					//	searchResults.pageingUrl = Request.Url.GetLeftPart(UriPartial.Path) + "?query=" + query + "&page=";
					//}
					//searchResults.pageNumber = pageNumber;

					reader.Close();

					data.Close();
				}

				return criteria;
			}
			catch (Exception ex)
			{
				criteria.ErrorMessage = ex.Message;
				return criteria;
			}
		}
	}
}