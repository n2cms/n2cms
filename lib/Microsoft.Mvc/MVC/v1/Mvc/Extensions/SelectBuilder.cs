namespace System.Web.Mvc {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Web;
    using System.Web.Routing;

    public static class SelectBuilder {
        /// <summary>
        /// Creates an HTML Select drop-down based on the passed-in datasource.
        /// </summary>
        /// <param name="dataSource">IEnumerable, IQueryable, DataSet, DataTable, or IDataReader</param>
        /// <param name="htmlName">The name of the control for the page</param>
        /// <param name="textField">The datasource field to use for the the display text</param>
        /// <param name="valueField">The datasource field to use for the the control value</param>
        /// <param name="selectedValue">The value that should be selected</param>
        /// <param name="htmlAttributes">Any attributes you want set on the select tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        /// <param name="selectedValue">The selected value</param>
        /// <returns></returns>
        public static string Select(string htmlName, object dataSource, string textField,
                string valueField, int size, bool multiple, object selectedValue, RouteValueDictionary htmlAttributes) {

            return Select(htmlName, dataSource, textField, valueField, size, multiple, new object[] { selectedValue }, htmlAttributes);
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
        /// <param name="selectedValues">The values that are selected</param>
        /// <param name="htmlAttributes">Dictionary of settings</param>
        /// <returns></returns>
        public static string Select(string htmlName, object dataSource, string textField,
                string valueField, int size, bool multiple, IEnumerable selectedValues, RouteValueDictionary htmlAttributes) {

            htmlAttributes = htmlAttributes ?? new RouteValueDictionary();
            htmlAttributes.Add("textField", textField);
            htmlAttributes.Add("valueField", valueField);

            if (multiple) {
                htmlAttributes.Add("multiple", "multiple");
            }

            if (size > 0) {
                htmlAttributes.Add("size", size.ToString(CultureInfo.InvariantCulture));
            }

            return Select(htmlName, dataSource, selectedValues, htmlAttributes);
        }

        /// <summary>
        /// Creates an HTML Select drop-down based on the passed-in datasource.
        /// </summary>
        /// <param name="dataSource">IEnumerable, IQueryable, DataSet, DataTable, or IDataReader</param>
        /// <param name="htmlName">The name of the control for the page</param>
        /// <param name="selectedValues">The values that are selected</param>
        /// <param name="htmlAttributes">Dictionary of settings</param>
        /// <returns></returns>
        public static string Select(string htmlName, object dataSource, IEnumerable selectedValues, RouteValueDictionary htmlAttributes) {

            //output
            StringBuilder sbOptions = new StringBuilder();
            htmlAttributes = htmlAttributes ?? new RouteValueDictionary();


            string valueField = string.Empty;
            string textField = string.Empty;

            if (htmlAttributes.ContainsKey("valueField")) {
                valueField = htmlAttributes["valueField"].ToString();
                htmlAttributes.Remove("valueField");
            }

            if (htmlAttributes.ContainsKey("textField")) {
                textField = htmlAttributes["textField"].ToString();
                htmlAttributes.Remove("textField");
            }

            Dictionary<object, object> listData = MvcControlDataBinder.SourceToDictionary(dataSource, valueField, textField);

            //loop the source and build the options
            foreach (object key in listData.Keys) {

                //see if one of the settings is a prompt
                if (htmlAttributes.ContainsKey("prompt")) {
                    string prompt = TagBuilder.CreateTag(HtmlTagType.Option, "", new RouteValueDictionary(new { text = htmlAttributes["prompt"].ToString(), value = "" }));
                    sbOptions.Append(prompt);
                    htmlAttributes.Remove("prompt");
                }

                // TODO: Should these be HTML encoded or HTML attribute encoded? Need to review all helper methods that call this.
                string thisText = HttpUtility.HtmlEncode(listData[key].ToString());
                string thisValue = HttpUtility.HtmlEncode(key.ToString());
                bool isSelected = false;

                if (selectedValues != null) {
                    IEnumerator en = selectedValues.GetEnumerator();
                    while (en.MoveNext()) {
                        isSelected = HtmlExtensionUtility.AreEqual(en.Current, thisValue);
                        if (isSelected)
                            break;
                    }
                }

                object attributeSettings = null;
                if (isSelected) {
                    attributeSettings = new { selected = "selected", text = thisText, value = thisValue };
                }
                else {
                    attributeSettings = new { text = thisText, value = thisValue };

                }
                string optionTag = TagBuilder.CreateTag(HtmlTagType.Option, "", new RouteValueDictionary(attributeSettings));

                //some light formatting
                optionTag = Environment.NewLine + "\t" + optionTag;

                sbOptions.Append(optionTag);
            }

            string selectTag = TagBuilder.CreateTag(HtmlTagType.Select, htmlName, htmlAttributes);

            //inject the options
            selectTag = selectTag.Insert(selectTag.IndexOf("</select>", StringComparison.OrdinalIgnoreCase), sbOptions.ToString() + Environment.NewLine);

            return selectTag;
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
        /// <param name="htmlAttributes">Dictionary of settings</param>
        public static string ListBox(string htmlName, object dataSource, int size, bool multiple, string textField,
            string valueField, IEnumerable selectedValues, RouteValueDictionary htmlAttributes) {

            return Select(htmlName, dataSource, textField, valueField, size, multiple, selectedValues, htmlAttributes);
        }
    }
}
