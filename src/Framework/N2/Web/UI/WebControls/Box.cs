using System.Web.UI.WebControls;

namespace N2.Web.UI.WebControls
{
    public class Box : Panel
    {
        public Box()
        {
            CssClass = "box";
            InnerCssClass = "inner box-inner";
            HeadingCssClass = "box-heading";
            RenderInnerContainer = true;
        }

        public string HeadingText { get; set; }
        public string HeadingCssClass { get; set; }
        public string InnerCssClass { get; set; }
        public bool RenderInnerContainer { get; set; }

        public override void RenderBeginTag(System.Web.UI.HtmlTextWriter writer)
        {
            base.RenderBeginTag(writer);
            if (!string.IsNullOrEmpty(HeadingText))
                writer.Write("<h4 class=\"" + HeadingCssClass + "\">" + HeadingText + "</h4>");
            if (RenderInnerContainer)
                writer.Write("<div class=\"" + InnerCssClass + "\">");
        }

        public override void RenderEndTag(System.Web.UI.HtmlTextWriter writer)
        {
            if (RenderInnerContainer)
                writer.Write("</div>");
            base.RenderEndTag(writer);
        }
    }
}
