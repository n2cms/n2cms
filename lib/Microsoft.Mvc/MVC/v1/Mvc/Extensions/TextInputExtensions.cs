namespace System.Web.Mvc {
    using System.Collections.Generic;
    using System.Web.Routing;

    public static class TextInputExtensions {
        /// <summary>
        /// Creates a scrollable text box for form input
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="value">The text for the control</param>
        public static string TextArea(this HtmlHelper helper, string htmlName, object value) {
            //set defaults for rows/cols
            //using Phi :) ratio cols=3*Phi*rows
            return TextArea(helper, htmlName, value, 12, 58, null);
        }

        /// <summary>
        /// Creates a scrollable text box for form input
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="value">The text for the control</param>
        /// <param name="htmlAttributes">Any attributes you want set on the tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        public static string TextArea(this HtmlHelper helper, string htmlName, object value, object htmlAttributes) {
            //set defaults for rows/cols
            //using Phi :) ratio
            return TextArea(helper, htmlName, value, 12, 58, htmlAttributes);
        }

        /// <summary>
        /// Creates a scrollable text box for form input
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="value">The text for the control</param>
        /// <param name="htmlAttributes">Dictionary of HTML Settings</param>
        public static string TextArea(this HtmlHelper helper, string htmlName, object value, IDictionary<string, object> htmlAttributes) {
            //set defaults for rows/cols
            //using Phi :) ratio
            return TextArea(helper, htmlName, value, 12, 58, htmlAttributes);
        }

        /// <summary>
        /// Creates a scrollable text box for form input
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="value">The text for the control</param>
        /// <param name="rows">The height</param>
        /// <param name="cols">The width</param>
        public static string TextArea(this HtmlHelper helper, string htmlName, object value, int rows, int cols) {
            return TextArea(helper, htmlName, value, rows, cols, null);
        }

        /// <summary>
        /// Creates a scrollable text box for form input
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="value">The text for the control</param>
        /// <param name="rows">The height</param>
        /// <param name="cols">The width</param>
        /// <param name="maxlength">The input limit for the text area</param>
        /// <param name="htmlAttributes">Any attributes you want set on the tag. Use anonymous-type declaration for this: new{_class=cssclass}</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "maxlength", Justification = "Spelling is appropriate for use (HTML reference)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper", Justification = "Required for Extension Method")]
        public static string TextArea(this HtmlHelper helper, string htmlName, object value, int rows, int cols, object htmlAttributes) {
            return TextInputBuilder.TextArea(htmlName, value, rows, cols, new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Creates a scrollable text box for form input
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="value">The text for the control</param>
        /// <param name="rows">The height</param>
        /// <param name="cols">The width</param>
        /// <param name="maxlength">The input limit for the text area</param>
        /// <param name="htmlAttributes">Dictionary of HTML Settings</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "maxlength", Justification = "Spelling is appropriate for use (HTML reference)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper", Justification = "Required for Extension Method")]
        public static string TextArea(this HtmlHelper helper, string htmlName, object value, int rows, int cols, IDictionary<string, object> htmlAttributes) {
            return TextInputBuilder.TextArea(htmlName, value, rows, cols, (htmlAttributes == null) ? new RouteValueDictionary() : new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Creates a text box for form input
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="value">The text for the control</param>
        /// <param name="htmlAttributes">Any attributes you want set on the tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper", Justification = "Required for Extension Method")]
        public static string Hidden(this HtmlHelper helper, string htmlName, object value) {
            return TextInputBuilder.Hidden(htmlName, value);
        }

        /// <summary>
        /// Creates a text box for form input
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="value">The text for the control</param>
        /// <param name="htmlAttributes">Any attributes you want set on the tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        public static string TextBox(this HtmlHelper helper, string htmlName, object value, object htmlAttributes) {
            return TextBox(helper, htmlName, value, 20, 0, htmlAttributes);
        }

        /// <summary>
        /// Creates a text box for form input
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="value">The text for the control</param>
        /// <param name="htmlAttributes">Dictionary of HTML Settings</param>
        public static string TextBox(this HtmlHelper helper, string htmlName, object value, IDictionary<string, object> htmlAttributes) {
            return TextBox(helper, htmlName, value, 20, 0, htmlAttributes);
        }

        /// <summary>
        /// Creates a text box for form input
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        public static string TextBox(this HtmlHelper helper, string htmlName) {
            return TextBox(helper, htmlName, "", 20, 0, null);
        }

        /// <summary>
        /// Creates a text box for form input
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="value">The text for the control</param>
        public static string TextBox(this HtmlHelper helper, string htmlName, object value) {
            return TextBox(helper, htmlName, value, 20, 0, null);
        }

        /// <summary>
        /// Creates a text box for form input
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="size">The size of the textbox</param>
        public static string TextBox(this HtmlHelper helper, string htmlName, int size) {
            return TextBox(helper, htmlName, "", size, 0, null);
        }

        /// <summary>
        /// Creates a text box for form input
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="size">The size of the textbox</param>
        /// <param name="maxlength">The maximum characters allowed in the textbox</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "maxlength", Justification = "Spelling is appropriate for use (HTML reference)")]
        public static string TextBox(this HtmlHelper helper, string htmlName, int size, int maxlength) {
            return TextBox(helper, htmlName, "", size, maxlength, null);
        }

        /// <summary>
        /// Creates a text box for form input
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="value">The text for the control</param>
        /// <param name="size">The size of the textbox</param>
        public static string TextBox(this HtmlHelper helper, string htmlName, object value, int size) {
            return TextBox(helper, htmlName, value, size, 0, null);
        }

        /// <summary>
        /// Creates a text box for form input
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="value">The text for the control</param>
        /// <param name="size">The size of the textbox</param>
        /// <param name="maxlength">The maximum characters allowed in the textbox</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "maxlength", Justification = "Spelling is appropriate for use (HTML reference)")]
        public static string TextBox(this HtmlHelper helper, string htmlName, object value, int size, int maxlength) {
            return TextBox(helper, htmlName, value, size, maxlength, null);
        }

        /// <summary>
        /// Creates a text box for form input
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="value">The text for the control</param>
        /// <param name="size">The size of the textbox</param>
        /// <param name="maxlength">The maximum characters allowed in the textbox</param>
        /// <param name="htmlAttributes">Any attributes you want set on the tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "maxlength", Justification = "Spelling is appropriate for use (HTML reference)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper", Justification = "Required for Extension Method")]
        public static string TextBox(this HtmlHelper helper, string htmlName, object value, int size, int maxlength, object htmlAttributes) {
            return TextInputBuilder.TextBox(htmlName, value, size, maxlength, new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Creates a text box for form input
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="value">The text for the control</param>
        /// <param name="size">The size of the textbox</param>
        /// <param name="maxlength">The maximum characters allowed in the textbox</param>
        /// <param name="htmlAttributes">Dictionary of HTML Settings</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "maxlength", Justification = "Spelling is appropriate for use (HTML reference)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper", Justification = "Required for Extension Method")]
        public static string TextBox(this HtmlHelper helper, string htmlName, object value, int size, int maxlength, IDictionary<string, object> htmlAttributes) {
            return TextInputBuilder.TextBox(htmlName, value, size, maxlength, (htmlAttributes == null) ? new RouteValueDictionary() : new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Creates a password entry field
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="value">The text for the control</param>
        public static string Password(this HtmlHelper helper, string htmlName, object value) {
            return Password(helper, htmlName, 20, value, null);
        }

        /// <summary>
        /// Creates a password entry field
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="size">The size of the textbox</param>
        /// <param name="value">The text for the control</param>
        public static string Password(this HtmlHelper helper, string htmlName, int size, object value) {
            return Password(helper, htmlName, size, value, null);
        }

        /// <summary>
        /// Creates a password entry field
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        public static string Password(this HtmlHelper helper, string htmlName) {
            return Password(helper, htmlName, 20, "", null);
        }

        /// <summary>
        /// Creates a password entry field
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="size">The size of the textbox</param>
        public static string Password(this HtmlHelper helper, string htmlName, int size) {
            return Password(helper, htmlName, size, "", null);
        }

        /// <summary>
        /// Creates a password entry field
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="size">The size of the textbox</param>
        /// <param name="value">The text for the control</param>
        /// <param name="htmlAttributes">Any attributes you want set on the tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper", Justification = "Required for Extension Method")]
        public static string Password(this HtmlHelper helper, string htmlName, int size, object value, object htmlAttributes) {
            return TextInputBuilder.Password(htmlName, size, value, new RouteValueDictionary(htmlAttributes));
        }
    }
}
