namespace System.Web.Mvc {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Web;
    using System.Web.Routing;

    public static class CheckBoxBuilder {
        public static string CheckBox(string htmlName, string text, string value, bool isChecked, RouteValueDictionary htmlAttributes) {
            htmlAttributes = htmlAttributes ?? new RouteValueDictionary();

            value = HttpUtility.HtmlEncode(value);
            text = HttpUtility.HtmlEncode(text);

            htmlAttributes.Add("value", value);
            htmlAttributes.Add("text", text);

            if (isChecked) {
                htmlAttributes.Add("checked", "checked");
            }

            string checkTag = CheckBox(htmlName, htmlAttributes);
            return checkTag;
        }

        public static string CheckBox(string htmlName, RouteValueDictionary htmlAttributes) {
            htmlAttributes = htmlAttributes ?? new RouteValueDictionary();
            string checkTag = TagBuilder.CreateInputTag(HtmlInputType.CheckBox, htmlName, htmlAttributes);
            return checkTag;
        }
    }
}
