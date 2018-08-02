using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Common.Logging;
using MatrixGroup.Sitefinity.Config.AppSettings;
using Telerik.Sitefinity.Blogs.Model;
using Telerik.Sitefinity.DynamicModules;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Model.ContentLinks;
using Telerik.Sitefinity.Modules.GenericContent.Web.UI;
using Telerik.Sitefinity.Modules.Libraries;
using Telerik.Sitefinity.Modules.Libraries.BlobStorage;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Publishing;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Services.Documents;
using Telerik.Sitefinity.Services.Search.Data;
using Telerik.Sitefinity.Services.Search.Publishing;
using Telerik.Sitefinity.Utilities;
using Telerik.Sitefinity.Utilities.TypeConverters;
using MatrixGroup.Sitefinity.Utilities;
using Telerik.Sitefinity.Libraries.Model;
using Telerik.Sitefinity;
using Telerik.Sitefinity.Data;
using System.Web;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.DynamicModules.Model;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Data.ContentLinks;
using Telerik.Sitefinity.Taxonomies;

namespace SitefinityWebApp.Custom.ResourceLibrary
{
    public class ResourceLibraryOutboundPipe : SearchIndexOutboundPipe
    {
        
        public override IEnumerable<Telerik.Sitefinity.Services.Search.Data.IDocument> GetConvertedItemsForMapping(Telerik.Sitefinity.Publishing.WrapperObject wrapperObject)
        {
            var taxonomyManager = TaxonomyManager.GetManager();
            #region contentItem

            wrapperObject.SetOrAddProperty("DocumentLibrary", string.Empty);
            wrapperObject.SetOrAddProperty("FeaturedRank", "0");

            var contentItem = (((WrapperObject)wrapperObject.WrappedObject).WrappedObject) as IDynamicFieldsContainer ?? ((WrapperObject)((WrapperObject)((WrapperObject)wrapperObject.WrappedObject).WrappedObject).WrappedObject).WrappedObject as IDynamicFieldsContainer;
            //if (contentItem != null)
            //{
            //    var categories = contentItem.GetValue<IList<Guid>>("Category");
            //    if (categories != null)
            //    {
            //        //remove the "-" from the guid since it's difficult to search for a special character
            //        wrapperObject.SetOrAddProperty("CategoryIds", string.Join(" ", categories.Select(g => g.ToString().Replace("-", ""))));
            //    }
            //}
            var dataItem = (IDataItem)contentItem;

            #endregion

            StringBuilder sb = new StringBuilder();

            #region Link
            //set the link from the content location
            wrapperObject.SetOrAddProperty("Link", string.Empty);
            var contentLocation = SystemManager.GetContentLocationService().GetItemDefaultLocation(dataItem);
            var content = contentItem.DoesFieldExist("Content") ? HttpUtility.HtmlDecode(contentItem.GetValue<Lstring>("Content").ToString().StripHtmlTags()) : null;

            if (content != null)
            {
                wrapperObject.SetOrAddProperty("Content", content);
            }
            else
            {
                wrapperObject.SetOrAddProperty("Content", "");
            }

            var source = contentItem.DoesFieldExist("SourceSite") ? contentItem.GetValue<string>("SourceSite") : null;

            if (string.IsNullOrWhiteSpace(content) && !string.IsNullOrWhiteSpace(source))
            {
                wrapperObject.SetProperty("Link", source);
            }
            else if (contentLocation != null)
            {
                wrapperObject.SetProperty("Link", contentLocation.ItemAbsoluteUrl);
            }
            else
            {
                try
                {
                    var clService = SystemManager.GetContentLocationService();

                    //gets the item default location of a given item by itemId provided
                    var location = clService.GetItemDefaultLocation(dataItem);
                    var absoluteUrl = location.ItemAbsoluteUrl;
                    wrapperObject.SetProperty("Link", absoluteUrl);
                }
                catch (Exception)
                {
                    if (wrapperObject.GetProperty("ContentType").ToString().Contains("Dynamic"))
                    {
                        var dynamicContent = contentItem as DynamicContent;
                        wrapperObject.SetProperty("Link", dynamicContent.Urls.FirstOrDefault().Url);
                    }
                    else
                    {
                        wrapperObject.SetProperty("Link", string.Empty);
                    }
                }
            }
            #endregion

            #region Title

            //set the "SortTitle" as a uppercase string - sorting by title didn't work very well...
            var sortTitle = wrapperObject.GetProperty("Title").ToString().Trim().ToLower().GenerateSlug().Replace("-", "");
            //Log.Write(String.Format("pipe-title:{0}, link:{1}", sortTitle, ), ConfigurationPolicy.Debug);
            
            wrapperObject.SetOrAddProperty("SortTitle", sortTitle);

            #endregion

            #region Provider

            //set the name of the provider
            var provider = dataItem.Provider as DataProviderBase;
            wrapperObject.SetOrAddProperty("Provider", string.Empty);
            if (provider != null)
            {
                wrapperObject.SetProperty("Provider", provider.Name);
            }

            #endregion

            #region Categories

            //set the list of category ids
            wrapperObject.SetOrAddProperty("CategoryIds", string.Empty);
            wrapperObject.SetOrAddProperty("CategoryList", string.Empty);
            if (contentItem.DoesFieldExist("Category"))
            {
                var categories = contentItem.GetValue<IList<Guid>>("Category");
                if (categories != null)
                {
                    var categoryList = "";
                    //remove the "-" from the guid since it's difficult to search for a special character
                    var categoryIds = string.Join(" ", categories.Select(g => g.ToString().Replace("-", "")));
                    wrapperObject.SetOrAddProperty("CategoryIds", string.Join(" ", categories.Select(g => g.ToString().Replace("-", ""))));

                    foreach (var item in categories)
                    {
                        var category = taxonomyManager.GetTaxon(item);
                        var categoryName = category.Title.Value;
                        categoryList = categoryList + "," + categoryName;
                    }

                    categoryList = categoryList.TrimStart(',');
                    wrapperObject.SetOrAddProperty("CategoryList", categoryList);
                }
            }

            //set the list of category ids
            wrapperObject.SetOrAddProperty("ResourceTypesIds", string.Empty);
            wrapperObject.SetOrAddProperty("ResourceTypesList", string.Empty);
            if (contentItem.DoesFieldExist("resourcetypes"))
            {
                var categories = contentItem.GetValue<IList<Guid>>("resourcetypes");
                if (categories != null)
                {
                    var categoryList = "";
                    //remove the "-" from the guid since it's difficult to search for a special character
                    wrapperObject.SetOrAddProperty("ResourceTypesIds", string.Join(" ", categories.Select(g => g.ToString().Replace("-", ""))));

                    foreach (var item in categories)
                    {
                        var category = taxonomyManager.GetTaxon(item);
                        var categoryName = category.Title.Value;
                        categoryList = categoryList + "," + categoryName;
                    }

                    categoryList = categoryList.TrimStart(',');
                    wrapperObject.SetOrAddProperty("ResourceTypesList", categoryList);
                }
            }

            //set the list of category ids
            wrapperObject.SetOrAddProperty("OrganizationalAuthorsIds", string.Empty);
            wrapperObject.SetOrAddProperty("OrganizationalAuthorsList", string.Empty);
            if (contentItem.DoesFieldExist("organizationalauthors"))
            {
                var categories = contentItem.GetValue<IList<Guid>>("organizationalauthors");
                if (categories != null)
                {
                    var categoryList = "";
                    //remove the "-" from the guid since it's difficult to search for a special character
                    wrapperObject.SetOrAddProperty("OrganizationalAuthorsIds", string.Join(" ", categories.Select(g => g.ToString().Replace("-", ""))));

                    foreach (var item in categories)
                    {
                        var category = taxonomyManager.GetTaxon(item);
                        var categoryName = category.Title.Value;
                        categoryList = categoryList + "," + categoryName;
                    }

                    categoryList = categoryList.TrimStart(',');
                    wrapperObject.SetOrAddProperty("OrganizationalAuthorsList", categoryList);
                }
            }

            #endregion

            #region Categories

            //set the list of category ids
            wrapperObject.SetOrAddProperty("DateField", string.Empty);
            if (contentItem.DoesFieldExist("Date"))
            {
                try
                {
                    if (contentItem.GetValue<DateTime>("Date") != null)
                    {
                        wrapperObject.SetOrAddProperty("DateField", contentItem.GetValue<DateTime>("Date").ToString());
                    }
                }
                catch (Exception ex)
                {
                }
            }

            #endregion


            #region Publication Date
            //set the "PublishDate" as a string - lucene will only order by strings
            var publicationDate = contentItem.GetValue<DateTime>("PublicationDate");
            wrapperObject.SetOrAddProperty("PublishDate", publicationDate.ToString());
            #endregion

            #region Image field
            wrapperObject.SetOrAddProperty("ImageId", "");
            wrapperObject.SetOrAddProperty("ImageUrl", "");
            if (contentItem.DoesFieldExist("Image"))
            {

                try
                {
                    var iDataItem = contentItem.GetValue<List<IDataItem>>("Image").FirstOrDefault();
                    if (iDataItem != null)
                    {
                        var image = LibrariesManager.GetManager().GetImages().FirstOrDefault(i => i.Id == iDataItem.Id);
                        if (image != null)
                        {
                            wrapperObject.SetOrAddProperty("ImageId", iDataItem.Id);
                            wrapperObject.SetOrAddProperty("ImageUrl", image.Urls.First().Url);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
            #endregion



            #region Blogs and Blog Posts

            //different blogs get different treatment
            wrapperObject.SetOrAddProperty("ThumbnailUrl", string.Empty);
            if (wrapperObject.GetProperty("ContentType").ToString().Contains("BlogPost"))
            {
                var post = (BlogPost)contentItem;
                var parent = post.Parent;
                if (parent.Id != null)
                {
                    wrapperObject.SetOrAddProperty("DocumentLibrary", parent.Title.Value);
                }
                else
                {
                    wrapperObject.SetOrAddProperty("DocumentLibrary", "Blog");
                }

                if (contentItem.DoesFieldExist("ThumbnailUrl"))
                {
                    wrapperObject.SetOrAddProperty("ThumbnailUrl", post.GetValue<string>("ThumbnailUrl"));
                }

                //try
                //{
                //    var featured = post.GetValue<Boolean>("Featured");
                //    if (featured)
                //    {
                //        wrapperObject.SetOrAddProperty("FeaturedRank", "1");
                //    }
                //}
                //catch (Exception)
                //{
                //}

                content = post.Content.ToString().StripHtmlTags();
                wrapperObject.SetOrAddProperty("Content", content);
            }

            #endregion

            #region News

            //different blogs get different treatment
            if (wrapperObject.GetProperty("ContentType").ToString().Contains("News"))
            {
                wrapperObject.SetOrAddProperty("DocumentLibrary", "News");
            }

            #endregion

            #region Event

            wrapperObject.SetOrAddProperty("EventStart", string.Empty);
            wrapperObject.SetOrAddProperty("EventEnd", string.Empty);
            wrapperObject.SetOrAddProperty("LocationStreet", string.Empty);
            wrapperObject.SetOrAddProperty("LocationState", string.Empty);
            wrapperObject.SetOrAddProperty("LocationCity", string.Empty);
            wrapperObject.SetOrAddProperty("Selfpaced", "False");

            if (contentItem.DoesFieldExist("EventStart"))
            {
                wrapperObject.SetOrAddProperty("DocumentLibrary", "Events");

                var eventStart = contentItem.GetValue<DateTime?>("EventStart");
                var eventEnd = contentItem.GetValue<DateTime?>("EventEnd");
                var street = contentItem.DoesFieldExist("Street") ? !contentItem.GetValue<Lstring>("Street").Value.IsNullOrEmpty() ? contentItem.GetValue<Lstring>("Street").Value : "" : "";
                var state = contentItem.DoesFieldExist("State") ? !contentItem.GetValue<Lstring>("State").Value.IsNullOrEmpty() ? contentItem.GetValue<Lstring>("State").Value : "" : "";
                var city = contentItem.DoesFieldExist("City") ? !contentItem.GetValue<Lstring>("City").Value.IsNullOrEmpty() ? contentItem.GetValue<Lstring>("City").Value : "" : "";
                var selfPaced = contentItem.DoesFieldExist("Selfpaced") ? contentItem.GetValue<bool>("Selfpaced") ? "True" : "False" : "False";

                wrapperObject.SetOrAddProperty("Selfpaced", selfPaced);

                if (eventStart.HasValue)
                {
                    wrapperObject.SetProperty("EventStart", eventStart.Value.ToLocalTime().ToString());
                    wrapperObject.SetProperty("PublicationDate", eventStart.Value.ToLocalTime().ToString());
                }

                if (eventEnd.HasValue)
                    wrapperObject.SetProperty("EventEnd", eventEnd.Value.ToLocalTime().ToString());

                if (!street.IsNullOrEmpty())
                    wrapperObject.SetOrAddProperty("LocationStreet", street);

                if (!state.IsNullOrEmpty())
                    wrapperObject.SetOrAddProperty("LocationState", state);

                if (!city.IsNullOrEmpty())
                    wrapperObject.SetOrAddProperty("LocationCity", city);
            }

            #endregion

            #region Documents

            //index the content of attached files
            wrapperObject.SetOrAddProperty("DocumentText", string.Empty);
            wrapperObject.SetOrAddProperty("DocumentLink", string.Empty);
            if (wrapperObject.GetProperty("ContentType").ToString().Contains("Document"))
            {
                var librariesManager = LibrariesManager.GetManager();
                var service = ServiceBus.ResolveService<IDocumentService>();
                var document = librariesManager.GetDocument(Guid.Parse(wrapperObject.GetProperty("Id").ToString()));
                var documentLibrary = document.Library;
                wrapperObject.SetOrAddProperty("DocumentLink", document.MediaUrl);
                wrapperObject.SetOrAddProperty("DocumentLibrary", documentLibrary.Title.Value);

                try
                {
                    var stream = BlobStorageManager.GetManager(document.GetStorageProviderName()).GetDownloadStream(document);
                    var pdfText = service.ExtractText(document.MimeType, stream);

                    wrapperObject.SetOrAddProperty("DocumentText", pdfText.Trim());
                }
                catch (Exception ex)
                {
                    //log the error, but don't stop the index
                    LogManager.GetCurrentClassLogger().Warn(ex);
                }
            }

            #endregion

            #region DynamicContent

            wrapperObject.SetOrAddProperty("DisplayDate", string.Empty);
            //different blogs get different treatment
            if (wrapperObject.GetProperty("ContentType").ToString().Contains("Dynamic"))
            {
                var dynamicType = wrapperObject.GetProperty("ContentType").ToString().Split('.').Last();
                wrapperObject.SetOrAddProperty("DocumentLibrary", dynamicType);
            }

            if (wrapperObject.GetProperty("ContentType").ToString().Contains("PressRelease"))
            {
                var dynamicContent = contentItem as DynamicContent;
                content = dynamicContent.GetValue<Lstring>("Body").ToString().StripHtmlTags();
                wrapperObject.SetOrAddProperty("Content", content);

                //var featured = dynamicContent.GetValue<Boolean>("Featured");
                //if (featured)
                //{
                //    wrapperObject.SetOrAddProperty("FeaturedRank", "1");
                //}

                content = dynamicContent.GetValue<Lstring>("Body").ToString().StripHtmlTags();
            }

            if (wrapperObject.GetProperty("ContentType").ToString().Contains("OnSceneArticles.OnSceneArticle"))
            {
                var dynamicContent = contentItem as DynamicContent;
                content = dynamicContent.GetValue<Lstring>("Body").ToString().StripHtmlTags();
                wrapperObject.SetOrAddProperty("Content", content);

                //var featured = dynamicContent.GetValue<Boolean>("Featured");
                //if (featured)
                //{
                //    wrapperObject.SetOrAddProperty("FeaturedRank", "1");
                //}
                var displayDate = dynamicContent.GetValue<Lstring>("DisplayDate").Value;
                if (!displayDate.IsNullOrWhitespace())
                {
                    wrapperObject.SetOrAddProperty("DisplayDate", displayDate);
                }
            }

            if (wrapperObject.GetProperty("ContentType").ToString().Contains(".Resources.Resource"))
            {
                var dynamicContent = contentItem as DynamicContent;
                content = dynamicContent.GetValue<Lstring>("Article").ToString().StripHtmlTags();
                wrapperObject.SetOrAddProperty("Content", content);

                //var featured = dynamicContent.GetValue<Boolean>("Featured");
                //if (featured)
                //{
                //    wrapperObject.SetOrAddProperty("FeaturedRank", "1");
                //}

                content = dynamicContent.GetValue<Lstring>("Article").ToString().StripHtmlTags();
            }

            if (wrapperObject.GetProperty("ContentType").ToString().Contains(".LegacyIndustryArticles.LegacyIndustryArticle"))
            {
                var dynamicContent = contentItem as DynamicContent;
                content = dynamicContent.GetValue<Lstring>("Body").ToString().StripHtmlTags();
                wrapperObject.SetOrAddProperty("Content", content);

                //var featured = dynamicContent.GetValue<Boolean>("Featured");
                //if (featured)
                //{
                //    wrapperObject.SetOrAddProperty("FeaturedRank", "1");
                //}
            }

            if (wrapperObject.GetProperty("ContentType").ToString().Contains(".DossierArticles.DossierArticle"))
            {
                var dynamicContent = contentItem as DynamicContent;
                content = dynamicContent.GetValue<Lstring>("Body").ToString().StripHtmlTags();
                wrapperObject.SetOrAddProperty("Content", content);

                //var featured = dynamicContent.GetValue<Boolean>("Featured");
                //if (featured)
                //{
                //    wrapperObject.SetOrAddProperty("FeaturedRank", "1");
                //}
            }

            #endregion

            #region Resources

            //different blogs get different treatment
            if (wrapperObject.GetProperty("ContentType").ToString().Contains("Resources.Resource"))
            {
                try
                {
                    var iDataItem = contentItem.GetValue<List<IDataItem>>("featuredimage").FirstOrDefault();
                    if (iDataItem != null)
                    {
                        var image = LibrariesManager.GetManager().GetImages().FirstOrDefault(i => i.Id == iDataItem.Id);
                        if (image != null)
                        {
                            wrapperObject.SetOrAddProperty("ImageId", iDataItem.Id);
                            wrapperObject.SetOrAddProperty("ImageUrl", image.Urls.First().Url);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }

            #endregion



            #region Pages

            if (wrapperObject.HasProperty("Page"))
            {
                wrapperObject.SetOrAddProperty("DocumentLibrary", "Page");

                //value looks like this in DB: "0619909c-4174-6fcd-88ea-ff000002c0f4;Policy & Initiatives > Policy Topics > Accessibility & Disability"
                var pageValue = wrapperObject.GetProperty("Page");

                if (pageValue != null)
                {
                    var pageId = Guid.Parse(pageValue.ToString().Split(';').First());
                    var pageManager = PageManager.GetManager();
                    var pageNode = pageManager.GetPageNode(pageId);
                    var pageData = pageManager.GetPageDataList().First(d => d.Id == pageNode.PageId);
                    var contentBlocks = pageData.Controls.Where(c => c.ObjectType == typeof(ContentBlock).FullName);
                    var contentText = string.Join(" ", contentBlocks.Select(c => ((ContentBlock)pageManager.LoadControl(c)).Html.StripHtmlTags()));

                    wrapperObject.SetOrAddProperty("Content", contentText);
                    wrapperObject.SetOrAddProperty("Link", pageNode.GetFullUrl());
                    wrapperObject.SetOrAddProperty("Title", pageNode.Title);
                }
            }

            #endregion

            #region Infographics

            if (wrapperObject.GetProperty("ContentType").ToString().Contains("Infographics"))
            {
                var dynamicManager = DynamicModuleManager.GetManager();
                var infographicType = TypeResolutionService.ResolveType("Telerik.Sitefinity.DynamicTypes.Model.Infographics.Infographics");
                var id = Guid.Parse(wrapperObject.GetProperty("Id").ToString());
                var item = dynamicManager.GetDataItem(infographicType, id);
                var contentLink = item.GetValue<ContentLink[]>("Image");
                if (contentLink != null && contentLink.Any())
                {
                    var librariesManager = LibrariesManager.GetManager();
                    var image = librariesManager.GetImage(contentLink.First().ChildItemId);
                    wrapperObject.SetOrAddProperty("ThumbnailUrl", image.MediaUrl);
                }
            }

            #endregion


            #region Resource Library Information

            //map a few columns for types
            var objectType = ResourceLibraryHelper.SearchTypes.FirstOrDefault(i => i.Item2 == (string)wrapperObject.GetProperty("DocumentLibrary"));
            wrapperObject.SetOrAddProperty("ContentTypeName", string.Empty);
            wrapperObject.SetOrAddProperty("ContentTypeOrdinal", string.Empty);
            if (objectType != null)
            {
                wrapperObject.SetOrAddProperty("ContentTypeName", objectType.Item1);
                wrapperObject.SetOrAddProperty("ContentTypeOrdinal", objectType.Item3.ToString("000"));
            }
            else
            {
                wrapperObject.SetOrAddProperty("ContentTypeName", "Unsorted");
                wrapperObject.SetOrAddProperty("ContentTypeOrdinal", "100");
            }

            #endregion

            return base.GetConvertedItemsForMapping(wrapperObject);
        }
    }
}