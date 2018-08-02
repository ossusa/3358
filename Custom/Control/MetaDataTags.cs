#region 

using System;

#endregion

using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using SitefinityWebApp.Custom.Utilities;
using Telerik.OpenAccess;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.News.Model;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Model;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Web.UI;
using Telerik.Sitefinity.DynamicModules.Web.UI.Frontend;
using Telerik.Sitefinity.DynamicModules.Model;
using MatrixGroup.Sitefinity.Config.AppSettings;

namespace SitefinityWebApp.Custom.Control
{
    /*========================================================================\
     |                HOW TO USE THIS FILE TO ADD META TAGS                   |
     |=====================================================================*//*
     |  Table of Contents:                                                    |
     |------------------------------------------------------------------------|
     | 1. Things You Need To Know                                             |
     | 2. What's the point of doing this?                                     |
     | 3. Adding This to Another Project                                      |
     | 4. These instructions are annoying and in the way.                     |
     |                                                                        |
     |------------------------------------------------------------------------|
     | Things You Need To Know:                                               |
     |------------------------------------------------------------------------|
     | This is to be used as a codebehind. Codebehind is code that's in a     |
     | file separate from the view (aspx file). It performs logic serverside  |
     | that can be used to do cool things on your website. You'll notice that |
     | this code is all by its lonesome, with no aspx file to go with it. You |
     | will need to manually list this as a codebehind for the view           |
     | (template) you want. See the "Adding This To Another Project" section  |
     | for instructions on how to do this.                                    |
     |                                                                        |
     | You also need the Extensions.cs file. It contains an important method  |
     | for us. If you don't see it, grab it from another project and double-  |
     | check that the function names are correct. In older versions of the    |
     | file, the word "existing" was misspelled in some function names as     |
     | "exsisting".                                                           |
     |                                                                        |
     |------------------------------------------------------------------------|
     | What's the point of doing this?                                        |
     |------------------------------------------------------------------------|
     | We use a third-party search engine called "Searchblox". It works by    |
     | reading every page on the website and noting information about it.     |
     | Some of that information is categories that it expects to be in the    |
     | <meta> tags of an HTML document. This file is here to add those tags   |
     | to articles and the like, so that Searchblox can do its job.           |
     |                                                                        |
     |------------------------------------------------------------------------|
     | Adding This to Another Project:                                        |
     |------------------------------------------------------------------------|
     | First, I assume that the project you want to add this to already has a |
     | news widget on a Page. Start by moving this file and Extensions.cs to  |
     | your project. It's easier if you keep the same folder structure, but   |
     | it doesn't really matter if you do or don't, as long as you remember   |
     | change the path in the next set of instructions.                       |
     |   Next, you'll need to add a "hook" into the template you want to add  |
     | meta tags to. To find this template, log into Sitefinity. Head to      |
     | "Pages" and find the page that your articles, news items, etc will be  |
     | contained on. Click on it, find the widget that displays these items,  |
     | and edit it. If your widget is like the one I'm using, the "Edit       |
     | selected template" button will be located in the "Single Item          |
     | Settings" tab. Clicking that button should open up a textarea that     |
     | contains some ASP.NET code. Insert a line at the beginning that looks  |
     | like: <%@ Control Language="C#" inherits="SitefinityWebApp.Custom.     |
     | Control.MetaDataTags" %> . Again, if you've changed the folder         |
     | structure, you should edit this to reflect that.                       |
     |   After that, it should just work. Build the project, load a page up   |
     | and look at the source to confirm it.                                  |
     |                                                                        |
     |------------------------------------------------------------------------|
     | These instructions are annoying and in the way.                        |
     |------------------------------------------------------------------------|
     | Sorry about that! Just trying to be thorough. Please don't delete this |
     | altogether from the file. Just collapse it on the side :)              |
     `-----------------------------------------------------------------------*/

    public class MetaDataTags : UserControl
    {
        /// <summary>
        ///     In-order for this to work with 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            
            var detailsView = this.Controls.OfType<RadListView>().FirstOrDefault();

            if (detailsView != null)
            {
                detailsView.ItemDataBound += metaData_ItemDataBound;
            }
            else
            {
                DynamicDetailContainer dynamicContainer = Controls.OfType<DynamicDetailContainer>().FirstOrDefault();
                if (dynamicContainer != null)
                {
                    var dynamicContents = ((DynamicContent[])(dynamicContainer.DataSource));
                    if (dynamicContents.Length > 0)
                    {
                        DynamicContent dynamicArticle = dynamicContents[0];
                        if (dynamicArticle != null)
                        {
                            AddMetaDataTagsFromArticle(dynamicArticle);
                        }
                    }
                }
            }
        }

