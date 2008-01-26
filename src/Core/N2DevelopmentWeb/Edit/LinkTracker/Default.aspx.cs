using System;

namespace N2.Edit.LinkTracker
{
	[ToolbarPlugIn("", "linktracker", "~/Edit/LinkTracker/Default.aspx?selected={selected}", ToolbarArea.Preview, "preview", "~/Edit/LinkTracker/Img/link_break.gif", 150, ToolTip = "tracks inbound/outbound links")]
	public partial class _Default : N2.Edit.Web.EditPage
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			rptReferencingItems.DataSource = Engine.Resolve<Tracker>().FindReferrers(this.SelectedItem);
			rptReferencedItems.DataSource = Engine.Resolve<Tracker>().FindLinkedItems(this.SelectedItem);
			DataBind();
		}
	}
}
