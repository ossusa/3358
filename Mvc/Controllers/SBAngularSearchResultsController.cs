using System;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using SitefinityWebApp.Mvc.Models;

namespace SitefinityWebApp.Mvc.Controllers
{
    /*========================================================================\
     |                 HOW TO MAKE CHANGES TO THIS WIDGET                     |
     |=====================================================================*//*
     |  Table of Contents:                                                    |
     |------------------------------------------------------------------------|
     | 1. Adding More Fields to This Widget                                   |
     | 2. Changing Functionality                                              |
     | 3. Porting this Widget over to another Solution                        |
     | 4. How do I exclude a file from my Solution and what does that mean?   |
     | 5. Help! I'm making changes but nothing's happening!                   |
     | 6. What is a Designer?                                                 |
     | 7. What is Sitefinity Thunder and how do I get it?                     |
     | 8. Where is this widget in Sitefinity?                                 |
     | 9. These instructions are annoying and in the way.                     |
     |                                                                        |
     |------------------------------------------------------------------------|
     | Adding More Fields to This Widget:                                     |
     |------------------------------------------------------------------------|
     | To add the fields you want to this widget, simply follow the examples  |
     | below. In the controller class, add public (usually string) variables  |
     | for each field you want. These will store what the user enters in our  |
     | widget. For them to be properly saved, however, you will also need to  |
     | put the variables in the model.                                        |
     |   Once you have your variables in the right places, you will need to   |
     | make sure that changes here get saved to the model. Edit the Index     |
     | method in this file. Simply use what has already been done as an       |
     | example and substitute in your variable names.                         |
     |   The last place you need to add your variables is to the View, in     |
     | "Default.cshtml". Just add your variable name to the var IAFCSearch,   |
     | following the examples already there.                                  |
     |   Now that you have done this, you will need to exclude the Designer   |
     | for this widget from the project and rename it to something else. Now, |
     | if you do not have the Sitefinity Thunder extension, add it to your    |
     | Visual Studio. If you do not know how to exclude files from your       |
     | solution, don't know what a designer is, or don't know how to add the  |
     | extention, jump to their respective sections below.                    |
     |   The reason that we're doing this is because it is impossible to add  |
     | a field manually. You can add the field in all the correct places, but |
     | it still won't work. We need to use a tool given to us in Sitefinity   |
     | Thunder.                                                               |
     |   Now you'll need to disassociate this widget from its designer.       |
     | Scroll down in this file until you see the class definition. Above     |
     | that you will find a line that specifies what Designer Sitefinity      |
     | should use, enclosed in brackets. Just delete this line and save the   |
     | file.                                                                  |
     |   Right click on WidgetDesigners in the Solution Explorer and select   | 
     | [Add>New Item]. From here, you can select "Designer for Existing       |
     | Widget" and give it the name "SBAngularSearchResultsDesigner.cs".      |
     |   Next you will need to select the Widget you want to be creating a    |
     | Designer for. Just select SBAngularSearchResults.                      |
     |   Continue the process and you'll arrive at a form where you can list  |
     | out every field you want, what you want to label it, and how you want  |
     | to explain it. Since you saved the old version of the designer rather  |
     | than simply deleting it, you can open it up in notepad and copy/paste  |
     | all the old fields' information. Add the information about yours and   |
     | click Done.                                                            |
     |                                                                        |
     |------------------------------------------------------------------------|
     | Changing Functionality:                                                |
     |------------------------------------------------------------------------|
     | If you want to change something on the logic side of things, head over |
     | to [SitefinityWebApp>MvcAssets>SBAngularSearchResults] in the Solution |
     | Explorer and check out the "searchBlox.js" file.                       |
     |                                                                        |
     |------------------------------------------------------------------------|
     | Porting this Widget over to another Solution:                          |
     |------------------------------------------------------------------------|
     | Porting this over is mostly a matter of copying and pasting, but there |
     | are a few things that you need to know to do it without headaches.     |
     | Firstly, check to see if you have an "Mvc" folder in your Solution     |
     | Explorer. If not, add it and its three subfolders "Controllers",       |
     | "Models", and "Views". Now copy over this file into Controllers, its   |
     | sister file in "Models" into "Models", and its sister folder in        |
     | "Views"  into "Views".                                                 |
     |   Now check if you have a folder called "MvcAssets". If you do not,    |
     | add it too. Now copy over the folder "SBAngularSearchResults" from     |
     | the folder in this Solution to the folder you just created.            |
     |   There's no need to copy over the contents of "WidgetDesigners". Just |
     | follow the instructions in the section "Adding More Fields to This     |
     | Widget", pretending like you're adding a field.                        |
     |   If you need to add categories via <meta> tags for the reasons        |
     | described in searchBlox.js, be sure to copy over the files in [Custom> |
     | Control] called "Extensions.cs" and "MetaDataTags.cs". Check out       |
     | MetaDataTags.cs for more information. You may also need to turn on     |
     | "Show All Files".                                                      |
     |                                                                        |
     |                                                                        |
     | Here's a quick checklist for you to double-check:                      |
     |                                                                        |
     | [SitefinityWebApp>Mvc>Controllers>SBAngularSearchResultsController.cs] |
     |                                                                        |
     | [SitefinityWebApp>Mvc>Models>SBAngularSearchResultsModel.cs]           |
     |                                                                        |
     | [SitefinityWebApp>Mvc>Views>SBAngularSearchResults]....(Entire folder) |
     |                                                                        |
     | [SitefinityWebApp>MvcAssets]...........................(Entire folder) |
     |                                                                        |
     | [SitefinityWebApp>App_Data>Sitefinity>WebsiteTemplates>MatrixBase>     |
     |  App_Themes>MatrixBase]................................(Entire folder) |
     |                                                                        |
     | [SitefinityWebApp>Custom>Control>Extensions.cs]                        |
     | [SitefinityWebApp>Custom>Control>MetaDataTags.cs]                      |
     |                                                                        |
     | Not necessary, but I'll put it here anyway (note the wildcard):        |
     |   [SitefinityWebApp>WidgetDesigners>SBAngularSearchResultsDesigner.*]  |
     |                                                                        |
     |------------------------------------------------------------------------|
     | How do I exclude a file from my Solution and what does that mean?      |
     |------------------------------------------------------------------------|
     | A solution contains a lot of files in a folder structure, but          |
     | maintains a list of all the files that it should use when building.    |
     | If a files exists in the folder structure, but is not in that list, it |
     | is excluded from the solution. To purposely exclude a file, right      |
     | click on it in the Solution Explorer and click "Exclude from Project". |
     |   If you want to reinclude a file that you excluded, you will find a   |
     | button at the top of the Solution Explorer that when you hover over it |
     | gives you a tooltip that says "Show All Files". Click it. Now you will |
     | be able to see the file you excluded. Right click on it and select     |
     | "Include in Project".                                                  |
     |                                                                        |
     |------------------------------------------------------------------------|
     | Help! I'm making changes but nothing's happening!                      |
     |------------------------------------------------------------------------|
     | There are two possibilities here, depending on the file type that      |
     | you're editing. If the file is a Javascript or SASS file, you'll need  |
     | to navigate up a folder repeatedly until you find a file called        |
     | "gulpfile.js". Now open up your command line and navigate to that      |
     | folder. Type "gulp". If you get some message about how your your       |
     | console doesn't know what that is, google "gulp installer", follow the |
     | instructions from the gulp website (or github) and try again.          |
     |   If the file type is ascx, cs, or the like, right click on            |
     | "SitefinityWebApp" in the Solution Explorer and select "Build". Wait   |
     | until the build finishes, then try again. It may take a long time to   |
     | load the page after a build, so don't worry if it's not loading right  |
     | away.                                                                  |
     |   If you tried one of those and it still didn't work, make sure you    |
     | saved the file and that you're looking at the correct website. (For    |
     | example, you made local changes, but are looking at the dev site.)     |
     | Another possibility is that you're looking at the Published page       |
     | instead of the Preview page in Sitefinity.                             |
     |                                                                        |
     |------------------------------------------------------------------------|
     | What is a Designer?                                                    |
     |------------------------------------------------------------------------|
     | A designer is a set of instructions that Sitefinity reads to display   |
     | the Edit form of a widget.                                             |
     |                                                                        |
     |------------------------------------------------------------------------|
     | What is Sitefinity Thunder and how do I get it?                        |
     |------------------------------------------------------------------------|
     | Sitefinity Thunder is an extension to Visual Studio made by Sitefinity |
     | themselves. It allows you to easily make features for Sitefinity. You  |
     | can install it by navigating to [Tools>Extensions and Updates>Online]  |
     | and seaching for it in the search box in the upper right-hand corner.  |
     | Click on Sitefinity Thunder in the search results, then click on       |
     | Download. Restart Visual Studio and you're done!                       |
     |                                                                        |
     |------------------------------------------------------------------------|
     | Where is this widget in Sitefinity?                                    |
     |------------------------------------------------------------------------|
     | Log into the Sitefinity backend. Click on "Pages", then the page you   |
     | want to add this widget to. Then scroll down on the right until you    |
     | find a tab called "MvcWidgets". If you've done everything correctly,   |
     | you should see a widget called SBAngular Search Results. Drag it onto  |
     | the page, click Edit and fill out all the relevant information. If you |
     | do not see the folder or the widget, double check that every file is   |
     | in the correct place.                                                  |
     |                                                                        |
     |------------------------------------------------------------------------|
     | These instructions are annoying and in the way.                        |
     |------------------------------------------------------------------------|
     | Sorry about that! Just trying to be thorough. Please don't delete this |
     | altogether from the file. Just collapse it on the side :)              |
     `-----------------------------------------------------------------------*/

