using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using MatrixGroup.Sitefinity.Config.AppSettings;
using Telerik.Microsoft.Practices.Unity.Utility;
using Telerik.Sitefinity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Blogs.Model;
using Telerik.Sitefinity.DynamicModules;
using Telerik.Sitefinity.DynamicModules.Model;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Lifecycle;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Model.Localization;
using Telerik.Sitefinity.Modules;
using Telerik.Sitefinity.Modules.Blogs;
using Telerik.Sitefinity.Modules.Libraries;
using Telerik.Sitefinity.Modules.News;
using Telerik.Sitefinity.News.Model;
using Telerik.Microsoft.Practices.EnterpriseLibrary.Common.Utility;
using Telerik.Sitefinity.Utilities.TypeConverters;
using Telerik.Sitefinity.Workflow;
//Taxonomies Namespaces
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Taxonomies.Model;
using System.Data.SqlClient;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Data.Configuration;

//using SitefinityWebApp.DataObjects;

//using FMI.Sitefinity.Modules;

namespace SitefinityWebApp.Scripts
{
    public partial class WebForm1 : System.Web.UI.Page
    {


        Dictionary<string, string> DoctypeMapping = new Dictionary<string, string>(){

                                                                        {"Articles","articles"},
                                                                        {"Tear Sheets","tear-sheets"},
                                                                        {"Samples","samples"},
                                                                        {"Additional Resources","additional-resources"}
                                                                    };

        Dictionary<int, string> NewsCategoryTaxonomyMapping = new Dictionary<int, string>(){
                                                                    
                                                                    };

        private const string siteUrl = "http://www.fmi.org/";

        protected void Page_Load(object sender, EventArgs e)
        {
           FixPressReleasePost("press-releases-press-release");
            FixOnScenePost("on-scene-articles-on-scene-article");
           FixResourcePost("press-releases-press-release");
            //ImportToBlogs();
            //ImportToNews();
            //ImportToModule();
            //FixBlogsPermissions("articles");
            //FixBlogsPermissions("samples");
            //FixBlogsPermissions("tear-sheets");
           
        }

