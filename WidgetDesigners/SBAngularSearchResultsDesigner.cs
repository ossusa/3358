using System;
using System.Linq;
using System.Web.UI;
using Telerik.Sitefinity.Web.UI;
using Telerik.Sitefinity.Web.UI.ControlDesign;
using System.Collections.Generic;

namespace SitefinityWebApp.WidgetDesigners
{
    /// <summary>
    /// Represents a designer for the <typeparamref name="SitefinityWebApp.Mvc.Controllers.SBAngularSearchResultsController"/> widget
    /// </summary>
    public class SBAngularSearchResultsDesigner : ControlDesignerBase
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
                    return SBAngularSearchResultsDesigner.layoutTemplatePath;
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
        /// Gets the control that is bound to the CollectionId property
        /// </summary>
        protected virtual Control CollectionId
        {
            get
            {
                return this.Container.GetControl<Control>("CollectionId", true);
            }
        }

        /// <summary>
        /// Gets the control that is bound to the DateRangeName property
        /// </summary>
        protected virtual Control DateRangeName
        {
            get
            {
                return this.Container.GetControl<Control>("DateRangeName", true);
            }
        }

        /// <summary>
        /// Gets the control that is bound to the FacetNames property
        /// </summary>
        protected virtual Control FacetNames
        {
            get
            {
                return this.Container.GetControl<Control>("FacetNames", true);
            }
        }

        /// <summary>
        /// Gets the control that is bound to the SearchBloxServerIp property
        /// </summary>
        protected virtual Control SearchBloxServerIp
        {
            get
            {
                return this.Container.GetControl<Control>("SearchBloxServerIp", true);
            }
        }

        /// <summary>
        /// Gets the control that is bound to the ShowSearchBox property
        /// </summary>
        protected virtual Control ShowSearchBox
        {
            get
            {
                return this.Container.GetControl<Control>("ShowSearchBox", true);
            }
        }

        #endregion

        #region Methods
        protected override void InitializeControls(Telerik.Sitefinity.Web.UI.GenericContainer container)
        {
            // Place your initialization logic here
        }
        #endregion

        #region IScriptControl implementation
        /// <summary>
        /// Gets a collection of script descriptors that represent ECMAScript (JavaScript) client components.
        /// </summary>
        public override System.Collections.Generic.IEnumerable<System.Web.UI.ScriptDescriptor> GetScriptDescriptors()
        {
            var scriptDescriptors = new List<ScriptDescriptor>(base.GetScriptDescriptors());
            var descriptor = (ScriptControlDescriptor)scriptDescriptors.Last();

            descriptor.AddElementProperty("collectionId", this.CollectionId.ClientID);
            descriptor.AddElementProperty("dateRangeName", this.DateRangeName.ClientID);
            descriptor.AddElementProperty("facetNames", this.FacetNames.ClientID);
            descriptor.AddElementProperty("searchBloxServerIp", this.SearchBloxServerIp.ClientID);
            descriptor.AddElementProperty("showSearchBox", this.ShowSearchBox.ClientID);

            return scriptDescriptors;
        }

        /// <summary>
        /// Gets a collection of ScriptReference objects that define script resources that the control requires.
        /// </summary>
        public override System.Collections.Generic.IEnumerable<System.Web.UI.ScriptReference> GetScriptReferences()
        {
            var scripts = new List<ScriptReference>(base.GetScriptReferences());
            scripts.Add(new ScriptReference(SBAngularSearchResultsDesigner.scriptReference));
            return scripts;
        }
        #endregion

        #region Private members & constants
        public static readonly string layoutTemplatePath = "~/WidgetDesigners/SBAngularSearchResultsDesigner.ascx";
        public const string scriptReference = "~/WidgetDesigners/SBAngularSearchResultsDesigner.js";
        #endregion
    }
}
 
