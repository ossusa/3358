#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using MatrixGroup.Data.Extension;
using ServiceStack.Logging;
using Telerik.Sitefinity.Security.Claims;
using Telerik.Sitefinity.Security.Model;
using Telerik.Sitefinity.Web;

#endregion

namespace SitefinityWebApp.App_Master
{
    public partial class Base : MasterPage
    {
        private List<string> _bodyCssClasses = new List<string>();

        public string BodyCssClass
        {
            get { return string.Join(" ", _bodyCssClasses); }
        }

        protected List<PageSiteNode> SiteMapPath
        {
            get
            {
                if (Context.Items["SiteMapPath"] == null)
                {
                    var path = new List<PageSiteNode>();
                    var currentNode = SiteMapBase.GetActualCurrentNode();

                    if (currentNode != null)
                    {
                        do
                        {
                            path.Add(currentNode);
                            currentNode = currentNode.ParentNode as PageSiteNode;
                        } while (currentNode != null && currentNode.Title != "Pages");
                    }

                    Context.Items.Add("SiteMapPath", path);
                }

                return (List<PageSiteNode>) Context.Items["SiteMapPath"];
            }
        }

        private ILog log;
        protected void Page_Load(object sender, EventArgs e)
        {
            log = LogManager.GetLogger(typeof (Base));

            _bodyCssClasses.Add(this.IsDesignMode() ? "backend" : "frontend");
            _bodyCssClasses.AddRange(SiteMapPath.Select(p => "p-" + p.Key));

            iMisFrm.Attributes["class"] = "hidden";
            Guid id =  ClaimsManager.GetCurrentIdentity().UserId;
            if (!id.IsNullOrEmptyGuid())
            {
                //User usr = Telerik.Sitefinity.Security.UserManager.GetManager().GetUser(id);
                iMisFrm.Src = string.Format("https://members.iafc.org/helix/MembershipSignIn/{0}/", ClaimsManager.GetCurrentIdentity().Name);
                //https://members.iafc.org/helix/MembershipSignIn/Timmy1/true    
                //log.InfoFormat("willAct:{0}, name:{1}", string.Format("https://members.iafc.org/helix/MembershipSignIn/{0}/true", ClaimsManager.GetCurrentIdentity().Name), usr.UserName);
            }

        }
    }
}