        private void ImportToBlogs()
        {
            var sqlQuery = @"
								SELECT *
								FROM IAFC_migration
 								WHERE  channel='Articles' AND status='open'
                               
                                ORDER BY contentId;
							";

            var dbConn = new SqlConnection("Data Source=luke.theforce.local;Initial Catalog=ASF_migration;User ID=rw_asf_migration;Password=Nxgfsd#h877;pooling=true;connection lifetime=120;");
            var sqlComm = new SqlCommand(sqlQuery, dbConn);

            //sqlComm.Parameters.Add(new SqlParameter("@channel", "Articles"));
            sqlComm.Connection.Open();
            var dataReader = sqlComm.ExecuteReader();

            var contentManager = BlogsManager.GetManager();
            var taxManager = TaxonomyManager.GetManager();
            var usersManager = RoleManager.GetManager();
            var lm = LibrariesManager.GetManager();

            while (dataReader.Read())
            {
                var docType = dataReader["Channel"].ToString();
                Response.Write("<br />----" + dataReader["title"].ToString() + "(doctype " + docType + ") -------<br />"); //Response.Flush();
                var parent = contentManager.GetBlogs().Where(b => b.UrlName == DoctypeMapping[docType]).Single();
                var ci = contentManager.CreateBlogPost();
                ci.Parent = parent;
                ci.Title = TransformTitle(dataReader["title"].ToString());
                if (dataReader["articleText"].ToString() != "null")
                    ci.Content = TransformContent(dataReader["articleText"].ToString());
                else if (dataReader["description"].ToString() != "null")
                    ci.Content = TransformContent(dataReader["description"].ToString());

                //ci.Summary = TransformSumary(importItem.summary);
                //else ci.Summary = (importItem.fulltext);
                //var custom_listing = TransformContent(importItem.custom_listing);
                if (dataReader["subTitle"].ToString() != "null") 
                    ci.SetValue("Subtitle", dataReader["subTitle"].ToString());

                if (dataReader["Author"].ToString() != "null")
                    ci.SetValue("Author", dataReader["Author"].ToString());
                if (dataReader["jobTitle"].ToString() != "null")
                    ci.SetValue("JobTitle", dataReader["jobTitle"].ToString());
                if (dataReader["JobRole"].ToString() != "null")
                    ci.SetValue("JobRole", dataReader["JobRole"].ToString());

                if (dataReader["Company"].ToString() != "null")
                    ci.SetValue("Company", dataReader["Company"].ToString());
                if (dataReader["CompanyURL"].ToString() != "null")
                    ci.SetValue("CompanyURL", dataReader["CompanyURL"].ToString());
                if (dataReader["QuarterlyEssential"].ToString() != "null")
                    ci.SetValue("QuarterlyEssential", dataReader["QuarterlyEssential"].ToString());
                if (dataReader["asideImageCaption"].ToString() != "null")
                    ci.SetValue("ImageCaption", dataReader["asideImageCaption"].ToString());
                if (dataReader["asideQuote"].ToString() != "null")
                    ci.SetValue("AsideQuote", dataReader["asideQuote"].ToString());
                if (dataReader["asideAttrib"].ToString() != "null")
                    ci.SetValue("Attribution", dataReader["asideAttrib"].ToString());
                if (dataReader["asideLinkUrl"].ToString() != "null")
                    ci.SetValue("AsideLink", dataReader["asideLinkUrl"].ToString());
                

                ci.SourceKey = dataReader["contentId"].ToString();
                ci.UrlName = Regex.Replace(ci.Title.ToLower(), DefinitionsHelper.UrlRegularExpressionFilter, "-") + "-" + dataReader["contentId"].ToString(); ;
                ci.PublicationDate = DateTime.Parse(dataReader["entry_date"].ToString());
                ci.LastModified = DateTime.Parse(dataReader["entry_date"].ToString());
                ci.DateCreated = DateTime.Parse(dataReader["entry_date"].ToString());
                contentManager.SaveChanges();

                var categories = new List<string>();


                foreach (var cat in dataReader["categories"].ToString().Split('|'))
                {
                    if (cat.Trim() != "")
                    {
                        var cleanCat = Regex.Replace(cat, @"([\n\t\r]|(1ef8)|(4d0)|(@n[\s|\w]+bsp@))", "", RegexOptions.IgnoreCase).Trim();
                        var taxonId = taxManager.GetTaxa<HierarchicalTaxon>().Where(t => t.UrlName == Regex.Replace(cleanCat, DefinitionsHelper.UrlRegularExpressionFilter, "-")).Single().Id;
                        Response.Write(taxonId + ","); //Response.Flush();
                        categories.Add(cleanCat);
                        try { ci.Organizer.AddTaxa("Category", taxonId); }
                        catch { } // we put this in a try-catch b/c the above mapping may cause dups

                    }
                }
 

                contentManager.SaveChanges();
                var relatedFiles = "";
                var fileUrl = "";
                var documentFieldCounter = 1;
                foreach (var pdfUrl in dataReader["docFile"].ToString().Split('|'))
                {
                    if (pdfUrl.Trim() != ""){
                        var localpdfUrl = pdfUrl.Replace("http://www.smallfoundations.org", "http://phase2.psers.matrixdev.net");
                        Response.Write("<br />----" + pdfUrl + "---<br />"); //Response.Flush();

                        var relatedFileTitle = (!String.IsNullOrEmpty(ci.Title)) ? ci.Title.ToString() : pdfUrl.ToString().Trim();

                        var permissions = new List<string>();
                        if (dataReader["publicFlag"].ToString() != "y")
                        {
                            permissions.Add("Member-Only");
                            permissions.Add("Staff");
                            permissions.Add("Siteadmin");
                        }

                        var fileGuid = ImportNewsletterPDFs(localpdfUrl, dataReader["title"].ToString(), DateTime.Parse(dataReader["entry_date"].ToString()), DoctypeMapping[docType], relatedFileTitle, "", categories.AsQueryable(), permissions.AsQueryable() );
                        fileUrl = lm.GetDocument(fileGuid).Url.Replace("http://", "");
                        fileUrl = ((fileUrl.IndexOf('/') >= 0) ? fileUrl.Substring(fileUrl.IndexOf('/')) : pdfUrl);
                        relatedFiles += String.Format(@"<li><a sfref=""[documents]{0}"" href=""{1}"" title=""{2}"">{2}</a></li>",
                                                        fileGuid,
                            //libraryUrl,
                                                        fileUrl,
                                                        relatedFileTitle
                                                    );
                        if (fileUrl.Trim() != "")
                        {
                            switch (documentFieldCounter)
                            {
                                case 1: ci.SetValue("Document", fileGuid.ToString()); break;
                                case 2: ci.SetValue("Document2", fileGuid.ToString()); break;
                                case 3: ci.SetValue("Document3", fileGuid.ToString()); break;
                            }
                            documentFieldCounter += 1;
                        }
                    }
                }

                if (dataReader["publicFlag"].ToString() != "y")
                {
                    var secSetNames = contentManager.SecurityRoot.SupportedPermissionSets;
                    contentManager.BreakPermiossionsInheritance(ci);

                    var roleId = SecurityManager.ApplicationRoles["Everyone"].Id;
                    Telerik.Sitefinity.Security.Model.Permission permissionForEveryone = ci.GetActivePermissions().Where(p => p.PrincipalId == roleId && p.ObjectId == ci.Id && p.SetName == secSetNames[1]).FirstOrDefault();

                    //var newroleId = SecurityManager.ApplicationRoles["Staff"].Id;
                    var role = usersManager.GetRoles().Where(p => p.Name == "Member-Only").SingleOrDefault();
                    if (permissionForEveryone != null)
                    {
                        permissionForEveryone.UngrantActions("ViewBlogPost");
                        contentManager.SaveChanges();
                    }
                    var permissionForAutenticated = contentManager.CreatePermission(secSetNames[1], ci.Id, role.Id);
                    ci.Permissions.Add(permissionForAutenticated);
                    permissionForAutenticated.GrantActions(false, "ViewBlogPost");
                   
                    contentManager.SaveChanges();

                    role = usersManager.GetRoles().Where(p => p.Name == "Staff").SingleOrDefault();
                    permissionForAutenticated = contentManager.CreatePermission(secSetNames[1], ci.Id, role.Id);
                    ci.Permissions.Add(permissionForAutenticated);
                    permissionForAutenticated.GrantActions(false, "ViewBlogPost");

                    contentManager.SaveChanges();

                    role = usersManager.GetRoles().Where(p => p.Name == "Siteadmin").SingleOrDefault();
                    permissionForAutenticated = contentManager.CreatePermission(secSetNames[1], ci.Id, role.Id);
                    ci.Permissions.Add(permissionForAutenticated);
                    permissionForAutenticated.GrantActions(false, "ViewBlogPost");

                    contentManager.SaveChanges();
 
                }
 
                ci.ApprovalWorkflowState = "PUBLISHED";
                contentManager.Lifecycle.Publish(ci);
                contentManager.SaveChanges();

                ci.DateCreated = DateTime.Parse(dataReader["entry_date"].ToString());
                ci.PublicationDate = DateTime.Parse(dataReader["entry_date"].ToString());
                ci.LastModified = DateTime.Parse(dataReader["entry_date"].ToString());

                var mni = contentManager.Lifecycle.GetMaster(ci);
                mni.LastModified = DateTime.Parse(dataReader["entry_date"].ToString());

                contentManager.SaveChanges();

                contentManager.Lifecycle.PublishWithSpecificDate(mni, DateTime.Parse(dataReader["entry_date"].ToString()) );
            }
        }


