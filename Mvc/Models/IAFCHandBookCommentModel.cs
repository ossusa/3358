using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitefinityWebApp.Mvc.Models
{
	public class IAFCHandBookCommentModel
	{
		public Guid id { get; set; }
		public string CommentTitle { get; set; }
		public string CommentText { get; set; }
		public DateTime DateCreated { get; set; }
		public string Author { get; set; }
		
		public List<IAFCHandBookLikesModel> Likes { get; set; }



		public IAFCHandBookCommentModel(string commentTitle, string сommentText, DateTime dateCreated, string author)
		{
			CommentTitle = commentTitle;
			CommentText = сommentText;
			DateCreated = dateCreated;
			Author = author;
			Likes = new List<IAFCHandBookLikesModel>();
		}
		public IAFCHandBookCommentModel()
		{
			Likes = new List<IAFCHandBookLikesModel>();
		}
	}
}