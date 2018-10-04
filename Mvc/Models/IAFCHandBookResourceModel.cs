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
			IsUserAuthorized = false;			
		}

		public IAFCHandBookResourceModel(Guid id, string resourceTitle, DateTime dateCreated)
		{
			Id = id;
			ResourceTitle = resourceTitle;
			DateCreated = dateCreated;
			IsResourceAddedToMyHandBook = false;
			IsResourceCompleted = false;
			Comments = new List<IAFCHandBookCommentModel>();
			Likes = new IAFCHandBookLikesModel();
			ResourceDetails = new IAFCHandBookResourceDetailsModel();
			MoreResources = new List<IAFCHandBookMoreResourcesModel>();
			IsUserAuthorized = false;			
		}

		public System.Guid Id { get; set; }
		public string ResourceTitle { get; set; }
		public DateTime DateCreated { get; set; }
		public string ResourceUrl { get; set; }
		public string OrderBy { get; set; }
		public int CommentsAmount { get; set; }
		public Boolean IsResourceAddedToMyHandBook { get; set; }
		public Boolean IsResourceCompleted { get; set; }
		public Boolean IsUserAuthorized { get; set; }		
		public List<IAFCHandBookCommentModel> Comments { get; set; }
		public IAFCHandBookLikesModel Likes { get; set; }	
		public IAFCHandBookResourceDetailsModel ResourceDetails { get; set; }
		public List<IAFCHandBookMoreResourcesModel> MoreResources {get;set;}
		public int MoreThen5Resources { get; set; }	
		public string SharedUser { get; set; }
		public Guid UserId { get; set; }
		public Guid SharedUserId { get; set; }
	}
}