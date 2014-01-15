using System.Web.UI;
using N2.Templates.Web.UI.WebControls;
using N2.Web;

namespace N2.Templates.Mvc.Details.Controls
{
    public class Path : Control
    {
        public string SeparatorText
        {
            get { return (string)(ViewState["SeparatorText"] ?? " / "); }
            set { ViewState["SeparatorText"] = value; }
        }

        /// <summary>The page depth level below which the path is hidden from view. This is used to hide the path on the start page.</summary>
        public int VisibilityLevel { get; set; }

        protected override void CreateChildControls()
        {
            int added = 0;
            foreach (ContentItem page in Find.EnumerateParents(Find.CurrentPage, Find.ClosestLanguageRoot, true))
            {
                IBreadcrumbAppearance appearance = page as IBreadcrumbAppearance;
                bool visible = appearance == null || appearance.VisibleInBreadcrumb;
                if (visible && page.IsPage)
                {
                    ILink link = appearance == null ? (ILink)page : appearance;
                    if (added > 0)
                    {
                        Controls.AddAt(0, new LiteralControl(SeparatorText));
                        PrependAnchor(link, null);
                    }
                    else
                    {
                        PrependAnchor(link, "current");
                    }
                    ++added;
                }
            }

            if (added < VisibilityLevel)
                this.Visible = false;
                        
            base.CreateChildControls();
        }

        private void PrependAnchor(N2.Web.ILink item, string cssClass)
        {
            Control anchor = N2.Web.Link.To(item).Class(cssClass).ToControl();
            Controls.AddAt(0, anchor);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write("<div class='path'>");
            base.Render(writer);
            writer.Write("</div>");
        }
    }
}
