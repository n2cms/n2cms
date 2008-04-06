namespace System.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Web.Routing;

    internal static class TagBuilder {
        private const string INPUT_TAG_FORMAT = "<input type=\"{0}\" {2} />";
        private const string TAG_FORMAT = "<{0} {1} >{2}</{0}>";
        private const string NON_OPEN_TAG_FORMAT = "<{0} {1} {2}/>";
        private const string ATT_FORMAT = " {0}=\"{1}\" ";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase",
            Justification = "We want the value to be lower case since it is used in HTML.")]
        public static string CreateInputTag(HtmlInputType inputType, string name, RouteValueDictionary attributes) {
            string result = string.Empty;
            string tagType = Enum.GetName(typeof(HtmlInputType), inputType).ToLowerInvariant();

            string text = "";

            string htmlID = name;

            //add the name and ID
            attributes.Add("name", name);
            attributes.Add("id", htmlID);


            if (inputType == HtmlInputType.Radio || inputType == HtmlInputType.CheckBox) {
                //have to remove the "text" attribute and place next to the radio button
                if (attributes.ContainsKey("text")) {
                    text = attributes["text"].ToString();
                    attributes.Remove("text");
                }

                //remove the ID for radio
                attributes.Remove("id");
            }

            //send the Disctionary to an attribute list
            string attList = CreateAttributeList(attributes);

            //format it
            result = string.Format(CultureInfo.InvariantCulture, INPUT_TAG_FORMAT, tagType, name, attList);

            //have to rework for Radio and Checkbox
            if (inputType == HtmlInputType.Radio || inputType == HtmlInputType.CheckBox) {
                //add the text to the right of the tag
                // TODO: Why do we need this non-breaking space?
                // TODO: We should be using <labels> instead of just text
                result += "&nbsp;" + text;
            }

            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase",
            Justification = "We want the value to be lower case since it is used in HTML.")]
        public static string CreateTag(HtmlTagType tagType, string name, RouteValueDictionary attributes) {
            string result = string.Empty;
            string tag = Enum.GetName(typeof(HtmlTagType), tagType).ToLowerInvariant();

            if (tagType == HtmlTagType.Mailto || tagType == HtmlTagType.Anchor)
                tag = "a";


            //decide the format
            string formatToUse = TAG_FORMAT;

            //if it's an image, we need to use the non-open tag
            if (tagType == HtmlTagType.Image) {
                formatToUse = NON_OPEN_TAG_FORMAT;
                tag = "img";
            }

            string value = string.Empty;

            //value is sent in as a setting - not explicitly
            //as it's not required in order to build a tag
            if (attributes.ContainsKey("value")) {
                value = attributes["value"].ToString();

                //leave value as an attribute if we're creating an Option tag
                if (tagType != HtmlTagType.Option)
                    attributes.Remove("value");
            }

            //prepare the name and ID
            if (!string.IsNullOrEmpty(name)) {
                attributes.Add("name", name);
                attributes.Add("id", name);
            }

            //if this tag is an Option tag
            //we need to rework it
            //since Value is an attribute
            if (tagType == HtmlTagType.Option) {
                //set the "value" - the thing we show between the tags
                //to the text. Value is an attribute on the option tag
                if (attributes.ContainsKey("text")) {
                    value = attributes["text"].ToString();

                    //remove it so we don't add it to the attributes list
                    attributes.Remove("text");

                }
            }

            //create the attribute list from the Dictionary
            string attList = CreateAttributeList(attributes);

            //format the result
            result = String.Format(CultureInfo.InvariantCulture, formatToUse, tag, attList, value);

            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase",
            Justification = "We want the value to be lower case since it is used in HTML.")]
        private static string CreateAttributeList(RouteValueDictionary attributes) {
            StringBuilder sb = new StringBuilder();
            if (attributes != null) {
                foreach (string key in attributes.Keys) {
                    string attValue = attributes[key].ToString();

                    if (attributes[key] is Boolean)
                        attValue = attValue.ToLowerInvariant();

                    sb.AppendFormat(ATT_FORMAT, key.ToLowerInvariant().Replace("_", ""), attValue);
                }
            }
            return sb.ToString();
        }
    }
}
