using System.Web.UI;

namespace N2.Web.UI.WebControls
{
    /// <summary>
    /// A horizontal ruler.
    /// </summary>
    public class Hr : Control
    {
        public string CssClass
        {
            get { return (string)(ViewState["CssClass"] ?? string.Empty); }
            set { ViewState["CssClass"] = value; }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (CssClass.Length > 0)
                writer.Write("<hr class='" + CssClass + "'/>");
            else
                writer.Write("<hr />");
            
            RenderChildren(writer);
        }
    }
}
