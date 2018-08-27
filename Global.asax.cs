using SitefinityWebApp.Custom.ContentList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using SitefinityWebApp.Custom.News;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Publishing;
using Telerik.Sitefinity.Services.Search.Publishing;
using SitefinityWebApp.Custom.ResourceLibrary;
using System.Web.Routing;
using System.Web.Http;
using Common.Logging;
using MatrixGroup.Data.Interfaces;
using MatrixGroup.Data.Model;
using MatrixGroup.Implementation.Security;
using MatrixGroup.Implementation.Services;
using ServiceStack;
using SitefinityWebApp.App_Start;
using SitefinityWebApp.Custom;
using Telerik.Sitefinity.Publishing.Pipes;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.DynamicModules.Events;
using SitefinityWebApp.Custom.IAFCHandBook;
using Telerik.Sitefinity.GenericContent.Model;

namespace SitefinityWebApp
{
	public class Global : System.Web.HttpApplication
	{
	    private SfBootstraper bootstraper;
	    private ILog log = LogManager.GetLogger(typeof (Global));
		protected void Application_Start(object sender, EventArgs e)
		{
            /*Telerik.Sitefinity.Services.SystemManager.ApplicationStart += 
                new EventHandler<EventArgs>(SystemManager_ApplicationStart);*/

            bootstraper = new SfBootstraper();
            bootstraper.Setup();
            ///Bootstrapper.Initialized += Bootstrapper_Initialized;
            ///

            SystemManager.ApplicationStart += SystemManager_ApplicationStart;
        }

        private void SystemManager_ApplicationStart(object sender, EventArgs e)
        {
            EventHub.Subscribe<IDynamicContentCreatedEvent>(evt => DynamicContentCreatedEventHandler(evt));
            EventHub.Subscribe<IDynamicContentUpdatedEvent>(evt => DynamicContentUpdatedEventHandler(evt));
        }

        private void DynamicContentCreatedEventHandler(IDynamicContentCreatedEvent evt)
        {
            var item = evt.Item;
            var itemType = item.GetType();

            var helper = new IAFCHandBookHelper();

            if (itemType == helper.ResourceType ||
                itemType == helper.ExternalResourcesType)
            {
                if (item.Status == ContentLifecycleStatus.Live && item.Visible)
                {
                    if (helper.IsResourceContainedWithinTopicsCategory(item.Id, itemType))
                    {
                        helper.CreateIAFCHandBookResourcesData(item.Id, itemType);
                    }
                }
            }
        }

        private void DynamicContentUpdatedEventHandler(IDynamicContentUpdatedEvent evt)
        {
            var item = evt.Item;
            var itemType = item.GetType();

            var helper = new IAFCHandBookHelper();

            if (itemType == helper.ResourceType ||
                itemType == helper.ExternalResourcesType)
            {
                if (item.Status == ContentLifecycleStatus.Live && item.Visible)
                {
                    if (helper.IsResourceContainedWithinTopicsCategory(item.Id, itemType))
                    {
                        if (helper.IsHandBookResourcesDataExistsFor(item.Id, itemType))
                        {
                            helper.UpdateIAFCHandBookResourcesData(item.Id, itemType);
                        }
                        else
                        {
                            helper.CreateIAFCHandBookResourcesData(item.Id, itemType);
                        }
                    }
                    else
                    {
                        if (helper.IsHandBookResourcesDataExistsFor(item.Id, itemType))
                        {
                            helper.DeleteIAFCHandBookResourcesData(item.Id, itemType);
                        }
                    }
                }
            }
        }

        /*void SystemManager_ApplicationStart(object sender, EventArgs e)
        {
            SFRoutes.RegisterType();
        }*/

        protected void Application_BeginRequest(Object sender, EventArgs e)
	    {
            // will init a placeholder,
	        /*var user = MxAppHost.Instance.Container.Resolve<ISiteAppRunner>().SiteUser.Instance() as VisitUserModel?? new VisitUserModel();
	        var appck = System.Web.HttpContext.Current.GetCkSessionId();
	        if (String.IsNullOrEmpty(appck))
	        {
                System.Web.HttpContext.Current.InitAppCookie("anouser", "1-init");
                //user.UpdateSessionId(HttpContext.Current.GetCkSessionId());
	            //FluentSiteUser<VisitUserModel>.Init(user).Cached(HttpContext.Current.GetCkSessionId());
	        }*/
            
	    }


        void Bootstrapper_Initialized(object sender, Telerik.Sitefinity.Data.ExecutedEventArgs e)
		{
			if (e.CommandName == "Bootstrapped")
			{
                //RegisterRoutes(RouteTable.Routes);
				PublishingSystemFactory.UnregisterPipe(SearchIndexOutboundPipe.PipeName);
                PublishingSystemFactory.RegisterPipe(SearchIndexOutboundPipe.PipeName, typeof(ResourceLibraryOutboundPipe));
			}
		}
        public void AnonymousIdentification_Creating(object sender, AnonymousIdentificationEventArgs args)
        {
            args.AnonymousID = Guid.NewGuid().ToString();
        }
        public static void RegisterRoutes(RouteCollection routes)
        {
            /*routes.Ignore("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );*/
        }

        protected void Application_Error()
        {
            var context = HttpContext.Current;
            var httpException = context.Server.GetLastError() as HttpException;

            if (httpException != null)
            {
                var statusCode = httpException.GetHttpCode();
                var path = context.Request.Url.AbsolutePath;
                log.ErrorFormat("GlobalEx:code::{0}, path:{1}", statusCode, path);
                
                if (statusCode == 500)
                {
                    log.ErrorFormat("500:{0}", path);
                    context.Server.ClearError();
                }
                else
                {
                    log.ErrorFormat("GlobalEx:code::{0}, path:{1}", statusCode, path);
                }
            }
        }
    }
}