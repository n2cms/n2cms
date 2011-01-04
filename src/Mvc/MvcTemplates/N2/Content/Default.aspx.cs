using System;
using N2.Configuration;
using N2.Web.UI.WebControls;

namespace N2.Edit
{
	[ToolbarPlugin("PAGES", "tree", "{ManagementUrl}/Content/Default.aspx?selected={selected}", ToolbarArea.Navigation, Targets.Top, "{ManagementUrl}/Resources/icons/sitemap_color.png", -30,
		ToolTip = "show navigation",
		GlobalResourceClassName = "Toolbar", SortOrder = -1)]
	[ToolbarPlugin("VIEW", "preview", "{url}", ToolbarArea.Preview | ToolbarArea.Files, Targets.Preview, "{ManagementUrl}/Resources/icons/eye.png", 0, ToolTip = "Preview", 
		GlobalResourceClassName = "Toolbar")]
	[ControlPanelLink("cpAdminister", "{ManagementUrl}/Resources/icons/application_side_expand.png", "{ManagementUrl}/Content/Default.aspx?selected={Selected.Path}", "Manage content", -50, ControlPanelState.Visible, 
		Target = Targets.Top)]
	[ControlPanelLink("cpView", "{ManagementUrl}/Resources/icons/application_side_contract.png", "{Selected.Url}", "View", -60, ControlPanelState.Visible, 
		Target = Targets.Top)]
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
				selectedPath = Selection.SelectedItem.Path;
				selectedUrl = Engine.GetContentAdapter<NodeAdapter>(Selection.SelectedItem).GetPreviewUrl(Selection.SelectedItem);
			}
			catch(Exception ex)
			{
				Trace.Write(ex.ToString());
				Response.Redirect(Engine.Resolve<EditSection>().Installer.WelcomeUrl);
			}

			base.OnInit(e);
		}
	}
}