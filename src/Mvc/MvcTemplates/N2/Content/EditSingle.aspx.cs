using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Edit;
using N2.Security;
using N2.Edit.Web;
using N2.Web.UI.WebControls;
using N2.Web.UI;
using N2.Edit.Workflow;

namespace N2.Management.Content
{
	public partial class EditSingle : EditPage
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}

		protected void OnPublishCommand(object sender, CommandEventArgs args)
		{
			foreach (var ie in phAncestors.Controls.OfType<ItemEditor>())
			{
				Engine.Resolve<CommandDispatcher>().Publish(ie.CreateCommandContext());
			}
			Refresh(Selection.SelectedItem);
		}
	}
}
