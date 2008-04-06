namespace System.Web.Mvc {
    using System.Collections;
    using System.Collections.Generic;
    using System.Web.Routing;

    public static class CheckBoxExtensions {
        /// <summary>
        /// Creates a checkbox input for your form
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="value">The text for the control</param>
        /// <param name="isChecked">Whether the checkbox is selected</param>
        public static string CheckBox(this HtmlHelper helper, string htmlName, string text, string value, bool isChecked) {
            return CheckBox(helper, htmlName, text, value, isChecked, null);

        }

        /// <summary>
        /// Creates a checkbox input for your form
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="text">The text & value for the control</param>
        public static string CheckBox(this HtmlHelper helper, string htmlName, string text) {
            return CheckBox(helper, htmlName, text, text, false, null);
        }

        /// <summary>
        /// Creates a checkbox input for your form
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="text">The text & value for the control</param>
        /// <param name="value">The text for the control</param>
        public static string CheckBox(this HtmlHelper helper, string htmlName, string text, string value) {
            return CheckBox(helper, htmlName, text, value, false, null);
        }

        /// <summary>
        /// Creates a checkbox input for your form
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="text">The text & value for the control</param>
        /// <param name="isChecked">Whether the checkbox is selected</param>
        public static string CheckBox(this HtmlHelper helper, string htmlName, string text, bool isChecked) {
            return CheckBox(helper, htmlName, text, text, isChecked, null);
        }

        /// <summary>
        /// Creates a checkbox input for your form
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="text">The text for the control</param>
        /// <param name="value">The value for the control</param>
        /// <param name="isChecked">Whether the checkbox is selected</param>
        /// <param name="htmlAttributes">Any attributes you want set on the tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper", Justification = "Required for Extension Method")]
        public static string CheckBox(this HtmlHelper helper, string htmlName, string text, string value, bool isChecked, object htmlAttributes) {
            return CheckBoxBuilder.CheckBox(htmlName, text, value, isChecked, new RouteValueDictionary(htmlAttributes));
        }
    }
}
