using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using MatrixGroup.Ams.MatrixMaxx;
using MatrixGroup.Ams.MatrixMaxx.Linq;
using MatrixGroup.Ams.MatrixMaxx.Provider;
using MatrixGroup.Ams.Provider;
using Telerik.Sitefinity.Mvc;
using MatrixGroup.Sitefinity.Utilities;

namespace SitefinityWebApp.Mvc.Controllers
{
	/// <summary>
	/// 
	/// </summary>
	[ControllerToolboxItem(Name = "MaxxPublicationsController", Title = "Maxx Publications", SectionName = "MatrixMaxxToolboxSection")]
	public class MaxxPublicationsController : Controller
	{
		public string MaxxCategoryId { get; set; }

		private int _take = 5;
		public int Take
		{
			get { return _take; }
			set { _take = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public ActionResult Index()
		{
			try
			{
				var filterTaxon = Taxonomy.GetTaxonFromUrl(System.Web.HttpContext.Current);
				if (filterTaxon != null && String.IsNullOrEmpty(MaxxCategoryId))
					MaxxCategoryId = filterTaxon.Description;

				var provider = (MaxxAmsProvider)Ams.Provider;
				var publications = provider.GetProductsByCategoryId(MaxxCategoryId).OrderBy(p => Guid.NewGuid()).Take(Take);

				return View(publications.ToList());

			}
			catch(Exception ex)
			{
				return View("Error", null, ex.Message);
			}
		}

	}
}