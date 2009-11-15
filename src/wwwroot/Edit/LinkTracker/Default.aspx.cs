using System;

namespace N2.Edit.LinkTracker
{
	[ToolbarPlugin("LINKS", "linktracker", "~/Edit/LinkTracker/Default.aspx?selected={selected}", ToolbarArea.Preview, Targets.Preview, "~/Edit/LinkTracker/Img/link_break.gif", 160, ToolTip = "tracks inbound/outbound links", GlobalResourceClassName = "Toolbar")]
	public partial class _Default : N2.Edit.Web.EditPage
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
            rptReferencingItems.DataSource = Engine.Resolve<Tracker>().FindReferrers(Selection.SelectedItem);
            rptReferencedItems.DataSource = Engine.Resolve<Tracker>().FindLinkedItems(Selection.SelectedItem);
			DataBind();
		}
	}
}