        private void ImportToNews()
        {
            var sqlQuery = @"
								SELECT *
								FROM asfContent
 								WHERE  channel='News' AND status='open'
                                ORDER BY contentId;
							";

            var dbConn = new SqlConnection("Data Source=luke.theforce.local;Initial Catalog=ASF_migration;User ID=rw_asf_migration;Password=Nxgfsd#h877;pooling=true;connection lifetime=120;");
            var sqlComm = new SqlCommand(sqlQuery, dbConn);

            //sqlComm.Parameters.Add(new SqlParameter("@channel", "Articles"));
            sqlComm.Connection.Open();
            var dataReader = sqlComm.ExecuteReader();

            var contentManager = NewsManager.GetManager();
            var taxManager = TaxonomyManager.GetManager();
            var usersManager = RoleManager.GetManager();
            var lm = LibrariesManager.GetManager();

            contentManager.SaveChanges();

            while (dataReader.Read())
            {
                var docType = dataReader["Channel"].ToString();
                Response.Write("<br />----" + dataReader["title"].ToString() + "(doctype " + docType + ") -------<br />"); //Response.Flush();
                //var parent = contentManager.GetBlogs().Where(b => b.UrlName == DoctypeMapping[docType]).Single();
                var ci = contentManager.CreateNewsItem();
                
                ci.Title = TransformTitle(dataReader["title"].ToString());
                if (dataReader["articleText"].ToString() != "null")
                    ci.Content = TransformContent(dataReader["articleText"].ToString());
                //ci.Summary = TransformSumary(importItem.summary);
                //else ci.Summary = TransformContent(importItem.fulltext);
                //var custom_listing = TransformContent(importItem.custom_listing);
                if (dataReader["subTitle"].ToString() != "null") 
                    ci.SetValue("Subtitle", dataReader["subTitle"].ToString());
                if (dataReader["Author"].ToString() != "null")
                    ci.SetValue("Author", dataReader["Author"].ToString());
                if (dataReader["jobTitle"].ToString() != "null")
                    ci.SetValue("JobTitle", dataReader["jobTitle"].ToString());
                if (dataReader["JobRole"].ToString() != "null")
                    ci.SetValue("JobRole", dataReader["JobRole"].ToString());

                if (dataReader["Company"].ToString() != "null")
                    ci.SetValue("Company", dataReader["Company"].ToString());
                if (dataReader["CompanyURL"].ToString() != "null")
                    ci.SetValue("CompanyURL", dataReader["CompanyURL"].ToString());
                if (dataReader["QuarterlyEssential"].ToString() != "null")
                    ci.SetValue("QuarterlyEssential", dataReader["QuarterlyEssential"].ToString());
                if (dataReader["asideImageCaption"].ToString() != "null")
                    ci.SetValue("ImageCaption", dataReader["asideImageCaption"].ToString());
                if (dataReader["asideQuote"].ToString() != "null")
                    ci.SetValue("AsideQuote", dataReader["asideQuote"].ToString());
                if (dataReader["asideAttrib"].ToString() != "null")
                    ci.SetValue("Attribution", dataReader["asideAttrib"].ToString());
                if (dataReader["asideLinkUrl"].ToString() != "null")
                    ci.SetValue("AsideLink", dataReader["asideLinkUrl"].ToString());
                 

                ci.SourceKey = dataReader["contentId"].ToString();
                ci.UrlName = Regex.Replace(ci.Title.ToLower(), DefinitionsHelper.UrlRegularExpressionFilter, "-") + "-" + dataReader["contentId"].ToString();
                ci.PublicationDate = DateTime.Parse(dataReader["entry_date"].ToString());
                ci.LastModified = DateTime.Parse(dataReader["entry_date"].ToString());
                ci.DateCreated = DateTime.Parse(dataReader["entry_date"].ToString());
                contentManager.SaveChanges();

                var categories = new List<string>();


                foreach (var cat in dataReader["categories"].ToString().Split('|'))
                {
                    if (cat.Trim() != "")
                    {
                        var cleanCat = Regex.Replace(cat, @"([\n\t\r]|(1ef8)|(4d0)|(@n[\s|\w]+bsp@))", "", RegexOptions.IgnoreCase).Trim();
                        var taxonId = taxManager.GetTaxa<HierarchicalTaxon>().Where(t => t.UrlName == Regex.Replace(cleanCat, DefinitionsHelper.UrlRegularExpressionFilter, "-")).Single().Id;
                        Response.Write(taxonId + ","); //Response.Flush();
                        categories.Add(cleanCat);
                        try { ci.Organizer.AddTaxa("Category", taxonId); }
                        catch { } // we put this in a try-catch b/c the above mapping may cause dups

                    }
                }
 

                contentManager.SaveChanges();
                var relatedFiles = "";
                var fileUrl = "";
                var documentFieldCounter = 1;
                foreach (var pdfUrl in dataReader["docFile"].ToString().Split('|'))
                {
                    if (pdfUrl.Trim() != ""){
                        var localpdfUrl = pdfUrl.Replace("http://www.smallfoundations.org", "http://phase2.psers.matrixdev.net");
                        Response.Write("<br />----" + pdfUrl + "---<br />"); //Response.Flush();

                        var relatedFileTitle = (!String.IsNullOrEmpty(ci.Title)) ? ci.Title.ToString() : pdfUrl.ToString().Trim();

                        var permissions = new List<string>();
                        if (dataReader["publicFlag"].ToString() != "y")
                        {
                            permissions.Add("Member-Only");
                            permissions.Add("Staff");
                            permissions.Add("Siteadmin");
                        }

                        var fileGuid = ImportNewsletterPDFs(localpdfUrl, dataReader["title"].ToString(), DateTime.Parse(dataReader["entry_date"].ToString()), DoctypeMapping[docType], relatedFileTitle, "", categories.AsQueryable(), permissions.AsQueryable() );
                        fileUrl = lm.GetDocument(fileGuid).Url.Replace("http://", "");
                        fileUrl = ((fileUrl.IndexOf('/') >= 0) ? fileUrl.Substring(fileUrl.IndexOf('/')) : pdfUrl);
                        relatedFiles += String.Format(@"<li><a sfref=""[documents]{0}"" href=""{1}"" title=""{2}"">{2}</a></li>",
                                                        fileGuid,
                            //libraryUrl,
                                                        fileUrl,
                                                        relatedFileTitle
                                                    );
                        if (fileUrl.Trim() != "")
                        {
                            switch (documentFieldCounter)
                            {
                                case 1: ci.SetValue("Document", fileGuid.ToString()); break;
                                case 2: ci.SetValue("Document2", fileGuid.ToString()); break;
                                case 3: ci.SetValue("Document3", fileGuid.ToString()); break;
                            }
                            documentFieldCounter += 1;
                        }
                    }
                }

                foreach (var imgUrl in dataReader["imageFile"].ToString().Split('|'))
                {
                    if (imgUrl.Trim() != "")
                    {
                        var localpdfUrl = imgUrl.Replace("/thumbs/", "/").Replace("_thumb", "");
                        var imageFilename = localpdfUrl.Split('/').Last();
                        var imageGuid = ImportImages(localpdfUrl, imageFilename, DateTime.Now, "news-images", imageFilename.Split('.').First());
               
                        //var lm = LibrariesManager.GetManager();
                        var imageUrl = ((imageGuid != Guid.Empty) ? lm.GetImage(imageGuid).Url.Replace("http://", "") : "/images/missing.jpg");
                        ci.SetValue("Image", imageGuid.ToString());
                    }

                }
                /*
                if (dataReader["publicFlag"].ToString() != "y")
                {
                    var secSetNames = contentManager.SecurityRoot.SupportedPermissionSets;
                    contentManager.BreakPermiossionsInheritance(ci);

                    var roleId = SecurityManager.ApplicationRoles["Everyone"].Id;
                    Telerik.Sitefinity.Security.Model.Permission permissionForEveryone = ci.GetActivePermissions().Where(p => p.PrincipalId == roleId && p.ObjectId == ci.Id && p.SetName == secSetNames[1]).FirstOrDefault();

                    //var newroleId = SecurityManager.ApplicationRoles["Staff"].Id;
                    var role = usersManager.GetRoles().Where(p => p.Name == "Member-Only").SingleOrDefault();
                    if (permissionForEveryone != null)
                    {
                        permissionForEveryone.UngrantActions("ViewBlogPost");
                        contentManager.SaveChanges();
                    }
                    var permissionForAutenticated = contentManager.CreatePermission(SecurityConstants.Sets.General.SetName, ci.Id, role.Id);
                    ci.Permissions.Add(permissionForAutenticated);
                    permissionForAutenticated.GrantActions(false, SecurityConstants.Sets.General.View);
                   
                    contentManager.SaveChanges();

                    role = usersManager.GetRoles().Where(p => p.Name == "Staff").SingleOrDefault();
                    permissionForAutenticated = contentManager.CreatePermission(SecurityConstants.Sets.General.SetName, ci.Id, role.Id);
                    ci.Permissions.Add(permissionForAutenticated);
                    permissionForAutenticated.GrantActions(false, SecurityConstants.Sets.General.View);

                    contentManager.SaveChanges();

                    role = usersManager.GetRoles().Where(p => p.Name == "Siteadmin").SingleOrDefault();
                    permissionForAutenticated = contentManager.CreatePermission(SecurityConstants.Sets.General.SetName, ci.Id, role.Id);
                    ci.Permissions.Add(permissionForAutenticated);
                    permissionForAutenticated.GrantActions(false, SecurityConstants.Sets.General.View);

                    contentManager.SaveChanges();
 
                }
                */
                ci.ApprovalWorkflowState = "PUBLISHED";
                contentManager.Lifecycle.Publish(ci);
                contentManager.SaveChanges();

                ci.DateCreated = DateTime.Parse(dataReader["entry_date"].ToString());
                ci.PublicationDate = DateTime.Parse(dataReader["entry_date"].ToString());
                ci.LastModified = DateTime.Parse(dataReader["entry_date"].ToString());

                var mni = contentManager.Lifecycle.GetMaster(ci);
                mni.LastModified = DateTime.Parse(dataReader["entry_date"].ToString());

                contentManager.SaveChanges();

                contentManager.Lifecycle.PublishWithSpecificDate(mni, DateTime.Parse(dataReader["entry_date"].ToString()) );
            }
        }

