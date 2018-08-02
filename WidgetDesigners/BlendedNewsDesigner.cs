using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Sitefinity.Modules.News;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Web.UI;
using Telerik.Sitefinity.Web.UI.ControlDesign;
using System.Collections.Generic;
using Telerik.Sitefinity.Web.UI.Fields;

namespace SitefinityWebApp.WidgetDesigners
{
	/// <summary>
	/// Represents a designer for the <typeparamref name="SitefinityWebApp.Mvc.Controllers.BlendedNewsListController"/> widget
	/// </summary>
	public class BlendedNewsDesigner : ControlDesignerBase
	{
		#region Properties
		/// <summary>
		/// Obsolete. Use LayoutTemplatePath instead.
		/// </summary>
		protected override string LayoutTemplateName
		{
			get
			{
				return string.Empty;
			}
		}

		/// <summary>
		/// Gets the layout template's relative or virtual path.
		/// </summary>
		public override string LayoutTemplatePath
		{
			get
			{
				if (string.IsNullOrEmpty(base.LayoutTemplatePath))
					return BlendedNewsDesigner.layoutTemplatePath;
				return base.LayoutTemplatePath;
			}
			set
			{
				base.LayoutTemplatePath = value;
			}
		}

		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.Div;
			}
		}
		#endregion

		#region Control references
		/// <summary>
		/// Gets the control that is bound to the ItemsPerPage property
		/// </summary>
		protected virtual Control ItemsPerPage
		{
			get
			{
				return Container.GetControl<Control>("ItemsPerPage", true);
			}
		}

		/// <summary>
		/// Gets the control that is bound to the Limit property
		/// </summary>
		protected virtual Control Limit
		{
			get
			{
				return Container.GetControl<Control>("Limit", true);
			}
		}

        protected virtual HierarchicalTaxonField CategoriesSelector
        {
            get { return Container.GetControl<HierarchicalTaxonField>("CategoriesSelector", true); }
        }

        protected virtual CheckBoxList Providers
        {
            get { return Container.GetControl<CheckBoxList>("Providers", true); }
        }

        protected virtual DropDownList ViewTemplate
        {
            get { return Container.GetControl<DropDownList>("ViewTemplate", true); }
        }

        #endregion

        #region Methods
        protected override void InitializeControls(GenericContainer container)
        {
            CategoriesSelector.TaxonomyId = TaxonomyManager.CategoriesTaxonomyId;
            
            Providers.DataSource = NewsManager.GetManager().Providers;
            Providers.DataTextField = "Title";
            Providers.DataValueField = "Name";
            Providers.DataBind();

            ViewTemplate.DataSource = Directory.GetFiles(HttpContext.Current.Server.MapPath("~/Mvc/Views/BlendedNewsList/"), "*.cshtml").Select(f => f.Split('\\').Last().Split('.').First());
            ViewTemplate.DataBind();

        }

		public Guid[] SelectedCategories
		{
			get
			{
				if (selectedCategories == null) selectedCategories = new Guid[] { };
				return selectedCategories;
			}
			set { selectedCategories = value; }
		}

		private Guid[] selectedCategories;

		public string CategoryValue
		{
			get { return string.Join(",", SelectedCategories); }
			set
			{
				var list = new List<Guid>();
				if (value != null)
				{
					var guids = value.Split(',');
					foreach (var guid in guids)
					{
						Guid newGuid;
						if (Guid.TryParse(guid, out newGuid))
							list.Add(newGuid);
					}
				}
				SelectedCategories = list.ToArray();
			}
		}

		#endregion

    #region IScriptControl implementation
        /// <summary>
        /// Gets a collection of script descriptors that represent ECMAScript (JavaScript) client components.
        /// </summary>
    public override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
    {
      var scriptDescriptors = new List<ScriptDescriptor>(base.GetScriptDescriptors());
      var descriptor = (ScriptControlDescriptor)scriptDescriptors.Last();

      descriptor.AddElementProperty("itemsPerPage", ItemsPerPage.ClientID);
      descriptor.AddElementProperty("limit", Limit.ClientID);
      descriptor.AddElementProperty("viewTemplate", ViewTemplate.ClientID);
      descriptor.AddComponentProperty("CategoriesSelector", CategoriesSelector.ClientID);
			return scriptDescriptors;
		}

		/// <summary>
		/// Gets a collection of ScriptReference objects that define script resources that the control requires.
		/// </summary>
    public override IEnumerable<ScriptReference> GetScriptReferences()
		{
			 var scripts = new List<ScriptReference>(base.GetScriptReferences());
       scripts.Add(new ScriptReference(scriptReference));
			 return scripts;
		}
		#endregion

		#region Private members & constants
		public static readonly string layoutTemplatePath = "~/WidgetDesigners/BlendedNewsDesigner.ascx";
		public const string scriptReference = "~/WidgetDesigners/BlendedNewsDesigner.js";
		#endregion
	}
}
 
