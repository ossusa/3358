using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using MatrixGroup.Data.Extension;
using MatrixGroup.Data.Interfaces;
using MatrixGroup.Data.Model;
using MatrixGroup.Implementation.Security;
using MatrixGroup.Implementation.SFEvents;
using NLog.Config;
using ServiceStack;
using ServiceStack.Logging;
using ServiceStack.Logging.NLogger;
using SitefinityWebApp.Custom.ResourceLibrary;
using Telerik.Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Data.Events;
using Telerik.Sitefinity.Modules.Forms.Events;
using Telerik.Sitefinity.Publishing;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Services.Search.Publishing;
using Telerik.Sitefinity.Web.Events;

namespace SitefinityWebApp.App_Start
{
    public class SfBootstraper
    {
        private ILog log;
        public SfBootstraper()
        {
            ServiceStack.Logging.LogManager.LogFactory = new NLogFactory();
#if DEBUG
            NLog.LogManager.Configuration = new XmlLoggingConfiguration("~/NLog.config".MapHostAbsolutePath());
#else
            NLog.LogManager.Configuration = new XmlLoggingConfiguration("~/NLog.Release.config".MapHostAbsolutePath());
#endif


            log = LogManager.GetLogger(typeof (SfBootstraper));
        }
        public void Setup()
        {
            Telerik.Sitefinity.Abstractions.Bootstrapper.Initialized += Bootstrapper_Initialized;
            (new ServiceBootstrap()).Init();
            

        }

        
        public string GetLoginPage
        {
            get { return "~/Mxg/AuthService/SignInByHelix"; }
        }
        private void Bootstrapper_Initialized(object sender, Telerik.Sitefinity.Data.ExecutedEventArgs e)
        {


            //Telerik.Microsoft.Practices.EnterpriseLibrary.Caching.ICacheManager cazy = SystemManager.GetCacheManager(CacheManagerInstance.Configuration);
            

            if (e.CommandName == "Bootstrapped")
            {
                PublishingSystemFactory.UnregisterPipe(SearchIndexOutboundPipe.PipeName);
                PublishingSystemFactory.RegisterPipe(SearchIndexOutboundPipe.PipeName, typeof(ResourceLibraryOutboundPipe));
                //use UnauthorizedPageAccessEvent
                EventHub.Subscribe<IUnauthorizedPageAccessEvent>(new Telerik.Sitefinity.Services.Events.SitefinityEventHandler<IUnauthorizedPageAccessEvent>(OnUnauthorizedAccess));
                /*
                EventHub.Subscribe<IFormFieldValidatingEvent>(FormFileUploadFieldValidation);*/
                EventHub.Subscribe<IContextOperationEvent>(ContextOperationEvent);

            }

            if (e.CommandName == "RegisterRoutes")
            {
                RegisterRoutes(RouteTable.Routes);
                //ReplaceDefaultRoute(e.Data as IQueryable<RouteBase>);
                //SystemManager.RegisterServiceStackPlugin(new );
                //SystemManager.RegisterServiceStackPlugin(new MatrixGroup.Implementation.Services.Plugins.ServiceFeaturePlugin());

            }
        }
        
        private void OnUnauthorizedAccess(IUnauthorizedPageAccessEvent unauthorizedEvent)
        {
            // RWB 20171003 - var url = unauthorizedEvent.Page.Url.TrimStart('~');
			HttpRequest req = HttpContext.Current.Request;
            var url = req.AppRelativeCurrentExecutionFilePath.TrimStart('~');
      
            var currentContext = unauthorizedEvent.HttpContext;
            var page = unauthorizedEvent.Page;
            var redirectStrategyType = unauthorizedEvent.RedirectStrategy;
            var redirectUrl = unauthorizedEvent.RedirectUrl;
            log.Debug("Page-auth request with page title of:{0}".Fmt(unauthorizedEvent.Page.Title));
            unauthorizedEvent.HttpContext.Response.Redirect("{0}/?ReturnUrl={1}".Fmt(GetLoginPage, url));

        }
        private void ContextOperationEvent(IContextOperationEvent evt)
        {

            // 403 evnt
            var optContext = new RequestContextEndEvent(HttpContext.Current);
            var reqUrl = optContext.OperationKey;
            NameValueCollection qryDic = null;
            var currentId = Telerik.Sitefinity.Security.Claims.ClaimsManager.GetCurrentIdentity();
#if DEBUG

            log.DebugFormat("request for:{0}, status:{1}", reqUrl, optContext.Status );
#endif
            //log.Debug("status code from opt event:{0}".Fmt());
            //var optEvent = new MatrixGroup.Framework.SFEvents.RequestContextEndEvent().PopulateWith(evt);
            if (optContext.Status == "403")
            {
                VisitUserModel vuser = MxAppHost.Instance.Container.Resolve<ISiteUser>() as VisitUserModel ?? new VisitUserModel();
#if DEBUG

                log.Info("403-isAuth:{0}, usfid:{1}".Fmt(currentId.IsAuthenticated, vuser.UserId));
#endif
                //if (vuser.UserId.IsNullOrEmptyGuid())
                if (!currentId.IsAuthenticated)
                {
                    HttpContext.Current.Response.Redirect("{0}?ReturnUrl={1}".Fmt(GetLoginPage, reqUrl.UrlEncode()));
                }
                else
                {
                    HttpContext.Current.Response.Redirect("{0}?ReturnUrl={1}".Fmt("~/account/not-authorized", reqUrl.UrlEncode()));
                }

            }
            if (optContext.Status == "404")
            {
                //HttpContext.Current.Response.Redirect("~/common/404?ReturnUrl={0}".Fmt(reqUrl.UrlEncode()));
            }

            //var n = new MatrixGroup.Framework.SFEvents.RequestContextEndEvent(HttpContext.Current);
            //log.Debug("status code from opt event:{0}".Fmt(new {resp= RespStatus, hstatus = n.Status}.SerializeToString()));
        }
        private void RegisterRoutes(RouteCollection routes)

