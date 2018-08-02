using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MatrixGroup.Data;
using MatrixGroup.Data.Extension;
using MatrixGroup.Data.Interfaces;
using MatrixGroup.Data.Model;
using MatrixGroup.Implementation.Helix.Model;
using MatrixGroup.Implementation.Helix.Security;
using MatrixGroup.Implementation.ImpModel;
using MatrixGroup.Implementation.Security;
using MatrixGroup.Implementation.Services;
using Mx.IAFC.Webapp.Data;
using Mx.IAFC.Webapp.IMis.Command;
using ServiceStack;
using ServiceStack.Logging;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Claims;
using Telerik.Sitefinity.Services;
using Telerik.Windows.Documents.UI.TextDecorations.DecorationProviders;
using ResponseStatus = MatrixGroup.Data.ResponseStatus;

namespace SitefinityWebApp.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "AuthService", Title = "AuthService", SectionName = "Matrix")]
    [Telerik.Sitefinity.Web.UI.ControlDesign.ControlDesigner(typeof(WidgetDesigners.AuthService.AuthServiceDesigner))]
    public class AuthServiceController : Controller
    {
        private ILog log = LogManager.GetLogger(typeof (AuthServiceController));
        
        private IServiceConnection ServiceConn;

        [Category("String Properties")]
        public string Message { get; set; }
        public bool ShowLoginForm { get; set; }

        protected ISiteAppRunner SiteAppRunner()
        {
            return MxAppHost.Instance.Container.Resolve<ISiteAppRunner>();
        }

        public AuthServiceController()
        {

            ServiceConn = this.SiteAppRunner().ServiceConnection;

        }

        protected override void HandleUnknownAction(string actionName)
        {
            
            this.View("Index").ExecuteResult(ControllerContext);
            //this.View(new EmptyResult()).ExecuteResult(ControllerContext);

        }

        public ActionResult Index(string code = "")
        {   
            //return Content(String.Format("access code: {0}", code));
            if (SystemManager.IsDesignMode || SystemManager.IsPreviewMode)
            {
                return View("Index");
            }
            this.CreateOrUpdateCookie("cd", code);

            QueryDataResponse<HelixIndividual> qryIndv = new QueryDataResponse<HelixIndividual>() { ResponseStatus = ResponseStatus.Warning };
            //VisitUserModel bfAuth = this.GetSetAppSiteUser();
            VisitUserModel authUser = new VisitUserModel(); //this.GetSessionInforByUserId(this.MyId()).ToVisitUserModel();
            string returnUrl = this.GetCookieValue("reqUri");
#if DEBUG
            log.InfoFormat("myId in login:{0}, code:{1}", this.MyId(), code);
#endif

            // if not login & have a code then do authentication
            if ((this.MyId().IsNullOrEmptyGuid() || this.MyId().IsOneGuid()) && !String.IsNullOrEmpty(code))
            {
                this.SiteAppRunner().AppServiceClient
                    .Then(cl =>
                    {
                        HelixServiceClient client = cl as HelixServiceClient;
                        HelixIndividual helixIndv = new HelixIndividual();

                        client.ActivateHelixbyCode(code);
                        qryIndv = client.DataResponse();
                        if (qryIndv.ResponseStatus.Is(ResponseStatus.Success))
                        {
                            helixIndv = qryIndv.Result;

                            // RWB 20171013 Test
                            this.CreateOrUpdateCookie("IAFC", $"ContactId={helixIndv.MembershipId}&Password=H5erUjDICjI7EM4zJG3%2byg%3d%3d");
                            // RWB 20171013 Test

                        // RWB 20171012 - Username must be an email address for new accounts
                        helixIndv.UserName = helixIndv.Email;
                            VisitUserModel _vusr = helixIndv; // derived from the same class base
                            _vusr.ClientId = helixIndv.Client_id;
                            _vusr.Title = helixIndv.Title;
                            _vusr.ClientToken = new ServiceTokenModel()
                            {
                                ClientId = helixIndv.MembershipId,
                                ClientName = helixIndv.Email,
                                Token = helixIndv.access_token,
                                IssueTime = DateTime.UtcNow,
                                ExpireTime = DateTime.UtcNow.AddMinutes(12),
                                Refresh_token = helixIndv.Refresh_token,
                                Message = (new HelixIndividual() { Title = helixIndv.Title, Client_id = helixIndv.Client_id, MembershipId = helixIndv.MembershipId }).SerializeToString()
                                //Message = (new { Title = helixIndv.Title, MembershipId = helixIndv.MembershipId, Client_id = helixIndv.Client_id }).ToJson()
                            };
                            _vusr.ReturnUrl = returnUrl;
                            _vusr.Profile = new VisitorProfile() { Roles = new List<UserRole>() { new UserRole() { Name = "NonMember" } } };

                            /*if (!String.IsNullOrEmpty(helixIndv.MembershipId))
                            {
                                _vusr.Profile.Roles = new List<UserRole>() { new UserRole() { Name = "Member" } };
                            }*/
                            if (!String.IsNullOrEmpty(helixIndv.roles) && helixIndv.roles.ToLower().IndexOf("iafcmember") >= 0)
                            {
                                _vusr.Profile.Roles = new List<UserRole>() { new UserRole() { Name = "Member" } };
                            }


#if DEBUG
                            log.InfoFormat("convert user from Helix is: {0}", 
                                (new
                                {
                                    helix = helixIndv,
                                    Vuser = _vusr,
                                    ActivateMis = String.Format("{0}/{1}/{2}/true", ConfigurationManager.AppSettings["iMiSLoginBaseUri"]?.ToString(),
                                    ConfigurationManager.AppSettings["iMiSLogin"], _vusr.UserName ),
                                    
                                    
                                }).SerializeToString());
#endif

                            /*client
                            .LoginToIMis(System.Web.HttpContext.Current, ConfigurationManager.AppSettings["iMiSLoginBaseUri"], 
                            ConfigurationManager.AppSettings["iMiSLogin"], ((VisitorModel)_vusr).UserName);*/

                            
                            this.CreateOrUpdateCookie("ud", _vusr.ClientId);
                            

                            QueryDataResponse<VisitUserModel> sfResponse = new QueryDataResponse<VisitUserModel>();
                            client.AuthenticateBy((cnn, _srv) =>
                            {
                                sfResponse = _srv.GetAuthResponse(_vusr);

                                if (sfResponse.ResponseStatus.Is(ResponseStatus.Success))
                                {
                                    // login to iMIS
                                    /*client
                                    .LoginToIMis(ConfigurationManager.AppSettings["iMiSLoginBaseUri"],
                                    ConfigurationManager.AppSettings["iMiSLogin"], ((VisitorModel)_vusr).UserName);*/

                                    //this.CreateOrUpdate("hlsession", sfResponse.Result.ClientId);
                                    //this.SiteAppRunner().SiteUser = FluentSiteUser<ISiteUser>.Init(sfResponse.Result).Cached(this.SessionId());
                                    var _updateModel = sfResponse.Result;
                                    var _cacheUser = new GenericResultSerialized()
                                    {
                                        User = _updateModel.SerializeToString(),
                                        SessionId = "123",//this.SessionId(),
                                        UserId = _updateModel.UserId,
                                        Message = "init from Auth",
                                        CreationDate = DateTime.UtcNow,
                                        LastChangeDate = DateTime.UtcNow,
                                        // RWB 20170911 - Removed contactID ContactId = helixIndv.MembershipId,
                                        Data = helixIndv.ToJson<HelixIndividual>(),
                                    };
                                    //_updateModel.UpdateSessionId(this.SessionId());
                                    //this.GetSetAppSiteUser(sfResponse.Result);
                                    //this.SetAppSiteUser(_updateModel.UserId, _cacheUser);

#if DEBUG
                                    log.Info("update user to cache:{0} by id:".Fmt(new
                                    {
                                        cache = _cacheUser,
                                        sessonid = "123",//this.SessionId(),
                                        data = _updateModel
                                    }.SerializeToString()));
#endif
                                }
                                else
                                {
                                    log.Warn("issue with auth with SF:{0}".Fmt(sfResponse.Message));
                                }
                                authUser = _vusr;
                                return _vusr;
                            });
                        }
                        return cl;
                    });
            }

            // to show as a button then this need to be checked
            if (ShowLoginForm)
            {
                return View("Index");
            }
            // or last step is to redirect back

            if (!SystemManager.IsDesignMode || !SystemManager.IsPreviewMode)
            {
                //return Redirect(authUser.ReturnUrl.DefaultIfNullEmpty("/"));
                if (this.MyId().IsNullOrEmptyGuid())
                {
                    log.InfoFormat("auth-noCode:{0}", HttpContext.Request?.Url?.AbsolutePath);
                    return Redirect("/");
                }
                return Redirect(returnUrl.DefaultIfNullEmpty("/"));
                //return Redirect("http://members.iafc.org/helix/MembershipSignIn/ktomko/true");
            }

            return View("Index");
        }

        public ActionResult CaptureTest(string ReturnUrl = "")
        {
            this.CreateOrUpdateCookie("reqUri", ReturnUrl.DefaultIfNullEmpty("/"));
            return
                Content(String.Format("will:{0} by id:{1}, cook:{2}, myinfor:{3}", new object[] { this.MyId().IsNullOrEmptyGuid(), this.MyId(), this.GetCookieValue("reqUri"), this.SfUser() }));
        }

        public ActionResult SignInByHelix(string ReturnUrl = "")
        {

            //VisitUserModel usr = this.GetSessionInforByUserId(this.MyId()).ToVisitUserModel();
            ReturnUrl = ReturnUrl.DefaultIfNullEmpty(Request.QueryString["ReturnUrl"]?.ToString());
#if DEBUG
            log.InfoFormat("id is:{0}, then goto: {1}, user:{2}, cook:{3}", new object[] { this.MyId(), ServiceConn.SignInUrl, "", this.GetCookieValue("reqUri") });
#endif

            //return Redirect(ServiceConn.SignInUrl);
            // will need to work on remove expired cookie
            if (this.SfUser() == null)
            {
                //this.ProcessServiceSignOut();
                

                if (String.IsNullOrEmpty(ServiceConn.EndpointUrl))
                {
                    return Redirect("~/helixlogin=null");
                }
                //VisitUserModel usr = this.GetSessionInforByUserId(this.MyId()).ToVisitUserModel();
                this.CreateOrUpdateCookie("reqUri", ReturnUrl);
#if DEBUG
                log.InfoFormat("AfterLoginReturnUrl:{0} cook:{1}, part:{2}",
                    ReturnUrl,
                    this.GetCookieValue("reqUri"),
                    Request.Url?.GetLeftPart(UriPartial.Query));
#endif
                //usr.ReturnUrl = String.IsNullOrEmpty(ReturnUrl) ? "~/" : ReturnUrl;
                //this.GetSetAppSiteUser(usr.UserId, usr);

                // go to helix login
                return Redirect(ServiceConn.SignInUrl);
            }
            else
            {
                return Redirect(this.GetCookieValue("reqUri").DefaultIfNullEmpty("/"));
            }

            //return Content("Kun ker:{0}".Fmt((new {user= this.AppSiteUser(), myid = this.MyId(), name= ClaimsManager.GetCurrentIdentity().Name}).SerializeToString()));
        }

        public ActionResult Index_(string code = "")
        {
            if (String.IsNullOrEmpty(code))
            {
                return Redirect("~/?code=null");
            }

            // have code & not login then do activate code
            if ((this.MyId().IsNullOrEmptyGuid() || this.MyId().IsOneGuid()) && !String.IsNullOrEmpty(code))
            {
                log.Info("what myid: {0}, by code:{1}".Fmt(this.MyId(), code));
                /*var m = HelixServiceClient.Init(ServiceConn)
                    .GetAccessTokenByCode(code);*/
                var conn = MxAppHost.Instance.Container.Resolve<IServiceConnection>();
                var result = new ServiceTokenModel();

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
                    log.Info("resp:{0}".Fmt((new { helix = responseString, red = conn.RedirectUrl, client = conn.ClientId, sec = conn.SecretKey })));
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


                /*HelixClient response = HelixServiceClient.Init(ServiceConn)
                    .GetAccessTokenByCode(code).Instance();
                if (response.HelixDataResponse.ResponseStatus.Is(MatrixGroup.Data.ResponseStatus.Warning))
                {
                    ViewData["warning"] = response.HelixDataResponse.Message;
                    return Redirect("~/NotifyInfo/WarningInfo");
                }*/
            }
            return Redirect("~/?code={0}".Fmt(code));
        }
        
        private void ProcessServiceSignOut()
        {
            string clientId = GetCookieValue("ud");
            this.SiteAppRunner().AppServiceClient.Then((Func<IAppServiceClient, object>)(c =>
            {
                HelixServiceClient helixServiceClient1 = c as HelixServiceClient;
                //VisitUserModel visitUserModel = GenericResultSerializedExtension.ToVisitUserModel(this.GetSessionInforByUserId(sfid));
                //this.log.InfoFormat("accesskey:{0}", (object)TextExtensions.SerializeToString<VisitUserModel>(visitUserModel));
                try
                {
                    clientId = this.GetCookieValue("ud");
                    if (!string.IsNullOrEmpty(clientId) && helixServiceClient1 != null)
                    {   //ServiceTokenModel clientToken = ((VisitorModel)visitUserModel).ClientToken;
                        //string accessToken = clientToken != null ? clientToken.Token : (string)null;
                        helixServiceClient1.SignOff(clientId, null);
                    }

                }
                catch (Exception ex)
                {
                    log.ErrorFormat("signoff exception:{0}, inner:{1}", ex.Message, ex.InnerException?.Message);
                }
                
                AppSessionServices.DisposeAppCookie(System.Web.HttpContext.Current, "anouser");
                this.ClearCookie("ud");
                return (object)c;
            }));
            //this.ClearCookie("hlsession");
            this.SingOutCleanUp();
            Telerik.Sitefinity.Security.SecurityManager.Logout();

        }

        public ActionResult SignOut()
        {
            //this.GetAppCookie();
            Guid sfid = this.MyId();
            string clientId = GetCookieValue("ud");
            /*this.SiteAppRunner().AppServiceClient.Then((Func<IAppServiceClient, object>)(c =>
            {
                HelixServiceClient helixServiceClient1 = c as HelixServiceClient;
                VisitUserModel visitUserModel = GenericResultSerializedExtension.ToVisitUserModel(this.GetSessionInforByUserId(sfid));
                this.log.InfoFormat("accesskey:{0}", (object)TextExtensions.SerializeToString<VisitUserModel>(visitUserModel));
                try
                {
                    clientId = this.GetCookieValue("ud");
                    if (!string.IsNullOrEmpty(clientId) && helixServiceClient1 != null)
                    {
                        HelixServiceClient helixServiceClient2 = helixServiceClient1;
                        string contactId = clientId;
                        ServiceTokenModel clientToken = ((VisitorModel)visitUserModel).ClientToken;
                        string accessToken = clientToken != null ? clientToken.Token : (string)null;
                        helixServiceClient2.SignOff(contactId, accessToken);
                    }

                }
                catch (Exception ex)
                {
                    log.ErrorFormat("signoff exception:{0}, inner:{1}", ex.Message, ex.InnerException?.Message);
                }
                finally
                {
                    this.DeleteSessionInforByUserId(this.MyId());
                }
                AppSessionServices.DisposeAppCookie(System.Web.HttpContext.Current, "anouser");
                this.ClearCookie("ud");
                return (object)c;
            }));*/

            this.SingOutCleanUp();
            SecurityManager.Logout();
            //https://test.myhelix.org/App/logout/{0}?ReturnUrl=http://{1}
            string signOffUrl = ConfigurationManager.AppSettings["signOffUrl"];

            if (String.IsNullOrEmpty(clientId))
            {
                return Redirect("/?logout=noid");
            }

            if (!String.IsNullOrEmpty(signOffUrl))
            {
                var sb = new StringBuilder();
                sb.Append(signOffUrl).Append("{0}");
                return this.Redirect(string.Format(sb.ToString(), new object[2]
                {
                    clientId,
                    ConfigurationManager.AppSettings["domainName"]
                }));
            }
            return Redirect("/?signOff=null");
        }


        public Guid MyId()
        {
            return ClaimsManager.GetCurrentIdentity().UserId;
        }
        public Telerik.Sitefinity.Security.Model.User SfUser()
        {
            return this.MyId().IsNullOrEmptyGuid() ? null : Telerik.Sitefinity.Security.UserManager.GetManager().GetUser(this.MyId());
        }

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
        protected void SingOutCleanUp()
        {
            
        }
    }
}