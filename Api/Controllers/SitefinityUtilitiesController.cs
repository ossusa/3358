using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Data.Decorators;
using Telerik.Sitefinity.DynamicModules.Builder;
using Telerik.Sitefinity.DynamicModules.Builder.Model;
using Telerik.Sitefinity.Modules.Blogs;
using Telerik.Sitefinity.Modules.Events;
using Telerik.Sitefinity.Modules.Forms;
using Telerik.Sitefinity.Modules.GenericContent;
using Telerik.Sitefinity.Modules.Libraries;
using Telerik.Sitefinity.Modules.Lists;
using Telerik.Sitefinity.Modules.News;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Model;

namespace SitefinityWebApp.Api.Controllers
{
    public class SitefinityUtilitiesController : ApiController
    {
        /* **************************************************************************************************************
        * SitefinityUtilities Api Call
        * By: Steven Meehan
        * Company: Matrix Group International
        * Published: 2016/04/08
        * Last Updated: 2016/04/12
        * Last Updated By: Steven Meehan
        * ***************************************************************************************************************
        * Description: (Targetting version 8.1.5800.0)
        * GetResetPagePermissions: Api call exposed to reset the permissions of the pages in the site when a user is 
        *   unable to modify a pages permissions.
        * GetUpdateDynamicContent: Api call exposed to resetup a dynamic module if it shows as broken
        * GetFixNonAdminBackendUserAfterUpgrade: Specific to version 8.X upgrades fixes an issue where non admin users 
        *   can not log into the back end
        * ***************************************************************************************************************
        * TO-DO's:
        * Add other Sitefinity Utilities as necessary (Targetting version 8.1.5800.0)
        * ***************************************************************************************************************/

        private static Logger log = LogManager.GetCurrentClassLogger();

        //**************Actions**************//
        //***********************************//
        //***********************************//
        #region GetResetPagePermissions

        public List<string> GetResetPagePermissions()
        {
            log.Info("Begin GetResetPagePermissions.");
            PageManager manager = PageManager.GetManager();
            log.Trace("Got an instance of the PageManager.");
            var securityRoot = manager.GetPageNode(SiteInitializer.CurrentFrontendRootNodeId);
            log.Trace("Got the securityRoot, {0}.", securityRoot.Title.Value);
            var rootLevelPages = manager.GetPageNodes().Where(p => p.ParentId == SiteInitializer.CurrentFrontendRootNodeId);
            log.Debug("Retrieved all of the root level pages, {0}.", rootLevelPages.Count());
            var listToReturn = new List<string>();
            log.Trace("Got an instance of a list of strings to return.");

            var i = 1;
            log.Debug("Iterating through the list of rootLevelPages.");
            foreach (var rootLevelPage in rootLevelPages)
            {
                log.Trace("Page {0}: {1}", i, rootLevelPage.Title.Value);

                if (rootLevelPage.InheritsPermissions)
                {
                    log.Debug("The page inherits its permissions.");
                    log.Info("Calling ChangePermissions.");
                    this.ChangePermissions(manager, rootLevelPage, securityRoot);
                    log.Trace(string.Format("The root level page, {0}, has been processed and reset.", rootLevelPage.Title.Value));
                    listToReturn.Add(string.Format("The root level page, {0}, has been processed and reset.", rootLevelPage.Title.Value));
                }

                i++;
            }

            log.Info("Calling ProcessNodes.");
            this.ProcessNodes(rootLevelPages.ToList(), manager);

            log.Info("Finishing GetResetPagePermissions.");
            return (listToReturn);
        }

        #endregion

        #region GetUpdateDynamicContent

        public string GetUpdateDynamicContent(string moduleName)
        {
            log.Info("Begin GetUpdateDynamicContent.");
            var stringToReturn = "";
            log.Trace("Got a string to return.");

            var manager = ModuleBuilderManager.GetManager();
            log.Trace("Got a ModuleBuilderManager.");

            log.Debug("Attempt to get the Dynamic Content.");
            var module = manager.Provider.GetDynamicModules().Where(m => m.Name.ToUpper() == moduleName.ToUpper()).FirstOrDefault();

            if (module != null)
            {
                log.Trace("Module: {0}", module.Title);

                var assembly = Assembly.Load("Telerik.Sitefinity");
                log.Trace("Get the Telerik.Sitefinity assembly.");
                var installerType = assembly.GetType("Telerik.Sitefinity.DynamicModules.Builder.Install.BackendDefinitionInstaller");
                log.Trace("Get the installer Telerik.Sitefinity.DynamicModules.Builder.Install.BackendDefinitionInstaller type.");
                object installer = Activator.CreateInstance(installerType, manager);
                log.Trace("Get the installer.");

                var args = new object[] { module };
                log.Trace("get a new object called args.");
                installerType.InvokeMember("CreateDefaultBackendTypeDefinitionsIfNotExisting", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null, installer, args);
                log.Trace("Run the InvokeMember of the installerType.");

                stringToReturn = stringToReturn + "The code has run successfully. ";
                log.Trace("Append the return string with the success string.");
            }

            log.Trace("Finishing GetUpdateDynamicContent.");
            return stringToReturn + "Process Complete";
        }

