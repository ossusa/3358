using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Modules;
using Telerik.Sitefinity.Modules.Libraries;
using Telerik.Sitefinity.Modules.News;
using Telerik.Sitefinity.News.Model;
using Telerik.Sitefinity.Publishing;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Services.Search.Data;
using Telerik.Sitefinity.Services.Search.Publishing;
using Telerik.Sitefinity.Utilities;

namespace SitefinityWebApp.Custom.News
{
	public class BlendedListOutboundPipe : SearchIndexOutboundPipe
	{
		public override IEnumerable<IDocument> GetConvertedItemsForMapping(WrapperObject wrapperObject)
		{
			var contentItem = (((WrapperObject)wrapperObject.WrappedObject).WrappedObject) as IDynamicFieldsContainer ?? ((WrapperObject)((WrapperObject)((WrapperObject)wrapperObject.WrappedObject).WrappedObject).WrappedObject).WrappedObject as IDynamicFieldsContainer;
			var dataItem = (IDataItem)contentItem;

			#region Link
			//set the link from the content location
			wrapperObject.SetOrAddProperty("Link", string.Empty);
			var contentLocation = SystemManager.GetContentLocationService().GetItemDefaultLocation(dataItem);
			var content = contentItem.DoesFieldExist("Content") ? HttpUtility.HtmlDecode(contentItem.GetValue<Lstring>("Content").ToString().StripHtmlTags()) : null;
			var source = contentItem.DoesFieldExist("SourceSite") ? contentItem.GetValue<string>("SourceSite") : null;

			if (string.IsNullOrWhiteSpace(content) && !string.IsNullOrWhiteSpace(source))
			{
				wrapperObject.SetProperty("Link", source);
			}
			else if (contentLocation != null)
			{
				wrapperObject.SetProperty("Link", contentLocation.ItemAbsoluteUrl);
			}
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
			if (contentItem.DoesFieldExist("Category"))
			{
				var categories = contentItem.GetValue<IList<Guid>>("Category");
				if (categories != null)
				{
					//remove the "-" from the guid since it's difficult to search for a special character
					wrapperObject.SetOrAddProperty("CategoryIds", string.Join(" ", categories.Select(g => g.ToString().Replace("-", ""))));
				}
			}
			#endregion

			#region Publication Date
			//set the "PublishDate" as a string - lucene will only order by strings
			var publicationDate = contentItem.GetValue<DateTime>("PublicationDate");
			wrapperObject.SetOrAddProperty("PublishDate", publicationDate.ToString("yyyy-MM-dd-HH-mm"));
			#endregion

			#region Image
			wrapperObject.SetOrAddProperty("ImageId", "");
			if (contentItem.DoesFieldExist("Image"))
			{
				Guid imageId;
				if (Guid.TryParse(contentItem.GetValue<string>("Image"), out imageId))
				{
					var image = LibrariesManager.GetManager().GetImages().FirstOrDefault(i => i.Id == imageId);
					if (image != null)
					{
						wrapperObject.SetOrAddProperty("ImageId", imageId);
					}
				}
			}
			#endregion

			return base.GetConvertedItemsForMapping(wrapperObject);
		}
	}
}