using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitefinityWebApp.Mvc.Models
{
	public class IAFCHandBookResourceModel
	{
		public IAFCHandBookResourceModel()
		{
			AddToMyHandBook = false;
			Comments = new List<IAFCHandBookCommentModel>();
			Likes = new IAFCHandBookLikesModel();
			ResourceDetails = new IAFCHandBookResourceDetailsModel();
		}

		public IAFCHandBookResourceModel(Guid id, string resourceTitle, DateTime dateCreated, string url)
		{
			Id = id;
			ResourceTitle = resourceTitle;
			DateCreated = dateCreated;
			ResourceUrl = url;
			AddToMyHandBook = false;
			Comments = new List<IAFCHandBookCommentModel>();
			Likes = new IAFCHandBookLikesModel();
			ResourceDetails = new IAFCHandBookResourceDetailsModel();
		}		

		public System.Guid Id { get; set; }
		public string ResourceTitle { get; set; }		
		public DateTime DateCreated { get; set; }
		public string ResourceUrl { get; set; }		
		public int CommentsAmount { get; set; }
		public Boolean AddToMyHandBook { get; set; }
		public List<IAFCHandBookCommentModel> Comments { get; set; }

		public IAFCHandBookLikesModel Likes { get; set; }

		public IAFCHandBookResourceDetailsModel ResourceDetails { get; set; }

		
	}
}