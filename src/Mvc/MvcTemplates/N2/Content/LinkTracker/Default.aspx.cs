using System;

namespace N2.Edit.LinkTracker
{
    [ToolbarPlugin("LINKS", "linktracker", "{ManagementUrl}/Content/LinkTracker/Default.aspx?{Selection.SelectedQueryKey}={selected}", ToolbarArea.Options, Targets.Preview, "{ManagementUrl}/Resources/icons/link_break.png", 160, 
        ToolTip = "tracks inbound/outbound links",
        GlobalResourceClassName = "Toolbar",
        Legacy = true)]
    public partial class _Default : N2.Edit.Web.EditPage
    {
        protected override void OnInit(EventArgs e)
        {
            Page.Title += ": " + Selection.SelectedItem.Title;

            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var tracker = Engine.Resolve<Tracker>();
            rptReferencingItems.DataSource = tracker.FindReferrers(Selection.SelectedItem);
            rptReferencedItems.DataSource = tracker.FindLinkedItems(Selection.SelectedItem);
            DataBind();
        }
    }
}
