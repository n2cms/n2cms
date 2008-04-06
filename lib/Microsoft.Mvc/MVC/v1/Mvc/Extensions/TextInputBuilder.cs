namespace System.Web.Mvc {
    using System.Collections.Generic;
    using System.Web.Routing;

    public static class TextInputBuilder {
        public static string TextArea(string htmlName, object value, int rows, int cols, RouteValueDictionary htmlAttributes) {

            if (rows == 0)
                rows = 12;
            if (cols == 0)
                cols = 58;

            value = value ?? string.Empty;

            htmlAttributes = htmlAttributes ?? new RouteValueDictionary();
            htmlAttributes.Add("value", HttpUtility.HtmlEncode(value.ToString()));

            htmlAttributes.Add("rows", rows);
            htmlAttributes.Add("cols", cols);

            return TextArea(htmlName, htmlAttributes);
        }

        public static string TextArea(string htmlName, RouteValueDictionary htmlAttributes) {
            htmlAttributes = htmlAttributes ?? new RouteValueDictionary();
            string textAreaTag = TagBuilder.CreateTag(HtmlTagType.TextArea, htmlName, htmlAttributes);
            return textAreaTag;
        }

        public static string Hidden(string htmlName, object value) {
            value = value ?? string.Empty;
            RouteValueDictionary settings = new RouteValueDictionary();
            settings.Add("value", HttpUtility.HtmlEncode(value.ToString()));
            string textTag = TagBuilder.CreateInputTag(HtmlInputType.Hidden, htmlName, settings);

            return textTag;
        }

        public static string TextBox(string htmlName, object value, int size, int maxLength, RouteValueDictionary htmlAttributes) {
            value = value ?? string.Empty;
            htmlAttributes = htmlAttributes ?? new RouteValueDictionary();

            htmlAttributes.Add("value", HttpUtility.HtmlEncode(value.ToString()));

            if (maxLength > 0) {
                htmlAttributes.Add("maxlength", maxLength);
            }
            if (size > 0)
                htmlAttributes.Add("size", size);

            return TextBox(htmlName, htmlAttributes);
        }

        public static string TextBox(string htmlName, RouteValueDictionary htmlAttributes) {
            htmlAttributes = htmlAttributes ?? new RouteValueDictionary();
            string textTag = TagBuilder.CreateInputTag(HtmlInputType.Text, htmlName, htmlAttributes);
            return textTag;
        }

        public static string Password(string htmlName, int size, object value, RouteValueDictionary htmlAttributes) {
            value = value ?? string.Empty;
            htmlAttributes = htmlAttributes ?? new RouteValueDictionary();

            htmlAttributes.Add("value", HttpUtility.HtmlEncode(value.ToString()));

            size = size == 0 ? 20 : size;

            htmlAttributes.Add("size", size);

            string textTag = TagBuilder.CreateInputTag(HtmlInputType.Password, htmlName, htmlAttributes);
            return textTag;
        }
    }
}
