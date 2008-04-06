namespace System.Web.Mvc {
    using System.Collections.Generic;
    using System.Web.Routing;

    public static class RadioListExtensions {
        /// <summary>
        /// Creates an HTML Radio button list based on the passed-in datasource.
        /// </summary>
        /// <param name="dataSource">IEnumerable, IQueryable, DataSet, DataTable, or IDataReader</param>
        /// <param name="htmlName">The name of the control for the page</param>
        public static string[] RadioButtonList(this HtmlHelper helper, string htmlName, object dataSource) {
            return RadioButtonList(helper, htmlName, dataSource, "", "", null, null);
        }

        /// <summary>
        /// Creates an HTML Radio button list based on the passed-in datasource.
        /// </summary>
        /// <param name="dataSource">IEnumerable, IQueryable, DataSet, DataTable, or IDataReader</param>
        /// <param name="htmlName">The name of the control for the page</param>
        /// <param name="textField">The datasource field to use for the the display text</param>
        /// <param name="valueField">The datasource field to use for the the control value</param>
        /// <param name="selectedValue">The value that should be selected</param>
        /// <param name="htmlAttributes">Any attributes you want set on the tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        public static string[] RadioButtonList(this HtmlHelper helper, string htmlName, object dataSource, object selectedValue) {
            return RadioButtonList(helper, htmlName, dataSource, "", "", selectedValue, null);
        }

        public static string[] RadioButtonList(this HtmlHelper helper, string htmlName, object dataSource, string textField, string valueField) {
            return RadioButtonList(helper, htmlName, dataSource, textField, valueField, null, null);
        }

        public static string[] RadioButtonList(this HtmlHelper helper, string htmlName, object dataSource, string textField, string valueField, object selectedValue) {
            return RadioButtonList(helper, htmlName, dataSource, textField, valueField, selectedValue, null);
        }

        /// <summary>
        /// Creates an HTML Radio button list based on the passed-in datasource.
        /// </summary>
        /// <param name="dataSource">IEnumerable, IQueryable, DataSet, DataTable, or IDataReader</param>
        /// <param name="htmlName">The name of the control for the page</param>
        /// <param name="textField">The datasource field to use for the the display text</param>
        /// <param name="valueField">The datasource field to use for the the control value</param>
        /// <param name="selectedValue">The value that should be selected</param>
        /// <param name="htmlAttributes">Any attributes you want set on the tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper", Justification = "Required for Extension Method")]
        public static string[] RadioButtonList(this HtmlHelper helper, string htmlName, object dataSource, string textField, string valueField, object selectedValue, object htmlAttributes) {
            return RadioBuilder.RadioButtonList(htmlName, dataSource, textField, valueField, selectedValue, new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Creates an HTML Radio button list based on the passed-in datasource.
        /// </summary>
        /// <param name="dataSource">IEnumerable, IQueryable, DataSet, DataTable, or IDataReader</param>
        /// <param name="htmlName">The name of the control for the page</param>
        /// <param name="textField">The datasource field to use for the the display text</param>
        /// <param name="valueField">The datasource field to use for the the control value</param>
        /// <param name="selectedValue">The value that should be selected</param>
        /// <param name="htmlAttributes">Dictionary of HTML Attribute settings</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper", Justification = "Required for Extension Method")]
        public static string[] RadioButtonList(this HtmlHelper helper, string htmlName, object dataSource, string textField, string valueField, object selectedValue, IDictionary<string, object> htmlAttributes) {
            return RadioBuilder.RadioButtonList(htmlName, dataSource, textField, valueField, selectedValue, (htmlAttributes == null) ? new RouteValueDictionary() : new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Creates a radio button input for your form
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="value">The text for the control</param>
        /// <param name="isChecked">Whether the radio button is selected</param>
        public static string RadioButton(this HtmlHelper helper, string htmlName, string text, string value, bool isChecked) {
            return RadioButton(helper, htmlName, text, value, isChecked, null);
        }

        /// <summary>
        /// Creates a radio button input for your form
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="test">The text & value for the control</param>
        public static string RadioButton(this HtmlHelper helper, string htmlName, string text) {
            return RadioButton(helper, htmlName, text, text, false, null);
        }

        /// <summary>
        /// Creates a radio button input for your form
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="test">The text & value for the control</param>
        /// <param name="value">The text for the control</param>
        public static string RadioButton(this HtmlHelper helper, string htmlName, string text, string value) {
            return RadioButton(helper, htmlName, text, value, false, null);
        }

        /// <summary>
        /// Creates a radio button input for your form
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="value">The text for the control</param>
        /// <param name="isChecked">Whether the radio button is selected</param>
        public static string RadioButton(this HtmlHelper helper, string htmlName, string text, bool isChecked) {
            return RadioButton(helper, htmlName, text, text, isChecked, null);
        }

        /// <summary>
        /// Creates a radio button input for your form
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="text">The text for the control</param>
        /// <param name="value">The value for the control</param>
        /// <param name="isChecked">Whether the radio button is selected</param>
        /// <param name="htmlAttributes">Any attributes you want set on the tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper", Justification = "Required for Extension Method")]
        public static string RadioButton(this HtmlHelper helper, string htmlName, string text, string value, bool isChecked, object htmlAttributes) {
            return RadioBuilder.RadioButton(htmlName, text, value, isChecked, new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Creates a radio button input for your form
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="text">The text for the control</param>
        /// <param name="value">The value for the control</param>
        /// <param name="isChecked">Whether the radio button is selected</param>
        /// <param name="htmlAttributes">Dictionary of HTML Attribute settings</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper", Justification = "Required for Extension Method")]
        public static string RadioButton(this HtmlHelper helper, string htmlName, string text, string value, bool isChecked, IDictionary<string, object> htmlAttributes) {
            return RadioBuilder.RadioButton(htmlName, text, value, isChecked, (htmlAttributes == null) ? new RouteValueDictionary() : new RouteValueDictionary(htmlAttributes));
        }
    }
}
