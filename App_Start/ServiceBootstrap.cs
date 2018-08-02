using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Funq;
using MatrixGroup.Data.Interfaces;
using MatrixGroup.Data.Model;
using MatrixGroup.Data.Sitefinity;
using MatrixGroup.Implementation;
using MatrixGroup.Implementation.Helix.Security;
using MatrixGroup.Implementation.Security;
using Mx.IAFC.Webapp.IMis.Command;
using Mx.IAFC.Webapp.Interfaces;
using ServiceStack;
using ServiceStack.Caching;
using ServiceStack.Logging;
using ServiceStack.Text;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Claims;
using Telerik.Sitefinity.Security.Model;
using MatrixGroup.Data.Extension;
using MatrixGroup.Data.Interfaces.Security;
using MatrixGroup.Implementation.Services;
using MatrixGroup.Sitefinity.Config.AppSettings;
using Mx.IAFC.Webapp.Data;
using Mx.IAFC.Webapp.Data.Command;
using NLog.Internal;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace SitefinityWebApp.App_Start
{
    public class ServiceBootstrap: MxAppHost
    {
        private ILog log;

        public override void Configure(Container container)
        {
            log = LogManager.GetLogger(typeof (ServiceBootstrap));
            JsConfig.IncludeNullValues = true;
            //JsConfig<DateTime>.SerializeFn = time => new DateTime(time.Ticks, DateTimeKind.Local).ToString();

            //ServiceStack.Text.JsConfig.EmitCamelCaseNames = true; <- cause an issue with SF module do not turn on

            //ServiceStack.Text.JsConfig<Guid>.SerializeFn = guid => guid.ToString();
            container.Register<ICacheClient>(new MemoryCacheClient());
            
            /*CommonServiceConnectionModel initConfigSv = FluentSiteUser<CommonServiceConnectionModel>
                .InitFromCacheOrDefault(new CommonServiceConnectionModel()
            {
                ClientId = "29192542",
                EndpointUrl = "https://test.myhelix.org/App",
                SecretKey = "Bi4IB9Yhh5PrOa3y7PykntuQKg90ZNDiNes7tDNfwwKDgYIz4v",
                RedirectUrl = "https://iafc.matrixdev.net/Mxg/AuthService" //"https://iafc.matrixdev.net"
            }, "siteConn").Instance();*/
            /*CommonServiceConnectionModel initConfigSv = new CommonServiceConnectionModel()
            {
                ClientId = "29192542",
                EndpointUrl = "https://test.myhelix.org/App",
                SecretKey = "Bi4IB9Yhh5PrOa3y7PykntuQKg90ZNDiNes7tDNfwwKDgYIz4v",
                RedirectUrl = "https://iafc.matrixdev.net/Mxg/AuthService" //"https://iafc.matrixdev.net"
            };
            
            VisitUserModel stateUser = this.GetCurrentUser.Instance() as VisitUserModel;
            container.Register<ISiteAppRunner<HelixServiceClient>>(c =>
            {
                var sruner = new SiteAppRunner<HelixServiceClient>(initConfigSv)
                {
                    // should site user always reflect the current state or get default?
                    SiteUser = this.GetCurrentUser,
                    AppServiceClient = new AppServiceClient<HelixServiceClient>(),
                    ServiceConnection = initConfigSv
                };

                /*FluentSiteUser<ISiteUser>.Init(this.GetCurrentUser.Instance()).Cached("siteUser");
                FluentSiteUser<IServiceConnection>.Init(initConfigSv).Cached("siteConn");#1#

                return sruner;
            });*/
            //SfAppSettingsConfig mappconf =Telerik.Sitefinity.Configuration.Config.Get<SfAppSettingsConfig>();//["DomainName"];

            CommonServiceConnectionModel initConn = new CommonServiceConnectionModel()
            {
                ClientId = "29192542",
                EndpointUrl = System.Configuration.ConfigurationManager.AppSettings["helixApp"],
                SecretKey = "Bi4IB9Yhh5PrOa3y7PykntuQKg90ZNDiNes7tDNfwwKDgYIz4v",
                //RedirectUrl = "https://{0}/Mxg/AuthService".Fmt("iafc.matrixdev.net"), //"https://iafc.matrixdev.net"
                RedirectUrl = "https://{0}/Mxg/AuthService".Fmt(System.Configuration.ConfigurationManager.AppSettings["domainName"]), //"https://iafc.matrixdev.net"
                
            };
            initConn.SignInUrl = "{0}/OAuth/Authorize?client_id={1}&redirect_uri={2}&scope=basic,membership&state=blah&response_type=code"
                .Fmt(System.Configuration.ConfigurationManager.AppSettings["helixApp"], initConn.ClientId, initConn.RedirectUrl);
            initConn.SignOutUrl = "{0}/logout/{1}".Fmt(initConn.EndpointUrl, initConn.ClientId);
            

            container.Register<IServiceConnection>(c => initConn);
            //
            container.Register<IDbConnectionFactory>(
                c =>
                    new OrmLiteConnectionFactory(
                        System.Configuration.ConfigurationManager.ConnectionStrings[
                            "Simple.Data.Properties.Settings.DefaultConnectionString"].ConnectionString,
                        SqlServerDialect.Provider));
            
            using (var db = container.Resolve<IDbConnectionFactory>().Open())
            {
                //db.DropAndCreateTable<GenericResultSerialized>();
                db.CreateTableIfNotExists<GenericResultSerialized>();
                
            }

            //Database.OpenConnection(DatabaseHelper.ConnectionString)
            /*using (SqlConnection cnn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Simple.Data.Properties.Settings.DefaultConnectionString"].ConnectionString))
            {
                var cmd = cnn.CreateCommand();
                cnn.Open();
                cmd.CommandText = @"
                --if (OBJECT_ID('Mx_GenericResult') is not null)
--begin
--    TRUNCATE TABLE Mx_GenericResult
--end
                if (OBJECT_ID('Mx_GenericResult') is null)
                begin
	                create table Mx_GenericResult(
                        Id int identity primary key,
SessionId varchar(200),
                        [Data] varchar(max),
[User] varchar(max),
[Message] varchar(250),
                        CreationDate datetime not null, 
                        LastChangeDate datetime not null)
                    end
                ";
                cmd.Connection = cnn;
                cmd.ExecuteNonQuery();
            }
            dynamic db = Database.Open();
            //db.Mx_GenericResult.Insert(User: VisitUser.SerializeToString(), Message: "init", Data: "", CreationDate: DateTime.UtcNow, LastChangeDate:DateTime.UtcNow);
            //db.Mx_GenericResult.Insert(new GenericResultSerialized() {User = new VisitUserModel().SerializeToString(), Message = "init", CreationDate = DateTime.UtcNow, LastChangeDate = DateTime.UtcNow});
            log.Info("mxapp: get frpm db");
            //Simple.Data.ObjectReference
            object genresult = db.Mx_GenericResult.FindbyId(1);
                log.Info("mxapp: get :{0}".Fmt(genresult.SerializeToString()));*/
            
            VisitUserModel vUser = new VisitUserModel()
            {
                UserId = Guid.Empty.OneGuid(),
                ClientId ="", //System.Web.Security.Membership.GeneratePassword(16,6),
                SignInStatus = SfSignInStatus.Unknown,
                ReturnUrl = "~/",
                FirstName = "initUser"
            };

            
            // lazy load within the container
            container.Register<ISiteAppRunner>(m =>
            {
                var sessionId = System.Web.HttpContext.Current.GetCkSessionId();
                return new SiteAppRunner(initConn)
                {
                    //this potential to create multiple cache user
                    SiteUser = FluentSiteUser<ISiteUser>
                        .InitFromCacheOrDefault(vUser, sessionId)
                        .Then(c =>
                        {
                            VisitUserModel usr = c as VisitUserModel;
                            var ck = HttpContext.Current.Request;
                            Guid uid = ClaimsManager.GetCurrentIdentity().UserId;
                            SiteVisitorSessionCmd scmd = new SiteVisitorSessionCmd();
                            //get from db is have one
                            //dynamic db = Database.Open();
                            GenericResultSerialized _vuser = scmd.GetVisitorBySessionId(sessionId); //db.Mx_GenericResult.FindBySessionId(sessionId);
#if DEBUG
                            log.Warn("mxapp: in container for user: {0}, cacheID:{1}, user:{2}".Fmt(uid, sessionId, usr.SerializeToString()));
#endif
                            if (_vuser != null)
                            {
                                vUser = _vuser.User.FromJson<VisitUserModel>();
#if DEBUG
                                log.Info("mxapp: from db: {0}".Fmt(vUser.SerializeToString()));
#endif
                                return vUser;
                            }
                            // only for Auth session, but missing user email then prepopulate
                            /*if (!uid.IsNullOrEmptyGuid() && String.IsNullOrEmpty(usr.Email))
                            {
                                vUser = new HelixServiceClient(initConn, new SFAuthenServiceByHelixResponse(
                                    UserManager.GetManager(UserManager.GetDefaultProviderName()),
                                    UserProfileManager.GetManager(UserProfileManager.GetDefaultProviderName())
                                    ), vUser).InitUserByUserId(uid, ref usr).Entity as VisitUserModel;
                            }*/
                            // update session id
                            if (!String.IsNullOrEmpty(sessionId) && usr.SessionId != sessionId)
                            {
                                usr.UpdateSessionId(sessionId);
                            }
                            /*db.Mx_GenericResult.Insert(new GenericResultSerialized()
                            {
                                SessionId = sessionId,
                                CreationDate = DateTime.UtcNow,
                                LastChangeDate = DateTime.UtcNow,
                                Message = "new init from site app",
                                User = usr.SerializeToString()
                            });*/
                            scmd.UpdateBySessionId(sessionId, new GenericResultSerialized()
                            {
                                SessionId = sessionId,
                                CreationDate = DateTime.UtcNow,
                                LastChangeDate = DateTime.UtcNow,
                                Message = "new init from site app",
                                User = usr.SerializeToString()
                            });
                            return usr;
                        }),
                        
                    ServiceConnection = initConn,
                    AppServiceClient = FluentSiteUser<IAppServiceClient>.Init(new HelixServiceClient(initConn,
                        new SFAuthenServiceByHelixResponse(
                            UserManager.GetManager(UserManager.GetDefaultProviderName()),
                            UserProfileManager.GetManager(UserProfileManager.GetDefaultProviderName())
                            ), vUser))
                };
            }).ReusedWithin(ReuseScope.Container);
            container.Register<IServiceConnection>(c => initConn).ReusedWithin(ReuseScope.Request);

            /*container.Register<IAuthServiceResponse>(c =>
            {
                SFAuthenServiceByHelixResponse ss = new SFAuthenServiceByHelixResponse(
                    UserManager.GetManager(UserManager.GetDefaultProviderName()),
                    UserProfileManager.GetManager(UserProfileManager.GetDefaultProviderName()));
                return ss;
            }).ReusedWithin(ReuseScope.Request);*/

            container.Register<UserManager>(c => UserManager.GetManager(UserManager.GetDefaultProviderName())).ReusedWithin(ReuseScope.Request);
            container.Register<UserProfileManager>(c => UserProfileManager.GetManager()).ReusedWithin(ReuseScope.Request);
            container.Register<ISiteUser>(c => new VisitUserModel());

            //container.Register<ISoapConnection>(c => new IndividualSoapCmd("http://iafcimisapp1.iafc.org/webservicestest/wscontacts.asmx"));
        }

        
        
    }
}