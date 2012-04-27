using System;
using System.Web.UI.WebControls;
using N2.Persistence;

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

			if (!IsPostBack)
			{
				rptReferencingItems.DataSource = tracker.FindReferrers(Selection.SelectedItem);
				DataBind();
			}
		}

		protected void OnUpdateCommand(object sender, CommandEventArgs args)
		{
			tracker.UpdateReferencesTo(Selection.SelectedItem);
		}
	}
}
