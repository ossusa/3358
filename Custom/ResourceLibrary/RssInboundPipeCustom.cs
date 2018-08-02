using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Xml.Linq;
using Telerik.Sitefinity.Publishing;
using Telerik.Sitefinity.Publishing.Pipes;
using Telerik.Sitefinity.Utilities;

namespace SitefinityWebApp.Custom.ResourceLibrary
{
    public class RssInboundPipeCustom : RSSInboundPipe
    {
        public override WrapperObject ConvertToWraperObject(System.ServiceModel.Syndication.SyndicationItem item)
        {
            var obj = base.ConvertToWraperObject(item);

            if (item.Summary != null)
            {
                obj.SetOrAddProperty(PublishingConstants.FieldSummary, item.Summary.Text.StripHtmlTags());
            }

            var contentText = item.ElementExtensions.Select(extension => extension.GetObject<XElement>())
                                     .Where(e => e.Name.LocalName == "encoded" && e.Name.Namespace.ToString().Contains("content"))
                                     .Select(e => e.Value).FirstOrDefault();

            obj.SetOrAddProperty(PublishingConstants.FieldContent, contentText);

            //vimeo feed contains custom elements for media thumbnail
            var mediaContent = item.ElementExtensions.Select(extension => extension.GetObject<XElement>())
                                .FirstOrDefault(e => e.Name.LocalName == "content");

            if (mediaContent != null)
            {
                var thumbnailElement = mediaContent.Elements().FirstOrDefault(e => e.Name.LocalName == "thumbnail");
                if (thumbnailElement != null)
                {
                    var thumbnailUrl = thumbnailElement.Attributes().First(a => a.Name.LocalName == "url").Value;
                    obj.SetOrAddProperty("ThumbnailUrl", thumbnailUrl);    
                }
            }

            //youtube feed contains custom elements for media description & thumbnail
            var mediaGroup = item.ElementExtensions.Select(extension => extension.GetObject<XElement>())
                                .FirstOrDefault(e => e.Name.LocalName == "group");

            if (mediaGroup != null)
            {
                var thumbnailElement = mediaGroup.Elements().FirstOrDefault(e => e.Name.LocalName == "thumbnail");
                if (thumbnailElement != null)
                {
                    var thumbnailUrl = thumbnailElement.Attributes().First(a => a.Name.LocalName == "url").Value;
                    obj.SetOrAddProperty("ThumbnailUrl", thumbnailUrl);
                }

                var descriptionElement = mediaGroup.Elements().FirstOrDefault(e => e.Name.LocalName == "description");
                if (descriptionElement != null)
                {
                    var content = descriptionElement.Value;
                    obj.SetOrAddProperty(PublishingConstants.FieldContent, content);
                }
            }
            
            

            return obj;
        }
    }
}