        {

            //routes.IgnoreRoute("RestApi/{*pathInfo}");
            routes.Ignore("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );

            routes.MapRoute(
                "MxDefault",
                "mxg/{controller}/{action}/{id}",
                new { controller = "AuthService", action = "Index", id = UrlParameter.Optional }
                );
            
        }
    }

    /*public static class InitAppSession
    {
        private static ILog log = LogManager.GetLogger(typeof (InitAppSession));
        public static void InitAppCookie(string ckname, string message)
        {
            var usr = MxAppHost.Instance.Container.Resolve<ISiteAppRunner>().SiteUser.Instance() as VisitUserModel;
            // if the cookie is already created then this will now call in init
            var ck = GetValues("anouser");
            if (ck == null)
            {
                log.Warn("mxapp: cookie message is {0}".Fmt(message));
                GetOrSetCookie(ckname, message, true, false);
                //usr.UpdateSessionId(GetCkSessionId());
                FluentSiteUser<VisitUserModel>.Init(usr).Then(c =>
                {
                    c.UpdateSessionId(GetCkSessionId());

                    return c;
                }).Cached(GetCkSessionId());
            }
        }


        public static void DisposeAppCookie(string ckname)
        {
            GetOrSetCookie(ckname,"", false, true);
        }

        public static string GetCkSessionId()
        {
            var ckdef = GetValues("anouser");
            if (ckdef != null)
            {
                return ckdef["sessionid"];
            }
            return string.Empty;
        }
        public static NameValueCollection GetValues(string ckName)
        {
            var context = new HttpContextWrapper(HttpContext.Current);
            var domainCk = context.Request.Cookies[ckName];
            return domainCk?.Values;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ckName">optional</param>
        /// <param name="forceReset">need to set to true for create a app cookie</param>
        /// <param name="disposeMe">will remove cookie</param>
        private static HttpCookie GetOrSetCookie(string ckName, string message, bool forceReset, bool disposeMe)
        {
            
            ckName.DefaultIfNullEmpty("anouser");
            log.Warn("about to create cookie");
            var context = new HttpContextWrapper(HttpContext.Current);
            var domainCk = context.Request.Cookies[ckName];
            var timeStam = DateTime.UtcNow;
            if (disposeMe && domainCk != null)
            {
                domainCk["sessionid"] = "";
                domainCk["name"] = "";
                domainCk["message"] = message.DefaultIfNullEmpty("");
                domainCk["update"] = timeStam.ToString();
                //ck.Value = "";
                domainCk.Expires = timeStam.AddDays(-1);
                context.Response.Cookies.Add(domainCk);
            }
            if ( forceReset || disposeMe == false )
            {
                
                log.Warn("create cookie: {0}".Fmt(context.Request.AnonymousID));
                domainCk = domainCk ?? new HttpCookie(ckName);
                domainCk["sessionid"] = System.Web.Helpers.Crypto.GenerateSalt();
                domainCk["name"] = "anonymous";
                domainCk["message"] = "";
                domainCk["update"] = timeStam.ToString();
                //ck.Domain = ConfigurationManager.AppSettings["ckDomain"]?? context.Request.Url.Host;
                domainCk.Expires = timeStam.AddMinutes(60);
                context.Response.Cookies.Add(domainCk);
            }
            return domainCk;
        }
    }*/
}