    [ControllerToolboxItem(Name = "SBAngularSearchResults", Title = "SBAngular Search Results", SectionName = "MvcWidgets")]
    [Telerik.Sitefinity.Web.UI.ControlDesign.ControlDesigner(typeof(WidgetDesigners.SBAngularSearchResultsDesigner))]
    public class SBAngularSearchResultsController : Controller
    {
        /// <summary>
        ///   String containing a comma-separated series of integers, each
        /// representing a database of crawled information.
        /// </summary>
        [Category("String Properties")]
        public string CollectionId { get; set; }
        
        /// <summary>
        ///   String containing an IP Address or URL pointing to the Searchblox
        /// instance.
        /// </summary>
        [Category("String Properties")]
        public string SearchBloxServerIp { get; set; }

        /// <summary>
        ///   String containing a series of facet names, comma separated.
        /// </summary>
        [Category("String Properties")]
        public string FacetNames {get; set; }

        /// <summary>
        ///   String containing the name of the special date ranges facet.
        /// </summary>
        [Category("String Properties")]
        public string DateRangeName { get; set; }
        
        /// <summary>
        ///   If set to true, will display a search box alongside the search
        /// results.
        /// </summary>
        [Category("Boolean Properties")]
        public bool ShowSearchBox {get; set; }

