using System;
using System.Linq;
using System.Web.UI.WebControls;
using N2.Persistence;
using N2.Linq;

namespace N2.Edit.LinkTracker
{
	public partial class UpdateReferences : N2.Edit.Web.EditPage
	{
		private Tracker tracker;

		protected override void OnInit(EventArgs e)
		{
			tracker = Engine.Resolve<Tracker>();

			base.OnInit(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			Title = "Update links leading to " + Selection.SelectedItem.Title;

			if (!IsPostBack)
			{
				var referrers = tracker.FindReferrers(Selection.SelectedItem).ToList();
				bool showReferences = referrers.Count > 0;
				if (showReferences)
				{
					rptReferencingItems.DataSource = referrers;
					DataBind();
				}
				else
					fsReferences.Visible = false;

				bool showChildren = Selection.SelectedItem.Children.Count > 0;
				if (showChildren)
				{
					targetsToUpdate.CurrentItem = Selection.SelectedItem;
					targetsToUpdate.DataBind();
				}
				else
					fsChildren.Visible = false;

				if (!showReferences && !showChildren)
				{
					Refresh(Selection.SelectedItem, ToolbarArea.Both);
				}
			}
		}

		protected void OnUpdateCommand(object sender, CommandEventArgs args)
		{
			tracker.UpdateReferencesTo(Selection.SelectedItem);
			if (chkChildren.Checked)
			{
				mvPhase.ActiveViewIndex = 1;
				rptDescendants.DataSource = Content.Search.Find.Where.AncestralTrail.Like(Selection.SelectedItem.GetTrail() + "%").Select()
					.Where(Content.Is.Accessible());
				rptDescendants.DataBind();
			}
			else
			{
				Refresh(Selection.SelectedItem, ToolbarArea.Both);
			}
		}
	}
}
