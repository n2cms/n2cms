using System.Web.Mvc;

namespace N2.Web.Mvc.Html
{
    public static class TagBuilderExtensions
    {
        public static TagBuilder Html(this TagBuilder builder, string html)
        {
            builder.InnerHtml = html;
            return builder;
        }
        public static TagBuilder Id(this TagBuilder builder, string id)
        {
            builder.Attr("id", id);
            return builder;
        }
        public static TagBuilder Attr(this TagBuilder builder, string name, string value)
        {
            builder.Attributes[name] = value;
            return builder;
        }
    }
}
