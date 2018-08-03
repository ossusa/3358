using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitefinityWebApp.Mvc.Models
{
	public class IAFCHandBookLikesModel
	{
		public Guid Id { get; set; }
		public string LikeTitle { get; set; }
		public int Likes { get; set; }
		public int Dislikes { get; set; }
		public IAFCHandBookLikesModel()
		{

		}
		public IAFCHandBookLikesModel(Guid id, string likeTitle, int likes, int dislikes, DateTime dateCreated)
		{
			Id = id;
			LikeTitle = likeTitle;
			Likes = likes;
			Dislikes = dislikes;			
		}
	}
}