        public void metaData_ItemDataBound(object sender, RadListViewItemEventArgs e)
        {
            if (e.Item is RadListViewDataItem)
            {
                var dataItem = ((RadListViewDataItem)e.Item).DataItem;
                var contentItem = dataItem as Content;
                
                #region searchblox tags

                Page.RemoveExistingPublishedTags();
                Page.RemoveExistingModuleTags();

                // get data type and add content type
                var dataType = dataItem.GetType().Name;
                if (dataType == "NewsItem")
                {
                    // add published date tags
                    Page.AddCustomMetaTags("Published", contentItem.PublicationDate.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture));

                    try
                    {
                        if (!AppSettingsUtility.GetValue<string>("MetaDataTags.NewsItem.Name").IsNullOrWhitespace())
                            dataType = AppSettingsUtility.GetValue<string>("MetaDataTags.NewsItem.Name");
                    }
                    catch (Exception ex)
                    {
                    }

                    Page.AddCustomMetaTags("Module", dataType);

                    // get data type and add content type
                    // add categories
                    var taxonomyManager = TaxonomyManager.GetManager();
                    var categoryIds = contentItem.GetValue("Category") as TrackedList<Guid>;
                    if (categoryIds.Any())
                    {
                        var categoryList = string.Join(",", categoryIds.Select(cid => taxonomyManager.GetTaxon(cid).Title));
                        /* This works because the meta tag is allowed to have a
                         * comma-separated list of values to denote multiple
                         * categories. The list will simply be converted to
                         * the correct format.
                         */
                        Page.AddCustomMetaTags("webcategory", categoryList);
                    }

                    NewsItem newsContent = dataItem as NewsItem;

                    if (newsContent != null && newsContent.Author != null)
                    {
                        string author = newsContent.Author.Value;
                        Page.AddCustomMetaTags("Author", author);
                    }
                }
                else if (dataType == "BlogPost")
                {
                    try
                    {
                        if (!AppSettingsUtility.GetValue<string>("MetaDataTags.BlogPost.Name").IsNullOrWhitespace())
                            dataType = AppSettingsUtility.GetValue<string>("MetaDataTags.BlogPost.Name");
                    }
                    catch (Exception ex)
                    {
                    }

                    Page.AddCustomMetaTags("Module", dataType);
                }
                else if (dataType == "Event")
                {
                    try
                    {
                        if (!AppSettingsUtility.GetValue<string>("MetaDataTags.Event.Name").IsNullOrWhitespace())
                            dataType = AppSettingsUtility.GetValue<string>("MetaDataTags.Event.Name");
                    }
                    catch (Exception ex)
                    {
                    }

                    Page.AddCustomMetaTags("Module", dataType);
                }


                #endregion
            }
        }

        private void AddMetaDataTagsFromArticle(DynamicContent dynamicContent)
        {
            Page.RemoveExistingPublishedTags();

            // add published date tags
            Page.AddCustomMetaTags("Published", dynamicContent.PublicationDate.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture));

            TaxonomyManager taxonomyManager = TaxonomyManager.GetManager();
            if (dynamicContent.DoesFieldExist("Category"))
            {
                TrackedList<Guid> categoryIds = dynamicContent.GetValue("Category") as TrackedList<Guid>;
                if (categoryIds != null && categoryIds.Any())
                {
                    var categoryList = string.Join(",", categoryIds.Select(cid => taxonomyManager.GetTaxon(cid).Title));
                    Page.AddCustomMetaTags("webcategory", categoryList);
                }
            }

            if (dynamicContent.DoesFieldExist("resourcetypes"))
            {
                TrackedList<Guid> resourcetypesIds = dynamicContent.GetValue("resourcetypes") as TrackedList<Guid>;
                if (resourcetypesIds != null && resourcetypesIds.Any())
                {
                    var resourcetypesList = string.Join(",", resourcetypesIds.Select(cid => taxonomyManager.GetTaxon(cid).Title));
                    Page.AddCustomMetaTags("resourcetypes", resourcetypesList);
                }
            }

            Page.RemoveExistingModuleTags();
            string modulePage = dynamicContent.GetType().Name;

            try
            {
                if (!AppSettingsUtility.GetValue<string>("MetaDataTags.Article.Name").IsNullOrWhitespace())
                    modulePage = AppSettingsUtility.GetValue<string>("MetaDataTags.Article.Name");
            }
            catch (Exception)
            {
            }

            Page.AddCustomMetaTags("Module", modulePage);


            Type type;

            if (dynamicContent.SystemParentItem != null)
            {
                type = dynamicContent.SystemParentItem.GetType();
            }
            else
            {
                type = dynamicContent.GetType();
            }

            string articleType = type.FullName.Split('.')[4];

            //Could change this to work with On-Scene Articles...
            if (articleType.ToLower().Contains("healthprogress"))
            {
                Page.AddCustomMetaTags("Publication", "Health Progress");
                //Health Progress
            }

            if (dynamicContent.DoesFieldExist("author"))
            {
                var authorname = dynamicContent.Author;
                if (authorname != null)
                {
                    Page.AddCustomMetaTags("ContentAuthor", authorname);
                }
            }
            var authorsFieldName = "organizationalauthors";
            if (dynamicContent.DoesFieldExist(authorsFieldName))
            {
                TrackedList<Guid> authorIds = dynamicContent.GetValue(authorsFieldName) as TrackedList<Guid>;
                if (authorIds != null)
                {
                    var authorList = string.Join(",", authorIds.Select(cid => taxonomyManager.GetTaxon(cid).Title));
                    Page.AddCustomMetaTags("contentauthor", authorList);
                }
            }
        }
    }
}
