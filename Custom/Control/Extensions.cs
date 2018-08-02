
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Telerik.Sitefinity.Model.ContentLinks;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Model;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Taxonomies.Model;

namespace SitefinityWebApp.Custom.Utilities
{
    public static class Extensions
    {
        /// <summary>
        /// Caution: uses reflection. Also, only use in PreRender events - the controls won't have values until then
        /// </summary>
        /// <param name="page"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetWidget(this Page page, Type type)
        {
            var sfPageProperty = page.GetType().GetProperty("Page");
            var sfPage = sfPageProperty.GetValue(page, null);

            var widgetField = sfPage.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(f => f.FieldType == type);

            return widgetField == null ? null : widgetField.GetValue(sfPage);
        }

        public static string TrimEnd(this string target, string trimString)
        {
            string result = target;
            while (result.EndsWith(trimString))
            {
                result = result.Substring(0, result.Length - trimString.Length);
            }

            return result;
        }

        public static string LeftLimit(this string Input, int Length)
        {
            if (!String.IsNullOrEmpty(Input))
            {
                return Input.Substring(0, (Input.Length > Length) ? Length : Input.Length);
            }
            return "";
        }

        public static bool IsSecured(this ISecuredObject secObj)
        {
            //borrowed from Telerik.Sitefinity.Security.Data.Linq.SitefinityQuery.Get
            var everyoneId = SecurityManager.ApplicationRoles["Everyone"].Id;
            return !(((secObj.InheritsPermissions && !secObj.Permissions.Any<Permission>(p => (((((p.Deny % 2) != 0) && (p.ObjectId != secObj.Id))) && p.PrincipalId == everyoneId))) && secObj.Permissions.Any<Permission>(p => ((((p.Grant >= 0) && (p.ObjectId != secObj.Id))) && (p.PrincipalId) == everyoneId))) || ((!secObj.InheritsPermissions && !secObj.Permissions.Any<Permission>(p => (((((p.Deny % 2) != 0) && (p.ObjectId == secObj.Id))) && p.PrincipalId == everyoneId))) && secObj.Permissions.Any<Permission>(p => ((((p.Grant >= 0) && (p.ObjectId == secObj.Id))) && p.PrincipalId == everyoneId))));
        }

        private static IEnumerable<KeyValuePair<string, object>> GetKvpForObjectValues(this object request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            // Get all properties on the object
            var properties = request.GetType().GetProperties()
                .Where(x => x.CanRead)
                .Where(x => x.GetValue(request, null) != null)
                .ToDictionary(x => x.Name, x => x.GetValue(request, null));

            var propertiesToPass = new List<KeyValuePair<string, object>>();

            foreach (var kvp in properties)
            {
                var valueType = kvp.Value.GetType();

                if (valueType.IsPrimitive || valueType == typeof(string) || valueType == typeof(Guid) || valueType == typeof(DateTime))
                {
                    propertiesToPass.Add(kvp);
                    continue;
                }
                if (kvp.Value is IEnumerable)
                {
                    var valueElemType = valueType.IsGenericType
                                    ? valueType.GetGenericArguments()[0]
                                    : valueType.GetElementType();

                    if (valueElemType != null && (valueElemType.IsPrimitive || valueElemType == typeof(string) || valueElemType == typeof(Guid)))
                    {
                        var enumerable = kvp.Value as IEnumerable;
                        foreach (var value in enumerable)
                        {
                            propertiesToPass.Add(new KeyValuePair<string, object>(kvp.Key, value));
                        }
                    }
                }
                else //RECURSION!!
                {
                    propertiesToPass.AddRange(kvp.Value.GetKvpForObjectValues());
                }
            }

            return propertiesToPass;
        }

        public static string ToQueryString(this object request, string separator = ",")
        {
            var propertiesToPass = request.GetKvpForObjectValues();

            return String.Join("&", propertiesToPass
                    .Select(x => String.Concat(
                        Uri.EscapeDataString(x.Key), "=",
                        Uri.EscapeDataString(x.Value != null ? x.Value.ToString() : String.Empty))));
        }

        public static bool HasValues(this object obj)
        {
            //this is a bit of a hack, but damnit, it works.
            return !String.IsNullOrEmpty(obj.ToQueryString());
        }

        public static string Action(this UrlHelper url, string actionName, object routeValues, bool asQueryString)
        {
            return "";//asQueryString ? Mvc.ActionUrl(actionName, routeValues) : url.Action(actionName, routeValues);
        }

        public static MvcHtmlString FormatIfNotEmpty(this HtmlHelper html, string format, params object[] args)
        {
            var formattedString = args.All(a => a == null || String.IsNullOrEmpty(a.ToString())) ? String.Empty : String.Format(format, args);
            return new MvcHtmlString(formattedString);
        }

        public static MvcHtmlString ShowMessageOnEmpty(this HtmlHelper html, string value, string emptyMessage)
        {
            return new MvcHtmlString(String.IsNullOrWhiteSpace(value) ? emptyMessage : value);
        }