        private void ImportToModule()
        {
            var sqlQuery = @"
								SELECT *
								FROM asfContent
 								WHERE  channel='additional resources'  AND status='open'
                               
                                ORDER BY contentId;
							";

            var dbConn = new SqlConnection("Data Source=luke.theforce.local;Initial Catalog=ASF_migration;User ID=rw_asf_migration;Password=Nxgfsd#h877;pooling=true;connection lifetime=120;");
            var sqlComm = new SqlCommand(sqlQuery, dbConn);

            //sqlComm.Parameters.Add(new SqlParameter("@channel", "Articles"));
            sqlComm.Connection.Open();
            var dataReader = sqlComm.ExecuteReader();

            var contentManager =  DynamicModuleManager.GetManager();
            var resourceType = TypeResolutionService.ResolveType("Telerik.Sitefinity.DynamicTypes.Model.AdditionalResources.Resource");
           
            var taxManager = TaxonomyManager.GetManager();
            var usersManager = RoleManager.GetManager();
            var lm = LibrariesManager.GetManager();

            while (dataReader.Read())
            {
                var docType = dataReader["Channel"].ToString();
                Response.Write("<br />----" + dataReader["title"].ToString() + "(doctype " + docType + ") -------<br />"); //Response.Flush();
                
                var ci = contentManager.CreateDataItem(resourceType);

                if (dataReader["title"].ToString() != "null")
                    ci.SetValue("Title", TransformTitle(dataReader["title"].ToString()));
                if (dataReader["link"].ToString() != "null")
                    ci.SetValue("URL", dataReader["link"].ToString());
                if (dataReader["Author"].ToString() != "null")
                    ci.SetValue("ResourceAuthor", dataReader["Author"].ToString());
                if (dataReader["jobTitle"].ToString() != "null")
                    ci.SetValue("AuthorJobTitle", dataReader["jobTitle"].ToString());
                if (dataReader["Company"].ToString() != "null")
                    ci.SetValue("Organization", dataReader["Company"].ToString());
                if (dataReader["CompanyURL"].ToString() != "null")
                    ci.SetValue("OrganizationURL", dataReader["CompanyURL"].ToString());
                if (dataReader["description"].ToString() != "null")
                    ci.SetValue("Content", TransformContent(dataReader["description"].ToString()) );
                ci.UrlName = Regex.Replace(ci.GetValue("Title").ToString().ToLower(), DefinitionsHelper.UrlRegularExpressionFilter, "-") + "-" + dataReader["contentId"].ToString(); ;
                ci.PublicationDate = DateTime.Parse(dataReader["entry_date"].ToString());
                ci.LastModified = DateTime.Parse(dataReader["entry_date"].ToString());
                ci.DateCreated = DateTime.Parse(dataReader["entry_date"].ToString());
                contentManager.SaveChanges();

                var categories = new List<string>();


                foreach (var cat in dataReader["categories"].ToString().Split('|'))
                {
                    if (cat.Trim() != "")
                    {
                        var cleanCat = Regex.Replace(cat, @"([\n\t\r]|(1ef8)|(4d0)|(@n[\s|\w]+bsp@))", "", RegexOptions.IgnoreCase).Trim();
                        var taxonId = taxManager.GetTaxa<HierarchicalTaxon>().Where(t => t.UrlName == Regex.Replace(cleanCat, DefinitionsHelper.UrlRegularExpressionFilter, "-")).Single().Id;
                        Response.Write(taxonId + ","); //Response.Flush();
                        categories.Add(cleanCat);
                        try { ci.Organizer.AddTaxa("Category", taxonId); }
                        catch { } // we put this in a try-catch b/c the above mapping may cause dups

                    }
                }


                contentManager.SaveChanges();
                var relatedFiles = "";
                var fileUrl = "";
                var documentFieldCounter = 1;
                foreach (var pdfUrl in dataReader["docFile"].ToString().Split('|'))
                {
                    if (pdfUrl.Trim() != "")
                    {
                        var localpdfUrl = pdfUrl.Replace("http://www.smallfoundations.org", "http://phase2.psers.matrixdev.net");
                        Response.Write("<br />----" + pdfUrl + "---<br />"); //Response.Flush();

                        var relatedFileTitle = (!String.IsNullOrEmpty(ci.GetValue("Title").ToString())) ? ci.GetValue("Title").ToString() : pdfUrl.ToString().Trim();

                        var permissions = new List<string>();
                        if (dataReader["publicFlag"].ToString() != "y")
                        {
                            permissions.Add("Member-Only");
                            permissions.Add("Staff");
                            permissions.Add("Siteadmin");
                        }

                        var fileGuid = ImportNewsletterPDFs(localpdfUrl, dataReader["title"].ToString(), DateTime.Parse(dataReader["entry_date"].ToString()), DoctypeMapping[docType], relatedFileTitle, "", categories.AsQueryable(), permissions.AsQueryable());
                        fileUrl = lm.GetDocument(fileGuid).Url.Replace("http://", "");
                        fileUrl = ((fileUrl.IndexOf('/') >= 0) ? fileUrl.Substring(fileUrl.IndexOf('/')) : pdfUrl);
                        relatedFiles += String.Format(@"<li><a sfref=""[documents]{0}"" href=""{1}"" title=""{2}"">{2}</a></li>",
                                                        fileGuid,
                            //libraryUrl,
                                                        fileUrl,
                                                        relatedFileTitle
                                                    );
                        if (fileUrl.Trim() != "")
                        {
                            switch (documentFieldCounter)
                            {
                                case 1: ci.AddFile("Documents", fileGuid); break;
                                case 2: ci.AddFile("Document2", fileGuid); break;
                                case 3: ci.AddFile("Document3", fileGuid); break;
                            }
                            documentFieldCounter += 1;
                        }
                    }
                    contentManager.SaveChanges();
                }
                /*
                if (dataReader["publicFlag"].ToString() != "y")
                {
                    var secSetNames = contentManager.SecurityRoot.SupportedPermissionSets;
                    contentManager.BreakPermiossionsInheritance(ci);

                    var roleId = SecurityManager.ApplicationRoles["Everyone"].Id;
                    Telerik.Sitefinity.Security.Model.Permission permissionForEveryone = ci.GetActivePermissions().Where(p => p.PrincipalId == roleId && p.ObjectId == ci.Id && p.SetName == secSetNames[1]).FirstOrDefault();

                    //var newroleId = SecurityManager.ApplicationRoles["Staff"].Id;
                    var role = usersManager.GetRoles().Where(p => p.Name == "Member-Only").SingleOrDefault();
                    if (permissionForEveryone != null)
                    {
                        permissionForEveryone.UngrantActions("ViewBlogPost");
                        contentManager.SaveChanges();
                    }
                    var permissionForAutenticated = contentManager.CreatePermission(secSetNames[1], ci.Id, role.Id);
                    ci.Permissions.Add(permissionForAutenticated);
                    permissionForAutenticated.GrantActions(false, "ViewBlogPost");

                    contentManager.SaveChanges();

                    role = usersManager.GetRoles().Where(p => p.Name == "Staff").SingleOrDefault();
                    permissionForAutenticated = contentManager.CreatePermission(secSetNames[1], ci.Id, role.Id);
                    ci.Permissions.Add(permissionForAutenticated);
                    permissionForAutenticated.GrantActions(false, "ViewBlogPost");

                    contentManager.SaveChanges();

                    role = usersManager.GetRoles().Where(p => p.Name == "Siteadmin").SingleOrDefault();
                    permissionForAutenticated = contentManager.CreatePermission(secSetNames[1], ci.Id, role.Id);
                    ci.Permissions.Add(permissionForAutenticated);
                    permissionForAutenticated.GrantActions(false, "ViewBlogPost");

                    contentManager.SaveChanges();

                }
                */
                ci.ApprovalWorkflowState = "PUBLISHED";
                contentManager.Lifecycle.Publish(ci);
                contentManager.SaveChanges();

                ci.DateCreated = DateTime.Parse(dataReader["entry_date"].ToString());
                ci.PublicationDate = DateTime.Parse(dataReader["entry_date"].ToString());
                ci.LastModified = DateTime.Parse(dataReader["entry_date"].ToString());

                var mni = contentManager.Lifecycle.GetMaster(ci);
                mni.LastModified = DateTime.Parse(dataReader["entry_date"].ToString());

                contentManager.SaveChanges();

                contentManager.Lifecycle.PublishWithSpecificDate(mni, DateTime.Parse(dataReader["entry_date"].ToString()));
            }
        }

