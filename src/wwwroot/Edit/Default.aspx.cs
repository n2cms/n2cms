using System;
using N2.Resources;
using N2.Web.UI.WebControls;

namespace N2.Edit
{
    [ControlPanelLink("cpAdminister", "~/edit/img/ico/sitemap_color.gif", "~/edit/?selected={Selected.Path}", "Administer site", -50, ControlPanelState.Visible, Target = Targets.Top)]
	[ControlPanelSeparator(0, ControlPanelState.Visible)]
	public partial class Default : Web.EditPage
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			Register.JavaScript(this, "~/Edit/Js/Plugins.ashx");

			try
			{
				// These fields are used client side to store selected items
				Page.ClientScript.RegisterHiddenField("selected", SelectedItem.Path);
				Page.ClientScript.RegisterHiddenField("memory", "");
				Page.ClientScript.RegisterHiddenField("action", "");
			}
			catch(Exception ex)
			{
				Trace.Write(ex.ToString());
				Response.Redirect("install/begin/default.aspx");
			}
		}
	}
}