        #endregion

        #region GetFixNonAdminBackendUserAfterUpgrade

        public string GetFixNonAdminBackendUserAfterUpgrade()
        {
            log.Trace("");
            var returnString = "";
            log.Trace("");

            AddSupportedPermissionSetsToSecurityRoot<FormsManager>(SecurityConstants.Sets.SitemapGeneration.SetName);
            RemoveSupportedPermissionSetsToSecurityRoot<FormsManager>(SecurityConstants.Sets.Comments.SetName);
            log.Trace("");
            returnString = returnString + "Forms repaired; ";

            AddSupportedPermissionSetsToSecurityRoot<ContentManager>(SecurityConstants.Sets.SitemapGeneration.SetName);
            RemoveSupportedPermissionSetsToSecurityRoot<ContentManager>(SecurityConstants.Sets.Comments.SetName);
            log.Trace("");
            returnString = returnString + "Content repaired; ";

            AddSupportedPermissionSetsToSecurityRoot<LibrariesManager>(
                SecurityConstants.Sets.ImagesSitemapGeneration.SetName,
                SecurityConstants.Sets.DocumentsSitemapGeneration.SetName,
                SecurityConstants.Sets.VideosSitemapGeneration.SetName);
            log.Trace("");
            returnString = returnString + "Library Items repaired; ";

            AddSupportedPermissionSetsToSecurityRoot<BlogsManager>(SecurityConstants.Sets.SitemapGeneration.SetName);
            RemoveSupportedPermissionSetsToSecurityRoot<BlogsManager>(SecurityConstants.Sets.Comments.SetName);
            log.Trace("");
            returnString = returnString + "Blogs repaired; ";

            AddSupportedPermissionSetsToSecurityRoot<EventsManager>(SecurityConstants.Sets.SitemapGeneration.SetName);
            RemoveSupportedPermissionSetsToSecurityRoot<EventsManager>(SecurityConstants.Sets.Comments.SetName);
            log.Trace("");
            returnString = returnString + "Events repaired; ";

            AddSupportedPermissionSetsToSecurityRoot<ListsManager>(
                SecurityConstants.Sets.SitemapGeneration.SetName);
            log.Trace("");
            returnString = returnString + "Lists repaired; ";

            AddSupportedPermissionSetsToSecurityRoot<NewsManager>(SecurityConstants.Sets.SitemapGeneration.SetName);
            RemoveSupportedPermissionSetsToSecurityRoot<NewsManager>(SecurityConstants.Sets.Comments.SetName);

            returnString = returnString + "News repaired; ";
            log.Trace("");
            return returnString;
        }

        #endregion


        //***********Helper Class***********//
        //**********************************//
        //**********************************//
        #region ProcessNodes

        public void ProcessNodes(List<PageNode> pages, PageManager manager)
        {
            log.Info("Begin ProcessNodes.");

            foreach (var page in pages)
            {
                log.Debug("Parent Page: {0}", page.Title.Value);

                var childPages = manager.GetPageNodes().Where(p => p.ParentId == page.Id).ToList();
                log.Trace("Number of Child Pages: {0}", childPages.Count);

                foreach (var childPage in childPages)
                {
                    log.Debug("Child Page: {0}", page.Title.Value);

                    if (childPage.InheritsPermissions)
                    {
                        log.Debug("The page inherits its permissions.");
                        log.Info("Calling ChangePermissions.");
                        this.ChangePermissions(manager, childPage, childPage.Parent);
                        log.Trace(string.Format("The child page, {0}, has been processed and reset.", childPage.Title.Value));
                    }
                }

                log.Info("Calling ProcessNodes.");
                this.ProcessNodes(childPages, manager);
            }

            log.Info("Finishing ProcessNodes.");
        }

        #endregion

        #region ChangePermissions