        private void FixBlogsPermissions(string urlName)
        {


            var contentManager = BlogsManager.GetManager();
            var taxManager = TaxonomyManager.GetManager();
            var usersManager = RoleManager.GetManager();
            var lm = LibrariesManager.GetManager();

            var parent = contentManager.GetBlogs().Where(b => b.UrlName == urlName).Single();
            var currentPosts = contentManager.GetBlogPosts().Where(item => item.Status == ContentLifecycleStatus.Master && item.Parent.UrlName == urlName).ToList();
            var secSetNames = contentManager.SecurityRoot.SupportedPermissionSets;

            var configManager = Config.GetManager();
            configManager.Provider.SuppressSecurityChecks = true;
            var dataConfig = configManager.GetSection<DataConfig>();
            var Sitefinity = dataConfig.ConnectionStrings["Sitefinity"];
            var connectionString = Sitefinity.ConnectionString;
            //PagesConfig pagesConfig = configManager.GetSection<PagesConfig>();

            var dbConn = new SqlConnection(connectionString);
            dbConn.Open();
            foreach (BlogPost blogPost in currentPosts)
            {
                var roleId = SecurityManager.ApplicationRoles["Everyone"].Id;
                var permissionForEveryone = blogPost.GetActivePermissions().Where(p => p.PrincipalId == roleId && p.ObjectId == blogPost.Id && p.SetName == secSetNames[1]).FirstOrDefault();

                if (permissionForEveryone == null)
                {
                    try
                    {
                        var permissionForAutenticated = contentManager.CreatePermission(secSetNames[1], blogPost.Id, roleId);
                        var sqlQuery = @"
								DELETE
								FROM sf_permissions
 								WHERE  object_id = '" + blogPost.Id.ToString() + "' and principal_id = '" + roleId.ToString() + "'";


                        var sqlComm = new SqlCommand(sqlQuery, dbConn);
                        sqlComm.ExecuteScalar();

                        blogPost.Permissions.Add(permissionForAutenticated);
                        permissionForAutenticated.UngrantActions("ViewBlogPost");
                        contentManager.SaveChanges();
                    }
                    catch { }
                }
            }
            dbConn.Close();
        }

        private void FixPressReleasePost(string urlName)
        {

            var contentManager = DynamicModuleManager.GetManager();
            var resourceType = TypeResolutionService.ResolveType("Telerik.Sitefinity.DynamicTypes.Model.PressReleases.PressRelease");

            var parent = contentManager.GetDataItems(resourceType);
            var currentPosts = contentManager.GetDataItems(resourceType).Where(item => item.Status == ContentLifecycleStatus.Master).ToList();
            var secSetNames = contentManager.SecurityRoot.SupportedPermissionSets;

            var configManager = Config.GetManager();
            configManager.Provider.SuppressSecurityChecks = true;
            var dataConfig = configManager.GetSection<DataConfig>();
            var Sitefinity = dataConfig.ConnectionStrings["Sitefinity"];
            var connectionString = Sitefinity.ConnectionString;
            //PagesConfig pagesConfig = configManager.GetSection<PagesConfig>();


            foreach (var blogPost in currentPosts)
            {


                var sqlQuery = "SELECT TOP 1 * FROM Content WHERE  (title) = @title and categories like '%isPressRelease%'";//and categories = 'isPressRelease'";

                var dbConn = new SqlConnection("Data Source=luke.theforce.local;Initial Catalog=IAFC_migration;User ID=rw_asf_migration;Password=Nxgfsd#h877;pooling=true;connection lifetime=120;");
                dbConn.Open();
                var sqlComm = new SqlCommand(sqlQuery, dbConn);
                var title =  blogPost.GetValue("Title").ToString();
                sqlComm.Parameters.Add(new SqlParameter("@title", title));

                var dataReader = sqlComm.ExecuteReader();





                while (dataReader.Read())
                {

                    Response.Write("<br />----" + dataReader["title"].ToString() + " ( " + dataReader["contentId"].ToString() + " ) PRESS RELEASE-------<br />"); //Response.Flush();


                    if (dataReader["teaser"].ToString() != "" && dataReader["teaser"].ToString().ToUpper() != "NONE")
                    {
                        var cleanText = dataReader["teaser"].ToString();
                        if (cleanText.Length > 255) cleanText = cleanText.Left(250) + " ...";
                        blogPost.SetValue("Summary", cleanText);
                    }
 

                    blogPost.SetValue("LegacyId", dataReader["contentId"].ToString());
                    blogPost.PublicationDate = DateTime.Parse(dataReader["entry_date"].ToString());
                    blogPost.LastModified = DateTime.Parse(dataReader["entry_date"].ToString());
                    blogPost.DateCreated = DateTime.Parse(dataReader["entry_date"].ToString());



                }
               
                contentManager.SaveChanges();
                dbConn.Close();
            }
           
        }

