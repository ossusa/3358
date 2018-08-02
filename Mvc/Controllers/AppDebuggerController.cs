using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using MatrixGroup.Data.Interfaces;
using MatrixGroup.Data.Interfaces.Security;
using MatrixGroup.Data.Model;
using MatrixGroup.Implementation.Helix.Security;
using MatrixGroup.Implementation.ImpModel;
using MatrixGroup.Implementation.Security;
using ServiceStack;
using ServiceStack.Logging;
using Telerik.Sitefinity.Security;

namespace SitefinityWebApp.Mvc.Controllers
{
    public class AppDebuggerController : SharedController
    {
        private ILog log = LogManager.GetLogger(typeof (AppDebuggerController));

        // GET: AppDebugger
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddMe()
        {
            var sf = new SFAuthenServiceByHelixResponse(UserManager.GetManager(UserManager.GetDefaultProviderName()), UserProfileManager.GetManager() );
            var sf2 = new SFAuthHelixTest(MxAppHost.Instance.Container.Resolve<IServiceConnection>());
            var res = sf.GetAuthResponse(new VisitUserModel()
            {
                FirstName = "patlamp",
                LastName = "Lamp",
                Email = "lamp@mail.com",
                UserName = "uname",
                Password = "1234",
                ClientId = "999999",
                ClientToken = new ServiceTokenModel() { ClientId = "1232"}
                
            });

            return Content("result: {0}".Fmt(res.SerializeToString()));
        }

        public ServiceTokenModel GetAccessTokenByCode(string code)
        {
            var result = new ServiceTokenModel();
            var conn = MxAppHost.Instance.Container.Resolve<IServiceConnection>();

            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>

                {
                    {"grant_type", "authorization_code"},
                    {"code", code},
                    {"redirect_uri", conn.RedirectUrl},
                    {"client_id", conn.ClientId},
                    {"client_secret", conn.SecretKey}
                    /*{"redirect_uri","https://iafc.matrixdev.net/" },
                    {"client_id", "29192542"},
                    {"client_secret", "Bi4IB9Yhh5PrOa3y7PykntuQKg90ZNDiNes7tDNfwwKDgYIz4v"}*/
                };

                var content = new FormUrlEncodedContent(values);
                var response = client.PostAsync("{0}/Token".Fmt(conn.EndpointUrl), content).Result;

                // where accessToken create from successfully ask Helix exchange infor from ServiceTokenModel above.
                var respToken = new SiteAccessTokenModel();
                var responseString = response.Content.ReadAsStringAsync().Result;
                log.Info("resp:{0}".Fmt((new {helix= responseString , red=conn.RedirectUrl, client=conn.ClientId, sec=conn.SecretKey })));
                respToken = responseString.FromJson<SiteAccessTokenModel>();

                if (respToken != null)
                {
                    result.Refresh_token = respToken.Refresh_token;
                    result.Token = respToken.Access_token;
                    result.IssueTime = respToken.Issued;
                    result.ExpireTime = respToken.Expires;
                    result.ClientId = respToken.ContactId.ToString();
                }


                /*_log.Debug($"from Helix service client secon-token:{responseString.SerializeToString()}," +
                           $" objReso:{respToken.SerializeToString()}, Token:{respToken.Access_token}");*/
            }
            return result;
        }

        public ActionResult UpdateMe()
        {
            VisitUserModel cuser = this.AppSiteUser();
            cuser.FirstName = "update nem";
            this.SiteAppRunner().SiteUser.Then(c => cuser).Cached(this.SessionId());

            return Content("cahceuser:{0}".Fmt((new {cuser = this.AppSiteUser(), SfUser = this.SfUser()}).SerializeToString() ));
        }
        public ActionResult GetSimple(string code)
        {
            return Content("result:{0}".Fmt(GetAccessTokenByCode(code).SerializeToString() ));

        }

        public ActionResult GetPlace()
        {
            return View();
        }

        
        
    }
}