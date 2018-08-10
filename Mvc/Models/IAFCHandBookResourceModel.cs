using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Security.Claims;

namespace SitefinityWebApp.Mvc.Models
{
	public class IAFCHandBookResourceModel
	{
		public IAFCHandBookResourceModel()
		{
			IsResourceAddedToMyHandBook = false;
			Comments = new List<IAFCHandBookCommentModel>();
			Likes = new IAFCHandBookLikesModel();
			ResourceDetails = new IAFCHandBookResourceDetailsModel();
			MoreResources = new List<IAFCHandBookMoreResourcesModel>();
			UserAuthorized = IsUserAuthorized();
		}

		public IAFCHandBookResourceModel(Guid id, string resourceTitle, DateTime dateCreated)
		{
			Id = id;
			ResourceTitle = resourceTitle;
			DateCreated = dateCreated;
			IsResourceAddedToMyHandBook = false;
			Comments = new List<IAFCHandBookCommentModel>();
			Likes = new IAFCHandBookLikesModel();
			ResourceDetails = new IAFCHandBookResourceDetailsModel();
			MoreResources = new List<IAFCHandBookMoreResourcesModel>();
			UserAuthorized = IsUserAuthorized();
		}

		public System.Guid Id { get; set; }
		public string ResourceTitle { get; set; }
		public DateTime DateCreated { get; set; }
		public string ResourceUrl { get; set; }
		public int CommentsAmount { get; set; }
		public Boolean IsResourceAddedToMyHandBook { get; set; }
		public Boolean UserAuthorized { get; set; }
		public List<IAFCHandBookCommentModel> Comments { get; set; }
		public IAFCHandBookLikesModel Likes { get; set; }	
		public IAFCHandBookResourceDetailsModel ResourceDetails { get; set; }
		public List<IAFCHandBookMoreResourcesModel> MoreResources {get;set;}

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