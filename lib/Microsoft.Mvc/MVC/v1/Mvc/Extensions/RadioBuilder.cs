namespace System.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web;
    using System.Web.Routing;

    public static class RadioBuilder {
        /// <summary>
        /// Creates an HTML Radio button list based on the passed-in datasource.
        /// </summary>
        /// <param name="dataSource">IEnumerable, IQueryable, DataSet, DataTable, or IDataReader</param>
        /// <param name="htmlName">The name of the control for the page</param>
        /// <param name="textField">The datasource field to use for the the display text</param>
        /// <param name="valueField">The datasource field to use for the the control value</param>
        /// <param name="selectedValue">The value that should be selected</param>
        /// <param name="htmlAttributes">Dictionary of settings.</param>
        public static string[] RadioButtonList(string htmlName, object dataSource, string textField, string valueField, object selectedValue, RouteValueDictionary htmlAttributes) {

            //output
            List<string> radioButtons = new List<string>();
            Dictionary<object, object> listData = MvcControlDataBinder.SourceToDictionary(dataSource, valueField, textField);
            //loop the source
            foreach (object key in listData.Keys) {
                string thisValue = key.ToString();
                string thisText = listData[key] == null ? String.Empty : listData[key].ToString();
                bool isChecked = false;

                isChecked = HtmlExtensionUtility.AreEqual(selectedValue, thisValue);

                string radioButton = RadioButton(htmlName, thisText, thisValue, isChecked, htmlAttributes);

                radioButtons.Add(radioButton);
            }

            return radioButtons.ToArray();
        }

        /// <summary>
        /// Creates a radio button input for your form
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="text">The text for the control</param>
        /// <param name="value">The value for the control</param>
        /// <param name="isChecked">Whether the radio button is selected</param>
        /// <param name="htmlAttributes">Dictionary of settings</param>
        public static string RadioButton(string htmlName, string text, string value, bool isChecked, RouteValueDictionary htmlAttributes) {

            htmlAttributes = htmlAttributes == null ? new RouteValueDictionary() : new RouteValueDictionary(htmlAttributes);
            value = HttpUtility.HtmlEncode(value);
            text = HttpUtility.HtmlEncode(text);
            htmlAttributes.Add("text", text);
            htmlAttributes.Add("value", value);

            if (isChecked)
                htmlAttributes.Add("checked", "checked");

            string radioTag = RadioButton(htmlName, htmlAttributes);
            return radioTag;
        }

        /// <summary>
        /// Creates a radio button input for your form
        /// </summary>
        /// <param name="htmlName">The name and ID of the control</param>
        /// <param name="htmlAttributes">Dictionary of settings</param>
        public static string RadioButton(string htmlName, RouteValueDictionary htmlAttributes) {
            htmlAttributes = htmlAttributes ?? new RouteValueDictionary();
            string radioTag = TagBuilder.CreateInputTag(HtmlInputType.Radio, htmlName, htmlAttributes);
            return radioTag;
        }
    }
}
