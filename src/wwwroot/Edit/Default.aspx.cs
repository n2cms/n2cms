using System;
using N2.Web.UI.WebControls;

namespace N2.Edit
{
    [ControlPanelLink("cpAdminister", "~/edit/img/ico/sitemap_color.gif", "~/edit/?selected={Selected.Path}", "Administer site", -50, ControlPanelState.Visible, Target = Targets.Top)]
	[ControlPanelSeparator(0, ControlPanelState.Visible)]
	public partial class Default : Web.EditPage
	{
    	protected string SelectedPath = "/";

		protected override void OnInit(EventArgs e)
		{
			try
			{
				SelectedPath = SelectedItem.Path;
			}
			catch(Exception ex)
			{
				Trace.Write(ex.ToString());
				Response.Redirect("install/begin/default.aspx");
			}

			base.OnInit(e);
		}
	}
}