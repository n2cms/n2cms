using System.Web.UI;

namespace N2.Web.UI.WebControls
{
    public class Li : Control
    {
        private string text;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write("<li>");
            if (string.IsNullOrEmpty(Text))
                RenderChildren(writer);
            else
                writer.Write(Text);
            writer.Write("</li>");
        }
    }
}
