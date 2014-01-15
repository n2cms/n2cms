using System.Web.UI.WebControls;

namespace N2.Templates.Mvc.Details.Controls
{
    public class Box : Panel
    {
        public Box()
        {
            this.CssClass = "box";
        }

        public override void RenderBeginTag(System.Web.UI.HtmlTextWriter writer)
        {
            base.RenderBeginTag(writer);
            writer.Write("<div class='inner'>");
        }

        public override void RenderEndTag(System.Web.UI.HtmlTextWriter writer)
        {
            writer.Write("</div>");
            base.RenderEndTag(writer);
        }
    }
}
