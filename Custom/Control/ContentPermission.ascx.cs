using System;
using System.Net.Http;
using MatrixGroup.Data.Extension;
using ServiceStack;
using ServiceStack.Logging;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.DynamicModules;
using Telerik.Sitefinity.DynamicModules.Model;
using Telerik.Sitefinity.DynamicModules.Web.UI.Frontend;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Claims;
using Telerik.Sitefinity.Security.Configuration;
using Telerik.Sitefinity.Utilities.TypeConverters;
using Telerik.Web.UI;

namespace SitefinityWebApp.Custom.Control
{
    public partial class ContentPermission : System.Web.UI.UserControl
    {
        private ILog log = LogManager.GetLogger(typeof (ContentPermission));
        protected override void OnInit(EventArgs e)
        {
            var container = (DynamicDetailContainer)this.FindControl("detailContainer");
            ValidatePermission(container);
            base.OnInit(e);
            
        }

        private void ValidatePermission(DynamicDetailContainer container)
        {   
            DynamicContent[] detailItems = (DynamicContent[])container.DataSource;
            DynamicContent item = detailItems[0];
            var identity = ClaimsManager.GetCurrentIdentity();
            var url = Request.Url.OriginalString;
            var loginUrl = string.Format("~/Mxg/AuthService/SignInByHelix?ReturnUrl={0}", url.UrlDecode());
            try
            {
                /*var manager = DynamicModuleManager.GetManager();
                Type contentType = TypeResolutionService
                    .ResolveType("Telerik.Sitefinity.DynamicTypes.Model.ResourcesProtected.ProtectedResource");*/
                

                if (item != null)
                {
                    //var det = item?.Issec.DataItem as DynamicContent;
                    //var pressitem = manager.GetDataItem(contentType, new Guid(item.GetValue("Id").ToString()));
                    //bool isSecgrand = pressitem.IsSecurityActionTypeGranted(SecurityActionTypes.View);
                    
                    

                    bool isSecgrand = item.IsSecurityActionTypeGranted(SecurityActionTypes.View);


                    log.InfoFormat("title is {0}-:{1}, permission:{2}, userid:{3}, isNullGuid:{4}",
                        item.GetValue("Title"),
                        item.GetValue("Id"),
                        isSecgrand,
                        identity.UserId,
                        identity.UserId.IsNullOrEmptyGuid()

                        );

                    if (isSecgrand == false)
                    {
                        Response.Redirect(identity.UserId.IsNullOrEmptyGuid() ? loginUrl : "~/account/not-authorized");
                    }
                }
                // not login & not granded
                

            }
            catch (Exception ex)
            {
                log.InfoFormat("exception from get dynamic:{0}- inner:{1}", ex.Message, ex.InnerException?.Message);
                //Response.Redirect("~/account/not-authorized");
                Response.Redirect(identity.UserId.IsNullOrEmptyGuid() ? loginUrl : "~/account/not-authorized");
            }
        }

        protected void detailContainer_DataBound(object sender, EventArgs e)
        {
            var container = (DynamicDetailContainer)this.FindControl("detailContainer");
            var item = (DetailItem)container.Controls[0];
            try
            {
                var manager = DynamicModuleManager.GetManager();
                Type contentType = TypeResolutionService
                    .ResolveType("Telerik.Sitefinity.DynamicTypes.Model.ResourcesProtected.ProtectedResource");

                var det = item?.DataItem as DynamicContent;
                var pressitem = manager.GetDataItem(contentType, new Guid(det.GetValue("Id").ToString()));

                var identity = ClaimsManager.GetCurrentIdentity();
                bool isSecgrand = pressitem.IsSecurityActionTypeGranted(SecurityActionTypes.View);

                if (det != null)
                {
                    log.InfoFormat("title is {0}-:{1}, permission:{2}, userid:{3}, isNullGuid:{4}",
                        det.GetValue("Title"),
                        det.GetValue("Id"),
                        isSecgrand,
                        identity.UserId,
                        identity.UserId.IsNullOrEmptyGuid()

                        );
                }
                // not login & not granded
                if (isSecgrand == false)
                {
                    Response.Redirect(identity.UserId.IsNullOrEmptyGuid()? "~/Mxg/AuthService/SignInByHelix/" : "~/account/not-authorized");
                }
                
            }
            catch (Exception ex)
            {
                log.InfoFormat("exception from get dynamic:{0}- inner:{1}", ex.Message, ex.InnerException?.Message);
                Response.Redirect("~/account/not-authorized");
            }

        }

        void repeater_ItemDataBound(object sender, RadListViewItemEventArgs e)
        {
            log.InfoFormat("typeofItem:{0}", e.Item?.GetType()?.FullName);
            if (e.Item is RadListViewDataItem)
            {
                var args = (RadListViewDataItem)e.Item;
                /*var blogPost = (BlogPost)args.DataItem;
                var imagePath = DataExtensions.GetValue<String>(blogPost, "Image");
                var imageCtl = (Image)e.Item.FindControl("imgBlogImage");

                if (!string.IsNullOrEmpty(imagePath) && imageCtl != null)
                {
                    imageCtl.ImageUrl = imagePath;
                }
                else if (imageCtl != null)
                {
                    imageCtl.Visible = false;
                }*/
            }
        }
    }
}