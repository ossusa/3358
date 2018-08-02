using System;
using System.Collections.Generic;
using System.Linq;
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
using Telerik.Sitefinity.Utilities;
using Telerik.Sitefinity.Utilities.TypeConverters;
using MatrixGroup.Sitefinity.Utilities;
using Telerik.Sitefinity.Services.Search.Publishing;

namespace SitefinityWebApp.Custom.ContentList
{
	public class ContentListOutboundPipe : SearchIndexOutboundPipe
    {
        public override IEnumerable<Telerik.Sitefinity.Services.Search.Data.IDocument> GetConvertedItemsForMapping(Telerik.Sitefinity.Publishing.WrapperObject wrapperObject)
        {
            wrapperObject.SetOrAddProperty("CategoryIds", string.Empty);

            var contentItem = (((WrapperObject)wrapperObject.WrappedObject).WrappedObject) as IDynamicFieldsContainer ?? ((WrapperObject)((WrapperObject)((WrapperObject)wrapperObject.WrappedObject).WrappedObject).WrappedObject).WrappedObject as IDynamicFieldsContainer;
            if (contentItem != null)
            {
                var categories = contentItem.GetValue<IList<Guid>>("Category");
                if (categories != null)
                {
                    //remove the "-" from the guid since it's difficult to search for a special character
                    wrapperObject.SetOrAddProperty("CategoryIds", string.Join(" ", categories.Select(g => g.ToString().Replace("-", ""))));
                }
            }
            
            //different blogs get different treatment
            wrapperObject.SetOrAddProperty("ThumbnailUrl", string.Empty);
            if (wrapperObject.GetProperty("ContentType").ToString().Contains("BlogPost"))
            {

                    //something used to be here

            }

            //map a few columns for types
            var objectType = ContentListHelper.SearchTypes();
            wrapperObject.SetOrAddProperty("ContentTypeName", string.Empty);
            wrapperObject.SetOrAddProperty("ContentTypeOrdinal", string.Empty);
            if (objectType != null)
            {
                var objectTypeName = objectType.FirstOrDefault(i => i.Item2 == (string)wrapperObject.GetProperty("ContentType"));
                wrapperObject.SetOrAddProperty("ContentTypeName", objectTypeName.Item1);
                wrapperObject.SetOrAddProperty("ContentTypeOrdinal", objectTypeName.Item3.ToString("000"));
            }

            //set the "PublishDate" as a string - lucene will only order by strings
            var publicationDate = (DateTime)wrapperObject.GetProperty("PublicationDate");
            wrapperObject.SetOrAddProperty("PublishDate", publicationDate.ToString("yyyy-MM-dd-HH-mm"));

            //set the "SortTitle" as a uppercase string - sorting by title didn't work very well...
            var sortTitle = wrapperObject.GetProperty("Title").ToString().Trim().ToLower().GenerateSlug().Replace("-", "");
            wrapperObject.SetOrAddProperty("SortTitle", sortTitle);


            wrapperObject.SetOrAddProperty("EventStart", string.Empty);
            wrapperObject.SetOrAddProperty("EventEnd", string.Empty);
            if (contentItem.DoesFieldExist("EventStart"))
            {
                var eventStart = contentItem.GetValue<DateTime?>("EventStart");
                var eventEnd = contentItem.GetValue<DateTime?>("EventEnd");

                if (eventStart.HasValue)
                {
                    wrapperObject.SetProperty("EventStart", eventStart.Value.ToLocalTime().ToString());
                }

                if (eventEnd.HasValue)
                {
                    wrapperObject.SetProperty("EventEnd", eventEnd.Value.ToLocalTime().ToString());
                }
            }

            //index the content of attached files
            wrapperObject.SetOrAddProperty("DocumentText", string.Empty);
            wrapperObject.SetOrAddProperty("DocumentLink", string.Empty);
            if (wrapperObject.HasProperty("Document"))
            {
                var contentLink = wrapperObject.GetProperty("Document") as ContentLink[];
                if (contentLink != null)
                {
                    var librariesManager = LibrariesManager.GetManager();
                    var service = ServiceBus.ResolveService<IDocumentService>();
                    foreach (var link in contentLink)
                    {
                        try
                        {
                            var document = librariesManager.GetDocument(link.ChildItemId);
                            var stream = BlobStorageManager.GetManager(document.GetStorageProviderName()).GetDownloadStream(document);
                            var pdfText = service.ExtractText(document.MimeType, stream);
                            wrapperObject.SetOrAddProperty("DocumentText", pdfText.Trim());
                            wrapperObject.SetOrAddProperty("DocumentLink", document.MediaUrl);
                        }
                        catch (Exception ex)
                        {
                            //log the error, but don't stop the index
                            LogManager.GetCurrentClassLogger().Warn(ex);
                        }
                    }
                }
            }

            if (wrapperObject.HasProperty("Page"))
            {
                //value looks like this in DB: "0619909c-4174-6fcd-88ea-ff000002c0f4;Policy & Initiatives > Policy Topics > Accessibility & Disability"
                var pageValue = wrapperObject.GetProperty("Page");

                if(pageValue != null && pageValue != "")
                {
                    var pageId = Guid.Parse(pageValue.ToString().Split(';').First());
                    var pageManager = PageManager.GetManager();
                    var pageNode = pageManager.GetPageNode(pageId);
                    var pageData = pageManager.GetPageDataList().First(d => d.Id == pageNode.PageId);
                    var contentBlocks = pageData.Controls.Where(c => c.ObjectType == typeof (ContentBlock).FullName);
                    var content = string.Join(" ", contentBlocks.Select(c => ((ContentBlock) pageManager.LoadControl(c)).Html.StripHtmlTags()));

                    wrapperObject.SetOrAddProperty("Content", content);
                    wrapperObject.SetOrAddProperty("Link", pageNode.GetFullUrl());
                    wrapperObject.SetOrAddProperty("Title", pageNode.Title);
                } 
            }

            return base.GetConvertedItemsForMapping(wrapperObject);
        } 
    }



}