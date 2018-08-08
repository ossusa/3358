using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Security.Claims;

namespace SitefinityWebApp.Mvc.Models
{
	public class IAFCHandBookCommentModel
	{
		public IAFCHandBookCommentModel(string commentTitle, string сommentText, DateTime dateCreated, IAFCHandBookUserModel author)
		{
			CommentTitle = commentTitle;
			CommentText = сommentText;
			DateCreated = dateCreated;
			Author = author;
			Likes = new IAFCHandBookLikesModel();
			RepliedComments = new List<IAFCHandBookCommentModel>();
			RelpyButtonPressed = false;
			UserAuthorized = IsUserAuthorized();
		}
		public IAFCHandBookCommentModel()
		{
			Likes = new IAFCHandBookLikesModel();
			Author = new IAFCHandBookUserModel();
			RepliedComments = new List<IAFCHandBookCommentModel>();
			RelpyButtonPressed = false;
			UserAuthorized = IsUserAuthorized();
		}

		public Guid Id { get; set; }
		public string CommentTitle { get; set; }
		public string CommentText { get; set; }
		public DateTime DateCreated { get; set; }
		public IAFCHandBookUserModel Author{ get; set; }		
		public IAFCHandBookLikesModel Likes { get; set; }
		public List<IAFCHandBookCommentModel> RepliedComments { get; set; }
		public Boolean RelpyButtonPressed { get; set; }
		public Boolean UserAuthorized { get; set; }


		private Boolean IsUserAuthorized()
		{
			Boolean returnValue = false;
			var identity = ClaimsManager.GetCurrentIdentity();
			var currentUserGuid = identity.UserId;

			if (currentUserGuid != Guid.Empty)
			{
				returnValue = true;
			}
			return returnValue;
		}
	}
}