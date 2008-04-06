namespace System.Web.Mvc {
    using System.Collections;
    using System.Collections.Generic;
    using System.Web.Routing;

    public static class SelectExtensions {
        /// <summary>
        /// Creates an HTML Select drop-down based on the passed-in datasource.
        /// </summary>
        /// <param name="dataSource">IEnumerable, IQueryable, DataSet, DataTable, or IDataReader</param>
        /// <param name="htmlName">The name of the control for the page</param>
        public static string Select(this HtmlHelper helper, string htmlName, object dataSource) {
            return Select(helper, htmlName, dataSource, "", "", null, 0, false, null);
        }

        /// <summary>
        /// Creates an HTML Select drop-down based on the passed-in datasource.
        /// </summary>
        /// <param name="dataSource">IEnumerable, IQueryable, DataSet, DataTable, or IDataReader</param>
        /// <param name="htmlName">The name of the control for the page</param>
        /// <param name="selectedValue">The value that should be selected</param>
        public static string Select(this HtmlHelper helper, string htmlName, object dataSource, object selectedValue) {
            return Select(helper, htmlName, dataSource, "", "", selectedValue, 0, false, null);
        }

        /// <summary>
        /// Creates an HTML Select drop-down based on the passed-in datasource.
        /// </summary>
        /// <param name="dataSource">IEnumerable, IQueryable, DataSet, DataTable, or IDataReader</param>
        /// <param name="htmlName">The name of the control for the page</param>
        /// <param name="selectedValue">The value that should be selected</param>
        public static string Select(this HtmlHelper helper, string htmlName, object dataSource, IEnumerable selectedValues) {
            return Select(helper, htmlName, dataSource, "", "", selectedValues, 0, false, null);
        }

        /// <summary>
        /// Creates an HTML Select drop-down based on the passed-in datasource.
        /// </summary>
        /// <param name="dataSource">IEnumerable, IQueryable, DataSet, DataTable, or IDataReader</param>
        /// <param name="htmlName">The name of the control for the page</param>
        /// <param name="textField">The datasource field to use for the the display text</param>
        /// <param name="valueField">The datasource field to use for the the control value</param>
        public static string Select(this HtmlHelper helper, string htmlName, object dataSource, string textField, string valueField) {
            return Select(helper, htmlName, dataSource, textField, valueField, null, 0, false, null);
        }

        /// <summary>
        /// Creates an HTML Select drop-down based on the passed-in datasource.
        /// </summary>
        /// <param name="dataSource">IEnumerable, IQueryable, DataSet, DataTable, or IDataReader</param>
        /// <param name="htmlName">The name of the control for the page</param>
        /// <param name="textField">The datasource field to use for the the display text & value</param>
        public static string Select(this HtmlHelper helper, string htmlName, object dataSource, string textField) {
            return Select(helper, htmlName, dataSource, textField, textField, null, 0, false, null);
        }

        /// <summary>
        /// Creates an HTML Select drop-down based on the passed-in datasource.
        /// </summary>
        /// <param name="dataSource">IEnumerable, IQueryable, DataSet, DataTable, or IDataReader</param>
        /// <param name="htmlName">The name of the control for the page</param>
        /// <param name="textField">The datasource field to use for the the display text</param>
        /// <param name="valueField">The datasource field to use for the the control value</param>
        /// <param name="selectedValue">The value that should be selected</param>
        public static string Select(this HtmlHelper helper, string htmlName, object dataSource, string textField, string valueField, object selectedValue) {
            return Select(helper, htmlName, dataSource, textField, valueField, selectedValue, 0, false, null);
        }

        /// <summary>
        /// Creates an HTML Select drop-down based on the passed-in datasource.
        /// </summary>
        /// <param name="dataSource">IEnumerable, IQueryable, DataSet, DataTable, or IDataReader</param>
        /// <param name="htmlName">The name of the control for the page</param>
        /// <param name="textField">The datasource field to use for the the display text</param>
        /// <param name="valueField">The datasource field to use for the the control value</param>
        /// <param name="selectedValue">The value that should be selected</param>
        public static string Select(this HtmlHelper helper, string htmlName, object dataSource, string textField, string valueField, IEnumerable selectedValues) {
            return Select(helper, htmlName, dataSource, textField, valueField, selectedValues, 0, false, null);
        }

        /// <summary>
        /// Creates an HTML Select drop-down based on the passed-in datasource.
        /// </summary>
        /// <param name="dataSource">IEnumerable, IQueryable, DataSet, DataTable, or IDataReader</param>
        /// <param name="htmlName">The name of the control for the page</param>
        /// <param name="textField">The datasource field to use for the the display text</param>
        /// <param name="valueField">The datasource field to use for the the control value</param>
        /// <param name="selectedValue">The value that should be selected</param>
        /// <param name="htmlAttributes">Any attributes you want set on the select tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        public static string Select(this HtmlHelper helper, string htmlName, object dataSource, string textField,
            string valueField, object selectedValue, int size, bool multiple, object htmlAttributes) {
            return Select(helper, htmlName, dataSource, textField, valueField, new object[] { selectedValue }, size, multiple, htmlAttributes);
        }

        /// <summary>
        /// Creates an HTML Select drop-down based on the passed-in datasource.
        /// </summary>
        /// <param name="dataSource">IEnumerable, IQueryable, DataSet, DataTable, or IDataReader</param>
        /// <param name="htmlName">The name of the control for the page</param>
        /// <param name="textField">The datasource field to use for the the display text</param>
        /// <param name="valueField">The datasource field to use for the the control value</param>
        /// <param name="selectedValue">The value that should be selected</param>
        /// <param name="htmlAttributes">Dictionary of HTML Attribute settings</param>
        public static string Select(this HtmlHelper helper, string htmlName, object dataSource, string textField,
            string valueField, object selectedValue, int size, bool multiple, IDictionary<string, object> htmlAttributes) {
            return Select(helper, htmlName, dataSource, textField, valueField, new object[] { selectedValue }, size, multiple, (htmlAttributes == null) ? new RouteValueDictionary() : new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Creates an HTML Select drop-down based on the passed-in datasource.
        /// </summary>
        /// <param name="dataSource">IEnumerable, IQueryable, DataSet, DataTable, or IDataReader</param>
        /// <param name="htmlName">The name of the control for the page</param>
        /// <param name="textField">The datasource field to use for the the display text</param>
        /// <param name="valueField">The datasource field to use for the the control value</param>
        /// <param name="selectedValue">The value that should be selected</param>
        /// <param name="htmlAttributes">Any attributes you want set on the select tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper", Justification = "Required for Extension Method")]
        public static string Select(this HtmlHelper helper, string htmlName, object dataSource, string textField,
            string valueField, IEnumerable selectedValues, int size, bool multiple, object htmlAttributes) {
            return SelectBuilder.Select(htmlName, dataSource, textField, valueField, size, multiple, selectedValues, new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Creates an HTML Select drop-down based on the passed-in datasource.
        /// </summary>
        /// <param name="dataSource">IEnumerable, IQueryable, DataSet, DataTable, or IDataReader</param>
        /// <param name="htmlName">The name of the control for the page</param>
        /// <param name="textField">The datasource field to use for the the display text</param>
        /// <param name="valueField">The datasource field to use for the the control value</param>
        /// <param name="selectedValue">The value that should be selected</param>
        /// <param name="htmlAttributes">Dictionary of HTML Attribute settings</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper", Justification = "Required for Extension Method")]
        public static string Select(this HtmlHelper helper, string htmlName, object dataSource, string textField,
            string valueField, IEnumerable selectedValues, int size, bool multiple, IDictionary<string, object> htmlAttributes) {
            return SelectBuilder.Select(htmlName, dataSource, textField, valueField, size, multiple, selectedValues, (htmlAttributes == null) ? new RouteValueDictionary() : new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Creates an HTML Select drop-down based on the passed-in datasource.
        /// </summary>
        /// <param name="dataSource">IEnumerable, IQueryable, DataSet, DataTable, or IDataReader</param>
        /// <param name="htmlName">The name of the control for the page</param>
        public static string ListBox(this HtmlHelper helper, string htmlName, object dataSource) {
            return ListBox(helper, htmlName, dataSource, 10, false, "", "", null, false);
        }

        /// <summary>
        /// Creates an HTML Select drop-down based on the passed-in datasource.
        /// </summary>
        /// <param name="dataSource">IEnumerable, IQueryable, DataSet, DataTable, or IDataReader</param>
        /// <param name="htmlName">The name of the control for the page</param>
        /// <param name="size">The number of rows to display on screen</param>
        public static string ListBox(this HtmlHelper helper, string htmlName, object dataSource, int size) {
            return ListBox(helper, htmlName, dataSource, size, false, "", "", null, false);
        }

        /// <summary>
        /// Creates an HTML Select drop-down based on the passed-in datasource.
        /// </summary>
        /// <param name="dataSource">IEnumerable, IQueryable, DataSet, DataTable, or IDataReader</param>
        /// <param name="htmlName">The name of the control for the page</param>
        /// <param name="size">The number of rows to display on screen</param>
        public static string ListBox(this HtmlHelper helper, string htmlName, object dataSource, int size, IEnumerable selectedValues) {
            return ListBox(helper, htmlName, dataSource, size, false, "", "", selectedValues, false);
        }

        /// <summary>
        /// Creates an HTML Select drop-down based on the passed-in datasource.
        /// </summary>
        /// <param name="dataSource">IEnumerable, IQueryable, DataSet, DataTable, or IDataReader</param>
        /// <param name="htmlName">The name of the control for the page</param>
        /// <param name="size">The number of rows to display on screen</param>
        /// <param name="multiple">Whether the user can select multiple values</param>
        public static string ListBox(this HtmlHelper helper, string htmlName, object dataSource, int size, bool multiple) {
            return ListBox(helper, htmlName, dataSource, size, multiple, "", "", null, false);
        }

        /// <summary>
        /// Creates an HTML Select drop-down based on the passed-in datasource.
        /// </summary>
        /// <param name="dataSource">IEnumerable, IQueryable, DataSet, DataTable, or IDataReader</param>
        /// <param name="htmlName">The name of the control for the page</param>
        /// <param name="size">The number of rows to display on screen</param>
        /// <param name="multiple">Whether the user can select multiple values</param>
        public static string ListBox(this HtmlHelper helper, string htmlName, object dataSource, int size, bool multiple, IEnumerable selectedValues) {
            return ListBox(helper, htmlName, dataSource, size, multiple, "", "", selectedValues, false);
        }

        /// <summary>
        /// Creates an HTML ListBox based on the passed-in datasource.
        /// </summary>
        /// <param name="dataSource">IEnumerable, IQueryable, DataSet, DataTable, or IDataReader</param>
        /// <param name="htmlName">The name of the control for the page</param>
        /// <param name="selectedValue">The value that should be selected</param>
        public static string ListBox(this HtmlHelper helper, string htmlName, object dataSource, IEnumerable selectedValues) {
            return ListBox(helper, htmlName, dataSource, 10, false, "", "", selectedValues, null);
        }

        /// <summary>
        /// Creates an HTML ListBox based on the passed-in datasource.
        /// </summary>
        /// <param name="dataSource">IEnumerable, IQueryable, DataSet, DataTable, or IDataReader</param>
        /// <param name="htmlName">The name of the control for the page</param>
        /// <param name="textField">The datasource field to use for the the display text</param>
        /// <param name="valueField">The datasource field to use for the the control value</param>
        public static string ListBox(this HtmlHelper helper, string htmlName, object dataSource, string textField, string valueField) {
            return ListBox(helper, htmlName, dataSource, 10, false, textField, valueField, null, null);
        }

        /// <summary>
        /// Creates an HTML ListBox based on the passed-in datasource.
        /// </summary>
        /// <param name="dataSource">IEnumerable, IQueryable, DataSet, DataTable, or IDataReader</param>
        /// <param name="htmlName">The name of the control for the page</param>
        /// <param name="size">The number of rows to display on screen</param>
        /// <param name="textField">The datasource field to use for the the display text</param>
        /// <param name="valueField">The datasource field to use for the the control value</param>
        public static string ListBox(this HtmlHelper helper, string htmlName, object dataSource, int size, string textField, string valueField) {
            return ListBox(helper, htmlName, dataSource, size, false, textField, valueField, null, null);
        }

        /// <summary>
        /// Creates an HTML ListBox based on the passed-in datasource.
        /// </summary>
        /// <param name="dataSource">IEnumerable, IQueryable, DataSet, DataTable, or IDataReader</param>
        /// <param name="htmlName">The name of the control for the page</param>
        /// <param name="size">The number of rows to display on screen</param>
        /// <param name="multiple">Whether the user can select multiple values</param>
        /// <param name="textField">The datasource field to use for the the display text</param>
        /// <param name="valueField">The datasource field to use for the the control value</param>
        public static string ListBox(this HtmlHelper helper, string htmlName, object dataSource, int size, bool multiple, string textField, string valueField) {
            return ListBox(helper, htmlName, dataSource, size, multiple, textField, valueField, null, null);
        }

        /// <summary>
        /// Creates an HTML ListBox based on the passed-in datasource.
        /// </summary>
        /// <param name="dataSource">IEnumerable, IQueryable, DataSet, DataTable, or IDataReader</param>
        /// <param name="htmlName">The name of the control for the page</param>
        /// <param name="textField">The datasource field to use for the the display text</param>
        /// <param name="valueField">The datasource field to use for the the control value</param>
        /// <param name="selectedValue">The value that should be selected</param>
        public static string ListBox(this HtmlHelper helper, string htmlName, object dataSource, string textField, string valueField, IEnumerable selectedValues) {
            return ListBox(helper, htmlName, dataSource, 10, false, textField, valueField, selectedValues, null);
        }

        /// <summary>
        /// Creates an HTML ListBox based on the passed-in datasource.
        /// </summary>
        /// <param name="htmlName">The name of the control for the page</param>
        /// <param name="dataSource">IEnumerable, IQueryable, DataSet, DataTable, or IDataReader</param>
        /// <param name="size">The number of rows to display on screen</param>
        /// <param name="multiple">Whether the user can select multiple values</param>
        /// <param name="textField">The datasource field to use for the the display text</param>
        /// <param name="valueField">The datasource field to use for the the control value</param>
        /// <param name="selectedValue">The value that should be selected</param>
        /// <param name="htmlAttributes">Any attributes you want set on the select tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper", Justification = "Required for Extension Method")]
        public static string ListBox(this HtmlHelper helper, string htmlName, object dataSource, int size, bool multiple, string textField,
            string valueField, IEnumerable selectedValues, object htmlAttributes) {
            return SelectBuilder.ListBox(htmlName, dataSource, size, multiple, textField, valueField, selectedValues, new RouteValueDictionary(htmlAttributes));
        }
    }
}
