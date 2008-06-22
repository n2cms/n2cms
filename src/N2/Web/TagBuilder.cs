using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Web
{
    public class TagBuilder
    {
        string tag;
        IDictionary<string, string> attributes;
        string innerHtml = null;

        public TagBuilder(string tag)
        {
            this.tag = tag;
            attributes = new Dictionary<string, string>();
        }
        public TagBuilder(string tag, string innerHtml)
            :this(tag)
        {
            this.tag = tag;
            this.innerHtml = innerHtml;
        }

        public TagBuilder Attr(string key, string value)
        {
            attributes[key] = value;
            return this;
        }

        public TagBuilder Id(string id)
        {
            Attr("id", id);
            return this;
        }

        public static implicit operator string(TagBuilder tb)
        {
            if (tb == null)
                return null;
            return tb.ToString();
        }

        public override string ToString()
        {
            if (innerHtml == null)
                return null;

            StringBuilder html = new StringBuilder();
            html.Append("<").Append(tag);
            foreach (KeyValuePair<string, string> attribute in attributes)
            {
                html.Append(" ").Append(attribute.Key).Append("=\"").Append(attribute.Value).Append("\"");
            }
            if (string.IsNullOrEmpty(innerHtml))
                html.Append("/>");
            else
                html.Append(">").Append(innerHtml).Append("</").Append(tag).Append(">");
            return html.ToString();
        }
    }
}
