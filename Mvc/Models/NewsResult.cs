using System;
using System.Linq;
using Telerik.Sitefinity.Libraries.Model;
using Telerik.Sitefinity.Modules.Libraries;

namespace SitefinityWebApp.Mvc.Models
{
	public class NewsResult
	{

		public string Title { get; set; }
        public string DisplayDate { get; set; }
        public string Summary { get; set; }
		public string ImageId { get; set; }
		public string ImageUrl
		{
			get { return Image != null ? Image.MediaUrl : null; }
		}
		public string ImageCaption {get; set; }

	    public string Content { get; set; }
		public string Link { get; set; }
		public DateTime PublicationDate { get; set; }
        public DateTime DateField { get; set; }

        public bool Featured { get; set; }

		//Lazy loading image
		private bool checkImage;
		private Image _image;
		public Image Image
		{
			get
			{
				if (!checkImage)
				{
					checkImage = true;
					if (!string.IsNullOrEmpty(ImageId))
					{
						Guid imageId;
						if (Guid.TryParse(ImageId, out imageId))
						{
							_image = LibrariesManager.GetManager().GetImages().FirstOrDefault(i => i.Id == imageId);
						}
					}
				}

				return _image;
			}
			set { _image = value; }
		}
	}
}