        public static string RenderRazorViewToString(this Controller controller, string viewName, object model)
        {
            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public static string ToParagraphFormat(this string str)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            str = HttpUtility.HtmlEncode(str);

            var sb = new StringBuilder();
            sb.Append("<p>");
            sb.Append(str);
            sb.Replace("\r", String.Empty);
            sb.Replace("\n\n", "</p><p>");
            sb.Replace("\n", "<br />");
            sb.Replace("<p></p>", String.Empty);
            sb.Replace("</p><p>", "</p>" + Environment.NewLine + "<p>");
            sb.Replace("<br />", "<br />" + Environment.NewLine);
            sb.Append("</p>");

            return sb.ToString();
        }

        public static string ToPhoneFormat(this string str, string format = "###-###-####")
        {
            if (str == null)
                return str;

            var digits = Convert.ToInt64(Regex.Replace(str, @"[^\d]", ""));

            return String.Format("{0:" + format + "}", digits);

        }

        public static string ToSerialCommaDelimitedString(this IEnumerable<object> items)
        {
            //http://en.wikipedia.org/wiki/Serial_comma (aka: the oxford comma)
            var enumerable = items.ToArray().Select(i => i.ToString()).ToArray();
            var count = enumerable.Count();

            if (count == 0) return String.Empty;

            if (count == 1)
            {
                return enumerable.First();
            }
            if (count == 2)
            {
                return String.Join(" and ", enumerable);
            }

            return String.Join(", ", enumerable.Take(count - 1)) + ", and " + enumerable.Last();
        }

        public static string ToCommaDelimitedString(this object obj, Guid taxonomyId)
        {
            var taxonIds = (IList<Guid>) obj;
            var taxa = TaxonomyManager.GetManager().GetTaxonomy(taxonomyId).Taxa.Where(t => taxonIds.Contains(t.Id)).Select(t => t.Title.ToString());
            return taxa.ToSerialCommaDelimitedString();
        }

        public static IEnumerable<HierarchicalTaxon> FlattenHierarchy(this HierarchicalTaxon parent)
        {
            yield return parent;
            foreach (var control in parent.Subtaxa)
            {
                yield return control;
                foreach (var descendant in control.FlattenHierarchy())
                {
                    yield return descendant;
                }
            }
        }

        public static Uri ReplaceParameter(this Uri uri, string parameterName, object replacementValue)
        {
            var nvp = HttpUtility.ParseQueryString(uri.Query); 
            nvp.Set(parameterName, replacementValue.ToString());
            return new Uri(uri.GetLeftPart(UriPartial.Path) + "?" + nvp);
        }

        /// <summary>
        /// Adds a HtmlMeta element to this Page's head given the name and content.
        /// Optioanlly uses "property" attribute instead of "name" for Open Graph per the spec.
        /// Make sure to include the namespace decliration if yu use this (e.g. <html prefix="og: http://ogp.me/ns#">)
        /// </summary>
        /// <param name="key">The Name attribute for the meta tag</param>
        /// <param name="value">The Content attribute for the meta tag</param>
        /// <param name="page">The page </param>
        /// <param name="forOpenGraph">If true, change the name of the Name attribute to Property (leaving the value the same)</param>
        public static void AddCustomMetaTags(this Page page, string key, string value, bool forOpenGraph = false)
        {
            HtmlMeta meta = new HtmlMeta();
            if (forOpenGraph)
            {
                meta.Attributes.Add("property", key.ToLower());
            }
            else
            {
                meta.Name = key.ToLower();
            }
            meta.Content = value;
            page.Header.Controls.Add(meta);
        }

        /// <summary>
        /// Removes any exsisting meta tags on the page with a name of "published". Note this does not include 
        /// "og:article:published_time" tags. This is intended to override a published date set by the page
        /// this widget is on with the published date of this widget's content item.
        /// </summary>
        public static void RemoveExistingPublishedTags(this Page page)
        {
            List<HtmlMeta> publishedMetaControls = new List<HtmlMeta>();
            foreach (HtmlMeta metaControl in page.Header.Controls.OfType<HtmlMeta>())
            {
                if (metaControl.Name.ToLower() == "published")
                {
                    publishedMetaControls.Add(metaControl);
                }
            }

            foreach (HtmlMeta metaControl in publishedMetaControls)
            {
                page.Header.Controls.Remove(metaControl);
            }
        }
        public static void RemoveExistingModuleTags(this Page page)
        {
            List<HtmlMeta> publishedMetaControls = new List<HtmlMeta>();
            foreach (HtmlMeta metaControl in page.Header.Controls.OfType<HtmlMeta>())
            {
                if (metaControl.Name.ToLower() == "module")
                {
                    publishedMetaControls.Add(metaControl);
                }
            }

            foreach (HtmlMeta metaControl in publishedMetaControls)
            {
                page.Header.Controls.Remove(metaControl);
            }
        }
    }
}
