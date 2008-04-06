namespace System.Web.Mvc {
    using System;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Web;
    using System.Web.Routing;
    using System.Web.UI;
    using System.Collections.Generic;

    internal static class HtmlExtensionUtility {
        /// <summary>
        /// Returns equality with type conversion
        /// </summary>
        /// <param name="o1">Object to compare to</param>
        /// <param name="o2">Object being compared</param>
        /// <returns>System.Boolean</returns>
        internal static bool AreEqual(object o1, object o2) {
            bool result = false;

            if (o1 == null && o2 == null) {
                result = true;
            }
            else {
                if (o1 != null) {
                    Type t = o1.GetType();
                    try {
                        o2 = Convert.ChangeType(o2, t, CultureInfo.CurrentCulture);
                    }
                    catch (FormatException x) {
                        throw new FormatException("Can't compare \"" + o1.ToString() + "\" (" + o1.GetType().Name + ") to \"" + o2.ToString() + "\" (" + o2.GetType().Name + ") because the types don't match. Are you comparing the right values?", x);
                    }
                    result = o1.Equals(o2);
                }

            }

            return result;
        }

        /// <summary>
        /// Renders a System.Web.Page and returns it's rendered page
        /// </summary>
        /// <param name="pg">Page to render</param>
        /// <returns>System.String</returns>
        internal static string RenderPage(Page sourcePage) {
            //render
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb, CultureInfo.CurrentCulture);

            HttpContext.Current.Server.Execute(sourcePage, writer, true);

            string result = sb.ToString();
            return result;
        }

        /// <summary>
        /// Sets the properties of an object based on a HashTable
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="settings"></param>
        internal static void SetPropertiesFromDictionary(object instance, RouteValueDictionary settings) {
            foreach (string key in settings.Keys) {
                //find a matching property
                PropertyInfo propInfo = instance.GetType().GetProperty(key, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                if (propInfo != null) {
                    propInfo.SetValue(instance, settings[key], null);
                }
            }
        }

        /// <summary>
        /// Creates a simple {0}="{1}" list based on current object state.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase",
            Justification = "We want the value to be lower case since it is used in HTML.")]
        public static string ConvertObjectToAttributeList(object value) {
            StringBuilder sb = new StringBuilder();
            if (value != null) {
                IDictionary<string, object> d = value as IDictionary<string, object>;
                if (d == null) {
                    d = new RouteValueDictionary(value);
                }

                string resultFormat = "{0}=\"{1}\" ";
                foreach (string attribute in d.Keys) {
                    object thisValue = d[attribute];
                    if (d[attribute] is bool) {
                        thisValue = d[attribute].ToString().ToLowerInvariant();
                    }
                    sb.AppendFormat(resultFormat, attribute.Replace("_", "").ToLowerInvariant(), thisValue);
                }
            }
            return sb.ToString();
        }
    }
}
