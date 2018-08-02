using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace SitefinityWebApp.Mvc.Models
{
	public class SearchbloxResult
	{
		public int Score { get; set; }
		public string Uid { get; set; }
		public int CollectionId { get; set; }
		public string CollectionName { get; set; }
		public string IndexDate { get; set; }
		public string LastModified { get; set; }
		public string ContentType { get; set; }
		public string Url { get; set; }
		public int Size { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string Context { get; set; }
		public DateTime? Published { get; set; }


		public SearchbloxResult() {}

		public SearchbloxResult(XElement x)
		{
			var score = x.Descendants("score").FirstOrDefault();
			if (score != null)
				this.Score = int.Parse(score.Value);

			this.Url = x.Descendants("url").FirstOrDefault().Value;
			this.Title = x.Descendants("title").FirstOrDefault().Value;

			var context = x.Descendants("context").FirstOrDefault();
			if (context != null)
				this.Context = context.ToString().Replace("<context>","").Replace("</context>","");

			var description = x.Descendants("description").FirstOrDefault();
			if (description != null)
				this.Description = description.ToString().Replace("<description>", "").Replace("</description>", "");

			var pubDateXDoc = x.Descendants("published").FirstOrDefault();
			if (pubDateXDoc != null)
			{
				string pubDate = pubDateXDoc.Value;
				pubDate = pubDate.Split(' ').FirstOrDefault();
				pubDate = pubDate.Insert(6, "-").Insert(4, "-") + "-04:00";
				this.Published = DateTime.Parse(pubDate);
			}
			else
			{
				this.Published = null;
			}
		}


		// Featured result constructor, al values are attributes rather than child nodes, and it does not provide context
		public SearchbloxResult(XElement x, bool isFeaturedResult)
		{
			if (!isFeaturedResult)
				return;

			this.Url = x.Attribute("url").Value;
			this.Title = x.Attribute("title").Value;


			var description = x.Attribute("description");
			if (description != null)
				this.Description = description.Value;

			this.Published = null;

		}
	}


}