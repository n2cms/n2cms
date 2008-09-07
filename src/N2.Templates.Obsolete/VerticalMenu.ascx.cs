using N2.Web.UI.WebControls;

namespace N2.Templates.UI.Parts
{
    public partial class VerticalMenu : Web.UI.TemplateUserControl<Templates.Items.AbstractContentPage, Templates.Items.VerticalMenu>
    {
        protected H4 h;
        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);

            ContentItem branchRoot = Find.AncestorAtLevel(CurrentItem.StartingDepth);
            if(branchRoot != null)
                h.Text = N2.Web.Link.To(branchRoot).ToString();
            else
                Visible = false;
        }
    }
}