        /// <summary>
        /// Gets or sets the category limiter.
        /// </summary>
        [Category("String Properties")]
        public string CategoryLimiter { get; set; }

        /// <summary>
        /// Gets or sets the metatag limiter.
        /// </summary>
        [Category("String Properties")]
        public string MetatagLimiter { get; set; }

        /// <summary>
        /// Gets or sets the excluded categories comma-delimited string.
        /// </summary>
        [Category("String Properties")]
        public string ExcludedCategories { get; set; }
        

        /// <summary>
        /// This is the default Action.
        /// </summary>
        public ActionResult Index()
        {
            var model = new SBAngularSearchResultsModel();
            if (string.IsNullOrEmpty(this.CategoryLimiter))
            {
                model.CategoryLimiter = "";
            }
            else
            {
                model.CategoryLimiter = this.CategoryLimiter;
            }

            if (string.IsNullOrEmpty(this.MetatagLimiter))
            {
                model.MetatagLimiter = "";
            }
            else
            {
                model.MetatagLimiter = this.MetatagLimiter;
            }

            if (string.IsNullOrEmpty(this.ExcludedCategories))
            {
                model.ExcludedCategories = "";
            }
            else
            {
                model.ExcludedCategories = this.ExcludedCategories;
            }

            //More compact version
            model.CollectionId = string.IsNullOrEmpty(this.CollectionId) ? "N/A" : this.CollectionId;
            model.SearchBloxServerIp = string.IsNullOrEmpty(this.SearchBloxServerIp) ? "N/A" : this.SearchBloxServerIp;
            model.FacetNames = string.IsNullOrEmpty(this.FacetNames) ? "N/A" : this.FacetNames;
            model.DateRangeName = string.IsNullOrEmpty(this.DateRangeName) ? "N/A" : this.DateRangeName;
            model.ShowSearchBox = this.ShowSearchBox;
            
            return View("Default", model);
        }
    }
}