        private void FixOnScenePost(string urlName)
        {

            var contentManager = DynamicModuleManager.GetManager();
            var resourceType = TypeResolutionService.ResolveType("Telerik.Sitefinity.DynamicTypes.Model.OnSceneArticles.OnSceneArticle");

            var parent = contentManager.GetDataItems(resourceType);
            var currentPosts = contentManager.GetDataItems(resourceType).Where(item => item.Status == ContentLifecycleStatus.Master).ToList();
            var secSetNames = contentManager.SecurityRoot.SupportedPermissionSets;

            var configManager = Config.GetManager();
            configManager.Provider.SuppressSecurityChecks = true;
            var dataConfig = configManager.GetSection<DataConfig>();
            var Sitefinity = dataConfig.ConnectionStrings["Sitefinity"];
            var connectionString = Sitefinity.ConnectionString;


            foreach (var blogPost in currentPosts)
            {


                var sqlQuery = "SELECT TOP 1 * FROM Content WHERE  (title) = @title and  not categories like '%isPressRelease%'";//and categories = 'isPressRelease'";

                var dbConn = new SqlConnection("Data Source=luke.theforce.local;Initial Catalog=IAFC_migration;User ID=rw_asf_migration;Password=Nxgfsd#h877;pooling=true;connection lifetime=120;");
                dbConn.Open();
                var sqlComm = new SqlCommand(sqlQuery, dbConn);
                var title = blogPost.GetValue("Title").ToString();
                sqlComm.Parameters.Add(new SqlParameter("@title", title));

                var dataReader = sqlComm.ExecuteReader();





                while (dataReader.Read())
                {

                    Response.Write("<br />----" + dataReader["title"].ToString() + " ( " + dataReader["contentId"].ToString() + " ) ON SCENE -------<br />"); //Response.Flush();


                    if (dataReader["teaser"].ToString() != "" && dataReader["teaser"].ToString().ToUpper() != "NONE")
                    {
                        var cleanText = dataReader["teaser"].ToString();
                        if (cleanText.Length > 255) cleanText = cleanText.Left(250) + " ...";
                        blogPost.SetValue("Summary", cleanText);
                    }


                    blogPost.SetValue("LegacyId", dataReader["contentId"].ToString());
                    blogPost.PublicationDate = DateTime.Parse(dataReader["entry_date"].ToString());
                    blogPost.LastModified = DateTime.Parse(dataReader["entry_date"].ToString());
                    blogPost.DateCreated = DateTime.Parse(dataReader["entry_date"].ToString());



                }

                contentManager.SaveChanges();
                dbConn.Close();
            }

        }

        private void FixResourcePost(string urlName)
        {

            var contentManager = DynamicModuleManager.GetManager();
            var resourceType = TypeResolutionService.ResolveType("Telerik.Sitefinity.DynamicTypes.Model.Resources.Resource");

            var parent = contentManager.GetDataItems(resourceType);
            var currentPosts = contentManager.GetDataItems(resourceType).Where(item => item.Status == ContentLifecycleStatus.Master).ToList();
            var secSetNames = contentManager.SecurityRoot.SupportedPermissionSets;

            var configManager = Config.GetManager();
            configManager.Provider.SuppressSecurityChecks = true;
            var dataConfig = configManager.GetSection<DataConfig>();
            var Sitefinity = dataConfig.ConnectionStrings["Sitefinity"];
            var connectionString = Sitefinity.ConnectionString;
            //PagesConfig pagesConfig = configManager.GetSection<PagesConfig>();


            foreach (var blogPost in currentPosts)
            {


                var sqlQuery = "SELECT TOP 1 * FROM Content WHERE  (title) = @title ";//and categories = 'isPressRelease'";

                var dbConn = new SqlConnection("Data Source=luke.theforce.local;Initial Catalog=IAFC_migration;User ID=rw_asf_migration;Password=Nxgfsd#h877;pooling=true;connection lifetime=120;");
                dbConn.Open();
                var sqlComm = new SqlCommand(sqlQuery, dbConn);
                var title = blogPost.GetValue("Title").ToString();
                sqlComm.Parameters.Add(new SqlParameter("@title", title));

                var dataReader = sqlComm.ExecuteReader();





                while (dataReader.Read())
                {

                    Response.Write("<br />----" + dataReader["title"].ToString() + " ( " + dataReader["contentId"].ToString() + " ) RESOURCE -------<br />"); //Response.Flush();

                    /*
                    if (dataReader["teaser"].ToString() != "" && dataReader["teaser"].ToString().ToUpper() != "NONE")
                    {
                        var cleanText = dataReader["teaser"].ToString();
                        if (cleanText.Length > 255) cleanText = cleanText.Left(250) + " ...";
                        blogPost.SetValue("Summary", cleanText);
                    }
                    */

                    blogPost.SetValue("LegacyId", dataReader["contentId"].ToString());
                    blogPost.PublicationDate = DateTime.Parse(dataReader["entry_date"].ToString());
                    blogPost.LastModified = DateTime.Parse(dataReader["entry_date"].ToString());
                    blogPost.DateCreated = DateTime.Parse(dataReader["entry_date"].ToString());



                }

                contentManager.SaveChanges();
                dbConn.Close();
            }

        }

