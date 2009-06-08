using System;
using N2.Web.UI.WebControls;

namespace N2.Edit
{
	[ToolbarPlugin("", "tpPreview", "{url}", ToolbarArea.Preview, Targets.Preview, "~/Edit/Img/Ico/Png/eye.png", 0, ToolTip = "edit", GlobalResourceClassName = "Toolbar")]
	[ControlPanelLink("cpAdminister", "~/edit/img/ico/png/application_side_tree.png", "~/edit/?selected={Selected.Path}", "Administer site", -50, ControlPanelState.Visible, Target = Targets.Top)]
	[ControlPanelSeparator(0, ControlPanelState.Visible)]
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
			try
			{
				selectedPath = SelectedItem.Path;
				selectedUrl = Engine.EditManager.GetPreviewUrl(SelectedItem);
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