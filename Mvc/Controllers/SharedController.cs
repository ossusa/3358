using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MatrixGroup.Data.Extension;
using MatrixGroup.Data.Interfaces;
using MatrixGroup.Data.Model;
using MatrixGroup.Implementation.Helix.Model;
using MatrixGroup.Implementation.Helix.Security;
using MatrixGroup.Implementation.Security;
using MatrixGroup.Implementation.Services;
using Mx.IAFC.Webapp.Data;
using Mx.IAFC.Webapp.Data.Command;
using ServiceStack;
using ServiceStack.Caching;
using ServiceStack.Logging;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Claims;
using Telerik.Sitefinity.Security.Model;

namespace SitefinityWebApp.Mvc.Controllers
{
    public class SharedController : Controller
    {
        protected ILog log = LogManager.GetLogger(typeof (SharedController));
        protected ICacheClient cacheClient = MxAppHost.Instance.Container.Resolve<ICacheClient>();
        private SiteVisitorSessionCmd _sessionCmd;

        public SharedController()
        {
            //VisitUserModel vUser = this.GetSetAppSiteUser();
            _sessionCmd = new SiteVisitorSessionCmd();
        }
        
        /// <summary>
        /// A convenience to get connected to app
        /// </summary>
        /// <returns></returns>
        protected ISiteAppRunner SiteAppRunner()
        {
            return MxAppHost.Instance.Container.Resolve<ISiteAppRunner>();
        }
        
        public Guid MyId()
        {
            return ClaimsManager.GetCurrentIdentity().UserId;
        }

        public Telerik.Sitefinity.Security.Model.User SfUser()
        {
            return this.MyId().IsNullOrEmptyGuid()? null : Telerik.Sitefinity.Security.UserManager.GetManager().GetUser(this.MyId());
        }

        public string SessionId()
        {
            //var susr = this.SiteAppRunner().SiteUser.Instance() as VisitUserModel ?? new VisitUserModel() {ClientId = String.Empty };
            //return ControllerContext.HttpContext.Request.AnonymousID;
           /* var context = System.Web.HttpContext.Current;
            if (context == null)
            {
                return "name is null";
            }
            var ck = context.Request.Cookies["anouser"];
            
            return "name: {0}, user:{1}".Fmt(ck?.Value, ck != null ? ck["name"]:"");*/
            //return AppSessionServices.GetCkSessionId();
            return System.Web.HttpContext.Current?.GetCkSessionId()??"";
        }

        protected void SingOutCleanUp()
        {
            SiteVisitorSessionCmd sitecmd = new SiteVisitorSessionCmd();
            sitecmd.DeleteBySessionId(this.SessionId());
            //this.GetSetAppSiteUser(this.MyId(), new VisitUserModel());
            sitecmd.DeleteVisitorByUserId(this.MyId());
        }

        /// <summary>
        /// Can be null if container doesnot have visitUser
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected void SetAppSiteUser(Guid userId, GenericResultSerialized model)
        {
            /*SiteVisitorSessionCmd sitecmd = new SiteVisitorSessionCmd();
            VisitUserModel i_user = this.SiteAppRunner().SiteUser.Instance() as VisitUserModel;
            VisitUserModel c_user = FluentSiteUser<VisitUserModel>
                .InitFromCacheOrDefault(i_user, this.SessionId())
                .Instance();
            string _sessionId = this.SessionId();*/

            if ((!userId.IsNullOrEmptyGuid() || !userId.IsOneGuid()) && model != null)
            {
                /*_sessionCmd.UpdateVisitorByUserId(userId, new GenericResultSerialized()
                {
                    User = model.SerializeToString(),
                    SessionId = _sessionId,
                    UserId = model.UserId,
                    Message = "set app site user",
                    CreationDate = DateTime.UtcNow,
                    LastChangeDate = DateTime.UtcNow,
                    ContactId = model.ClientToken?.ClientId
                });*/
                _sessionCmd.UpdateVisitorByUserId(userId, model);
            }
            
            

            /*if (model == null)
            {
                if (userId.IsNullOrEmptyGuid())
                {
                    //var _usr = sitecmd.GetVisitorBySessionId(_sessionId);
                    var _usr = sitecmd.GetVisitorByUserId(userId);
                    if (_usr != null)
                    {
                        c_user = _usr.User.FromJson<VisitUserModel>();
                        return c_user;
                    }
                }
                return c_user;
            }
            // cache will only be updated if session is the same as cookie
            if (_sessionId == model.SessionId && (!model.UserId.IsNullOrEmptyGuid() || !model.UserId.IsOneGuid() ))
            {
                // update to table
                //sitecmd.UpdateBySessionId(_sessionId, new GenericResultSerialized()
                sitecmd.UpdateVisitorByUserId(model.UserId, new GenericResultSerialized()
                {
                    User = model.SerializeToString(),
                    SessionId = _sessionId,
                    UserId = model.UserId,
                    Message = "init from Auth",
                    CreationDate = DateTime.UtcNow,
                    LastChangeDate = DateTime.UtcNow

                } );

                c_user = model;
            } */
            
        }
       
        #region OpenAccess

        public JsonResult GetIdentity()
        {
            return Json(new {data = MyId()}, JsonRequestBehavior.AllowGet);
        }
        #endregion
        protected void CreateOrUpdateCookie(string ckName, string value)
        {
            HttpCookie ck = Request.Cookies[ckName] ?? new HttpCookie(ckName, value);

            ck.Domain = ConfigurationManager.AppSettings["ckDomain"];
            ck.Expires = DateTime.UtcNow.AddMinutes(60);
            ck.Value = value;
            //Response.AppendCookie(ck);
            //ControllerContext.HttpContext.Response.AppendCookie(ck);
            HttpContext.Response.SetCookie(ck);
        }

        protected string GetCookieValue(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                return null;
            }
            return Request.Cookies[name]?.Value;
            
        }
        protected string GetAppCookie()
        {
            try
            {
                var ck = ControllerContext.HttpContext.Request.Cookies;
                if (ck != null && ck.AllKeys.Contains("hssession"))
                {
                    return ck.Get("hlsession").Value;
                }
            }
            catch (Exception)
            {
                
            }
            
            return string.Empty;
        }

        protected void ClearCookie(string ckName)
        {
            HttpCookie cookie = Request.Cookies[ckName];
            if (cookie != null)
            {
                cookie.Domain = ConfigurationManager.AppSettings["ckDomain"];
                cookie.Values[ckName] = "";
                cookie.Value = "";
                cookie.Expires = DateTime.UtcNow.AddMinutes(-1);
                Response.AppendCookie(cookie);
                //ControllerContext.HttpContext.Response.AppendCookie(cookie);
                HttpContext.Response.SetCookie(cookie);
            }
        }
    }
}