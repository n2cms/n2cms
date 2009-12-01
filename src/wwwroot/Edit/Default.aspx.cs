using System;
using N2.Web.UI.WebControls;
using System.Web.Security;

namespace N2.Edit
{
	[ToolbarPlugin("VIEW", "preview", "{url}", ToolbarArea.Preview, Targets.Preview, "~/Edit/Img/Ico/Png/eye.png", 0, ToolTip = "Preview", GlobalResourceClassName = "Toolbar")]
    [ControlPanelLink("cpAdminister", "~/edit/img/ico/png/application_side_tree.png", "~/edit/?selected={Selected.Path}", "Administer site", -50, ControlPanelState.Visible, Target = Targets.Top)]
    [ControlPanelLink("cpView", "~/edit/img/ico/png/eye.png", "{Selected.Url}", "View", -60, ControlPanelState.Visible, Target = Targets.Top)]
    public partial class Default : Web.EditPage
	{
		string selectedPath;
		string selectedUrl;

		public string SelectedPath
		{
			get { return selectedPath ?? "/"; }
		}
		public string SelectedUrl
		{
			get { return selectedUrl ?? "~/"; }
		}

		protected override void OnInit(EventArgs e)
		{
            logout.ToolTip = string.Format(GetLocalResourceString("logout.ToolTipFormat"), User.Identity.Name);
			try
			{
                selectedPath = Selection.SelectedItem.Path;
                selectedUrl = Engine.EditManager.GetPreviewUrl(Selection.SelectedItem);
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