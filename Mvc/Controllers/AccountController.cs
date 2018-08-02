using System;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using MatrixGroup.Data.Extension;
using MatrixGroup.Implementation.Security;
using Telerik.Sitefinity.Mvc;
using SitefinityWebApp.Mvc.Models;
using ServiceStack;

namespace SitefinityWebApp.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "Account", Title = "Account", SectionName = "Matrix")]
    [Telerik.Sitefinity.Web.UI.ControlDesign.ControlDesigner(typeof(WidgetDesigners.Account.AccountDesigner))]
    public class AccountController : SharedController
    {
        [Category("General")]
        public string ViewTemplate { get; set; }
        public bool LoginRequired { get; set; }
        public string ThankyouUrl { get; set; }


        private string ViewTemplateVal
        {
            get
            {
                if (String.IsNullOrEmpty(ViewTemplate))
                {
                    return "MemberJoin";
                }

                return ViewTemplate;
            }
        }

        protected override void HandleUnknownAction(string actionName)
        {
            string viewname = this.ViewTemplate;
            /*if (LoginRequired)
            {
                var refurl = ControllerContext.HttpContext.Request.UrlReferrer;
                var redUrl = refurl?.AbsoluteUri;
                if (this.MyId().IsNullOrEmptyGuid() || this.MyId().IsOneGuid())
                {
                    Redirect("/Mxg/AuthService/SignInByHelix/?ReturnUrl={0}".Fmt(redUrl.DefaultIfNullEmpty("/")));
                }
            }*/
            
            AccountModel model = new AccountModel();

            this.View(viewname, model).ExecuteResult(ControllerContext);
            

        }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [Category("String Properties")]
        public string Message { get; set; }

        /// <summary>
        /// This is the default Action.
        /// </summary>
        public ActionResult _Index()
        {
            var model = new AccountModel();
            model.Message = "default";

            return View("Index", model);
        }

        public ActionResult MemberJoin()
        {
            //FluentSiteUser<AccountModel>.Init().Cached()
            var model = new AccountModel();
            return View(model);
        }

        #region

        public JsonResult MyContact()
        {

            return Json(new {data = this.MyId()}, JsonRequestBehavior.AllowGet);
        }
#endregion

    }
}