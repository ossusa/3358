using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SitefinityWebApp.Custom.CustomErrors
{
    public partial class NotAuthorizedStatusCodeSetter : System.Web.UI.UserControl
    {
        protected override void Render(HtmlTextWriter writer)
        {
            if (!this.IsDesignMode())
            {
                base.Render(writer);
                Response.Status = "403 Not Authorized";
                Response.StatusCode = 403;
            }
        }
    }
}