using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace N2.Web.UI.WebControls
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "A")]
    public class A : HtmlAnchor
    {
        public A()
        {
        }
        public A(ILink destination)
        {
            Destination = destination;
        }
        public A(string href, string target, string title, string contents, string cssClass)
        {
            HRef = href;
            Target = target;
            Title = title;
            Contents = contents;
            if(!string.IsNullOrEmpty(cssClass))
                Attributes["class"] = cssClass;
        }

        public ILink Destination
        {
            set
            {
                HRef = value.Url;
                Target = value.Target;
                Title = value.ToolTip;
                Contents = value.Contents;
            }
        }

        public string Contents
        {
            set
            {
                Controls.Clear();
                if(!string.IsNullOrEmpty(value))
                    Controls.Add(new LiteralControl(value));
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (Controls.Count == 0)
            {
                // nothing
            }
            else if (string.IsNullOrEmpty(HRef))
            {
                writer.Write("<span");
                RenderAttributes(writer);
                writer.Write(">");
                RenderChildren(writer);
                writer.Write("</span>");
            }
            else
            {
                base.Render(writer);
            }
        }

    }
}