        private string TransformContent(string content)
        {
            //pull the images from the content
            //content = RewriteImages(content);

            //any custom transforms
            var cleanContent = content;
            //any custom transforms
            if (!String.IsNullOrEmpty(content))
            {
                //strip out patterns
                cleanContent = Regex.Replace(content, @"[<][\/]?font[^>]*[>]", "", RegexOptions.IgnoreCase);

                //simulate paragraphformat() function in CF
               
               //cleanContent = Regex.Replace(content, @"\r\n\r\n", "<p>");
               
               cleanContent = Regex.Replace(cleanContent, @"\r\n", "<br />");
               cleanContent = Regex.Replace(cleanContent, @"[\r|\n]", "<br />");
               /*
              cleanContent = Regex.Replace(cleanContent, @"[\s]{3}", "&nbsp;&nbsp;&nbsp;");
              cleanContent = Regex.Replace(cleanContent, @"[\t]", "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
              */
                cleanContent = Regex.Replace(cleanContent, @"(1ef8)|(4d0)|(@n[\s|\w]+bsp@)", "", RegexOptions.IgnoreCase);
                cleanContent = cleanContent.Replace("�?", "”");
                cleanContent = cleanContent.Replace(@"�""", "—");
                
                cleanContent = Regex.Replace(cleanContent, @"smallfoundations.org", "exponentphilanthropy.org", RegexOptions.IgnoreCase);
                cleanContent = Regex.Replace(cleanContent, @"Association[\s]+of[\s]+Small[\s]+Foundations", "Association of Small Foundations (now Exponent Philanthropy)", RegexOptions.IgnoreCase);
                cleanContent = Regex.Replace(cleanContent, @"ASF", "ASF (now Exponent Philanthropy)", RegexOptions.IgnoreCase);
            }

            return cleanContent;
        }

        private string TransformTitle(string title)
        {
            var cleanTitle = title;
            //any custom transforms
            if (!String.IsNullOrEmpty(title))
            {
                cleanTitle = Regex.Replace(title, @"[<][\/]?[^>]+[>]", @"");
                cleanTitle = Regex.Replace(cleanTitle, @"(1ef8)|(4d0)|(@n[\s|\w]+bsp@)", "", RegexOptions.IgnoreCase);
                cleanTitle = cleanTitle.Replace("�?", "”");
                cleanTitle = cleanTitle.Replace(@"�""", "—");

            }
            if (cleanTitle.Length > 255) cleanTitle = cleanTitle.Left(250) + " ...";
            return cleanTitle;
        }

        private void createNewsletterLibraries()
        {
            var lm = LibrariesManager.GetManager();
            var lib = lm.CreateDocumentLibrary();
            lib.Description = ""; 
            lib.Title = "test";
            lib.Visible = true;
            //lib.Permissions =

            var taxManager = TaxonomyManager.GetManager();
            var taxonId = taxManager.GetTaxa<HierarchicalTaxon>().Where(t => t.UrlName == NewsCategoryTaxonomyMapping[33]).Single().Id;
            lib.Organizer.AddTaxa("Category", taxonId);

        }

        private Guid ImportNewsletterPDFs(string pdfUrl, string filename, DateTime pubdate, string libraryUrlName, string documentTitle, IQueryable<String> newsCategories)
        {
            return (ImportNewsletterPDFs(pdfUrl, filename, pubdate, libraryUrlName, documentTitle, null, newsCategories, null));
        }

        private Guid ImportNewsletterPDFs(string pdfUrl, string filename, DateTime pubdate, string libraryUrlName, string documentTitle, string documentDescription, IQueryable<String> newsCategories)
        {
            return (ImportNewsletterPDFs(pdfUrl, filename, pubdate, libraryUrlName, documentTitle, documentDescription, newsCategories, null));
        }

        private Guid ImportNewsletterPDFs(string pdfUrl, string filename, DateTime pubdate, string libraryUrlName, string documentTitle, string documentDescription, IQueryable<String> newsCategories, IQueryable<String> permissions)
        {

            var lm = LibrariesManager.GetManager();
            var usersManager = RoleManager.GetManager();

            var documentLibrary = lm.GetDocumentLibraries().Where(d => d.UrlName == libraryUrlName).SingleOrDefault();
            var fileUrl = Regex.Replace(filename.ToLower(), DefinitionsHelper.UrlRegularExpressionFilter, "-");
            fileUrl = Regex.Replace(filename.ToLower(), @"[\s]+", "-");
            var fileUrlExt =  fileUrl + "-" + pdfUrl.Split('.').Last();
            var fileExists = lm.GetDocuments().Where(d => d.UrlName == fileUrlExt).FirstOrDefault(i => i.Status == Telerik.Sitefinity.GenericContent.Model.ContentLifecycleStatus.Live);

            if (fileExists != null)
            {
                return fileExists.Id;
            }
            var sfPdf = lm.CreateDocument();
            sfPdf.Parent = documentLibrary;

            sfPdf.Title = TransformTitle(documentTitle);
            if (String.IsNullOrEmpty(sfPdf.Title))
            {
                sfPdf.Title = String.Format(@"{0} - {1}", documentLibrary.Title, pubdate.ToLongDateString());
            }
            sfPdf.DateCreated = pubdate;
            sfPdf.PublicationDate = pubdate;
            sfPdf.LastModified = pubdate;
            sfPdf.UrlName = fileUrl + "-" + pdfUrl.Split('.').Last(); ;
            sfPdf.Extension = "." + pdfUrl.Split('.').Last();
            if (documentDescription != null)
            {
                sfPdf.Description = documentDescription;
            }

            //add taxonomies from Associations and Categories
            var taxManager = TaxonomyManager.GetManager();
            foreach (var newsCategory in newsCategories)
            {
                var taxonId = taxManager.GetTaxa<HierarchicalTaxon>().Where(t => t.UrlName == Regex.Replace(newsCategory, DefinitionsHelper.UrlRegularExpressionFilter, "-")).Single().Id;
                Response.Write(taxonId + ","); //Response.Flush();
                try { sfPdf.Organizer.AddTaxa("Category", taxonId); }
                catch { } // we put this in a try-catch b/c the above mapping may cause dups
            }

            if ((permissions != null) && (permissions.Count() > 0))
            {
                lm.BreakPermiossionsInheritance(sfPdf);
                foreach (var permission in permissions)
                {
                    var role = usersManager.GetRoles().Where(p => p.Name == permission).SingleOrDefault();
                    var currentPerm = sfPdf.GetActivePermissions().Where(p => p.PrincipalId == role.Id && p.ObjectId == sfPdf.Id && p.SetName == SecurityConstants.Sets.Document.SetName).FirstOrDefault();
                    if (currentPerm == null)
                    {
                        var perm = lm.CreatePermission(SecurityConstants.Sets.Document.SetName, sfPdf.Id, role.Id);
                        perm.GrantActions(false, SecurityConstants.Sets.Document.View);
                        sfPdf.Permissions.Add(perm);
                    }
                    else
                    {
                        currentPerm.GrantActions(false, SecurityConstants.Sets.Document.View);
                        //ci.Permissions.Add(perm);
                    }
                    var roleId = SecurityManager.ApplicationRoles["Everyone"].Id;
                    var everyOnePerm = sfPdf.GetActivePermissions().Where(p => p.PrincipalId == roleId && p.ObjectId == sfPdf.Id && p.SetName == SecurityConstants.Sets.Document.SetName).FirstOrDefault();
                    if (everyOnePerm != null) everyOnePerm.UngrantActions(SecurityConstants.Sets.Document.View);
                }
            }
            lm.SaveChanges();
            //get the file
            //try
            //{
            //  Response.Write(" :: " + pdfUrl + " ::"); //Response.Flush();
            var wc = new WebClient();
            wc.Headers.Add(HttpRequestHeader.Cookie, "username=maki; session=7157564; CFID=90236737; CFTOKEN=31881196");
            wc.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.79 Safari/535.11");
            try
            {
                var pdfFile = wc.DownloadData(pdfUrl);
                var pdfFileStream = new MemoryStream(pdfFile, 0, pdfFile.Length);
                //upload the image to SF

                lm.Upload(sfPdf, pdfFileStream, sfPdf.Extension);
                lm.SaveChanges();

                sfPdf.ApprovalWorkflowState = "PUBLISHED";
                lm.Lifecycle.PublishWithSpecificDate(sfPdf, pubdate);
                lm.SaveChanges();
            }
            catch { Response.Write("FILE DOWNLOAD ERROR!!!! " + pdfUrl); }



            //}
            //catch (Exception)
            //{
            //   Response.Write("Upload Failed: " +pdfUrl);
            //}

            return (sfPdf.Id);
        }

