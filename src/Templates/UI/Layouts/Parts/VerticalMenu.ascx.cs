using N2.Web.UI.WebControls;

namespace N2.Templates.UI.Layouts.Parts
{
	public partial class VerticalMenu : Web.UI.TemplateUserControl<Items.AbstractContentPage, Items.LayoutParts.VerticalMenu>
	{
		protected H4 h;
		protected override void OnInit(System.EventArgs e)
		{
			base.OnInit(e);

			ContentItem branchRoot = Find.FindAncestorAtLevel(CurrentItem.StartingDepth);
			if(branchRoot != null)
				h.Text = N2.Web.Link.To(branchRoot).ToString();
			else
				Visible = false;
		}
	}
}