        public void ChangePermissions(PageManager manager, PageNode childNode, ISecuredObject newParent)
        {
            log.Info("Begin ChangePermissions.");

            manager.Provider.DeletePermissionsInheritanceAssociation(childNode.Parent, childNode);
            log.Debug("Breaking the inheritance of permissions.");

            var decoratorField = manager.Provider.GetType()
                    .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(f => f.Name == "providerDecorator").FirstOrDefault();
            log.Trace("Getting the decorator filed.");

            OpenAccessDecorator decorator = ((OpenAccessDecorator)decoratorField.GetValue(manager.Provider));
            log.Trace("Getting its value.");

            decorator.CreatePermissionInheritanceAssociation(newParent, childNode);
            log.Trace("Setting up the inheritance of permissions");

            childNode.SupportedPermissionSets = newParent.SupportedPermissionSets;
            log.Info("Setting the page's permissions.");

            manager.SaveChanges();
            log.Info("Saving Changes.");

            log.Info("Finishing ChangePermissions.");
        }

        #endregion

        #region AddSupportedPermissionSetsToSecurityRoot

        private void AddSupportedPermissionSetsToSecurityRoot<TManager>(params string[] supportedPermissionSets) where TManager : IManager
        {
            log.Trace("");
            if (supportedPermissionSets == null || supportedPermissionSets.Count() == 0)
            {
                log.Trace("");
                throw new ArgumentNullException("supportedPermissionSets");
            }

            var providers = ManagerBase.GetManager(typeof(TManager)).Providers;
            log.Trace("");

            TManager manager;
            log.Trace("");

            foreach (var provider in providers)
            {
                log.Trace("");

                var securityRoot = this.GetRealSecurityRoot<TManager>(provider.Name, out manager);
                log.Trace("");
                var securityRootSupportedSets = securityRoot.SupportedPermissionSets.ToList<string>();
                log.Trace("");

                foreach (var supportedPermissionSet in supportedPermissionSets)
                {
                    log.Trace("");
                    if (!securityRootSupportedSets.Contains(supportedPermissionSet))
                    {
                        log.Trace("");
                        securityRootSupportedSets.Add(supportedPermissionSet);
                    }
                }

                if (securityRoot.SupportedPermissionSets.Count() < securityRootSupportedSets.Count())
                {
                    log.Trace("");
                    securityRoot.SupportedPermissionSets = securityRootSupportedSets.ToArray<string>();
                    log.Trace("");
                    manager.Provider.CommitTransaction();
                }
            }

            log.Trace("");
        }

        #endregion

        #region RemoveSupportedPermissionSetsToSecurityRoot

        private void RemoveSupportedPermissionSetsToSecurityRoot<TManager>(params string[] supportedPermissionSets) where TManager : IManager
        {
            log.Trace("");

            if (supportedPermissionSets == null || supportedPermissionSets.Count() == 0)
            {
                log.Trace("");
                throw new ArgumentNullException("supportedPermissionSets");
            }

            var providers = ManagerBase.GetManager(typeof(TManager)).Providers;
            log.Trace("");

            TManager manager;
            log.Trace("");

            foreach (var provider in providers)
            {
                log.Trace("");

                var securityRoot = this.GetRealSecurityRoot<TManager>(provider.Name, out manager);
                log.Trace("");

                var securityRootSupportedSets = securityRoot.SupportedPermissionSets.ToList<string>();
                log.Trace("");

                foreach (var supportedPermissionSet in supportedPermissionSets)
                {
                    log.Trace("");

                    if (securityRootSupportedSets.Contains(supportedPermissionSet))
                    {
                        log.Trace("");
                        securityRootSupportedSets.Remove(supportedPermissionSet);
                    }
                }

                if (securityRoot.SupportedPermissionSets.Count() > securityRootSupportedSets.Count())
                {
                    log.Trace("");
                    securityRoot.SupportedPermissionSets = securityRootSupportedSets.ToArray<string>();
                    log.Trace("");
                    manager.Provider.CommitTransaction();
                }
            }
        }

        #endregion

        #region GetRealSecurityRoot

        private SecurityRoot GetRealSecurityRoot<TManager>(string providerName, out TManager manager) where TManager : IManager
        {
            log.Trace("");
            manager = (TManager)ManagerBase.GetManagerInTransaction(typeof(TManager), providerName, "upgrade_view_backend_link_80");
            log.Trace("");
            var securityRootId = manager.Provider.GetSecurityRoot().Id;
            log.Trace("");
            var securityRoot = manager.GetItem(typeof(SecurityRoot), securityRootId) as SecurityRoot;
            log.Trace("");
            log.Trace("");
            return securityRoot;
        }

        #endregion
    }
}