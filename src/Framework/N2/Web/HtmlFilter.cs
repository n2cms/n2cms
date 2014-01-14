using System;
using System.Text.RegularExpressions;
using N2.Engine;

namespace N2.Web
{
    [Service]
    public class HtmlFilter
    {
        protected Regex tagExpression = new Regex(@"<(?<slash>/?)\s*?(?<tag>\w+)\s?(?<attr>[^>]*?)(?<close>/>|>|$)");
        protected Regex attrExpression = new Regex(@"(?<attr>\S*?)=(?<val>(""[^""]*"")|('[^']*')|(\S*))");
        protected Regex stripExpression = new Regex(@"<[^>]*>");
        protected Regex cleanUrlExpression = new Regex(@"[?&:<>*/ ]+");
        
        public HtmlFilter()
        {
            SafeTags = new string[]{
                                    "b", "del", "i", "ins", "u", "font", "big", "small", "sub", 
                                    "sup", "h1", "h2", "h3", "h4", "h5", "h6", "cite", "code", 
                                    "em", "s", "strike", "strong", "tt", "var", "div", "center", 
                                    "blockquote", "ol", "ul", "dl", "table", "caption", "pre", 
                                    "ruby", "rt", "rb", "rp", "p", "span", "u", "br", "hr", "li", 
                                    "dt", "dd", "td", "th", "tr", "a", "img"
                                   };
            SafeAttributes = new string[]{
                                            "href", "title", "target", "rel", "src", "alt"
                                         };
        }
        public string[] SafeTags { get; set; }
        public string[] SafeAttributes { get; set; }

        public virtual string StripHtml(string html)
        {
            return stripExpression.Replace(html, string.Empty);
        }

        public virtual string CleanUrl(string text)
        {
            return cleanUrlExpression.Replace(StripHtml(text), "-");
        }

        public virtual string FilterHtml(string html)
        {
            return tagExpression.Replace(html, FilterDangerousTags);
        }

        protected virtual string FilterDangerousTags(Match match)
        {
            string tag = match.Groups["tag"].Value.ToLower();
            bool isSafe = Array.IndexOf(SafeTags, tag) >= 0;
            if (!isSafe)
                return string.Empty;
            return FilterDangerousAttributes(match, tag);
        }

        protected virtual string FilterDangerousAttributes(Match match, string tag)
        {
            string slash = match.Groups["slash"].Value;
            string attributesPortion = match.Groups["attr"].Value;
            string filteredAttributes = string.Empty;
            foreach (Match attrMatch in attrExpression.Matches(attributesPortion))
            {
                string attr = attrMatch.Groups["attr"].Value.ToLower();
                if (Array.IndexOf(SafeAttributes, attr) >= 0)
                {
                    string value = attrMatch.Groups["val"].Value;
                    filteredAttributes += " " + attr + "=" + value;
                }
            }
            string close = match.Groups["close"].Value;
            return "<" + slash + tag + filteredAttributes + close;
        }
    }
}