        private Guid ImportImages(string pdfUrl, string filename, DateTime pubdate, string libraryUrlName, string documentTitle)
        {

            var lm = LibrariesManager.GetManager();
            var documentLibrary = lm.GetAlbums().Where(d => d.UrlName == libraryUrlName).SingleOrDefault();
            var picTitle = TransformTitle(documentTitle);
            if (String.IsNullOrEmpty(picTitle))
            {
                picTitle = String.Format(@"{0} - {1}", documentLibrary.Title, pubdate.ToLongDateString());
            }
            var picUrl = Regex.Replace(picTitle.ToLower(), @"[^\w\-\!\$\'\(\)\=\@\d_]+", "-");

            var picExists = lm.GetImages().Where(d => d.UrlName == picUrl).FirstOrDefault();

            if (picExists != null)
            {
                return picExists.Id;
            }
            var sfPdf = lm.CreateImage();
            sfPdf.Parent = documentLibrary;


            sfPdf.Title = picTitle;
            sfPdf.DateCreated = pubdate;
            sfPdf.PublicationDate = pubdate;
            sfPdf.LastModified = pubdate;
            sfPdf.UrlName = picUrl;
            sfPdf.Extension = "." + filename.Split('.').Last();



            //get the file
            //try
            //{
            //  Response.Write(" :: " + pdfUrl + " ::"); Response.Flush();
            var wc = new WebClient();
            wc.Headers.Add(HttpRequestHeader.Cookie, "username=maki; session=7157564; CFID=90236737; CFTOKEN=31881196");
            wc.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.79 Safari/535.11");
            byte[] pdfFile = null;
            try
            {
                pdfFile = wc.DownloadData(pdfUrl);
            }
            catch { return new Guid(); }

            var pdfFileStream = new MemoryStream(pdfFile, 0, pdfFile.Length);

            //upload the image to SF

            lm.Upload(sfPdf, pdfFileStream, sfPdf.Extension);
            lm.SaveChanges();

            sfPdf.ApprovalWorkflowState = "PUBLISHED";
            lm.Lifecycle.PublishWithSpecificDate(sfPdf, pubdate);
            lm.SaveChanges();


            //}
            //catch (Exception)
            //{
            //   Response.Write("Upload Failed: " +pdfUrl);
            //}

            return (sfPdf.Id);
        }

        private void ImportTaxonomy()
        {
            var taxManager = TaxonomyManager.GetManager();
            /* For Hierarchical
            var CategoriesTaxonomy = taxManager.GetTaxonomy<HierarchicalTaxonomy>(TaxonomyManager.CategoriesTaxonomyId);
            var newTaxonRoot = taxManager.CreateTaxon<HierarchicalTaxon>();
            newTaxonRoot.Title = "Key Terms";
            newTaxonRoot.Name = "KeyTerms";
            newTaxonRoot.Description = "Words to use for tagging";
            newTaxonRoot.UrlName = "key_terms";
            CategoriesTaxonomy.Taxa.Add(newTaxonRoot);

            var importFile = new StreamReader(@"C:\devsites\fmi\Keytermsforwebsite.csv");
            var line = "";
            Char alphaName = new Char();
            HierarchicalTaxon newTaxonAlpha = null;
            while ((line = importFile.ReadLine()) != null) 
            {
                var taxonTitle = line.Trim();
                if (alphaName != taxonTitle.First())
                {
                    alphaName = taxonTitle.First();
                    newTaxonAlpha = taxManager.CreateTaxon<HierarchicalTaxon>();
                    newTaxonAlpha.Title = alphaName.ToString();
                    newTaxonAlpha.Name = String.Format(@"key_terms_{0}",newTaxonAlpha.Name);
                    newTaxonAlpha.Description = newTaxonAlpha.Title;
                    newTaxonAlpha.UrlName = newTaxonAlpha.Name;
                    newTaxonAlpha.Taxonomy = CategoriesTaxonomy;
                    newTaxonRoot.Subtaxa.Add(newTaxonAlpha); 
                }

                var taxonName = Regex.Replace(taxonTitle, @"[^\w\-\!\$\'\(\)\=\@\d_]+", "-");
                var newTaxon = taxManager.CreateTaxon<HierarchicalTaxon>();
                newTaxon.Title = taxonTitle;
                newTaxon.Name = taxonName;
                newTaxon.Description = taxonTitle;
                newTaxon.UrlName = taxonName;
                newTaxon.Taxonomy = CategoriesTaxonomy;
                newTaxonAlpha.Subtaxa.Add(newTaxon);                
            }

            importFile.Close();

            taxManager.SaveChanges();
            */

            /* For Flat */

            var CategoriesTaxonomy = taxManager.GetTaxonomy<FlatTaxonomy>(TaxonomyManager.TagsTaxonomyId);
            var importFile = new StreamReader(@"C:\devsites\fmi\Keytermsforwebsite.csv");
            var line = "";
            while ((line = importFile.ReadLine()) != null)
            {
                var taxonTitle = line.Trim();
                var taxonName = Regex.Replace(taxonTitle, @"[^\w\-\!\$\'\(\)\=\@\d_]+", "-");
                var newTaxon = taxManager.CreateTaxon<FlatTaxon>();
                newTaxon.Title = taxonTitle;
                newTaxon.Name = taxonName;
                newTaxon.Description = taxonTitle;
                newTaxon.UrlName = String.Format(@"tag-{0}",taxonName);
               // newTaxon.FlatTaxonomy = CategoriesTaxonomy;
                CategoriesTaxonomy.Taxa.Add(newTaxon);
            }

            importFile.Close();

            taxManager.SaveChanges();
        }
    }
}