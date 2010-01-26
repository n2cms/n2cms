using System;
using N2.Web.UI.WebControls;
using System.Web.Security;

namespace N2.Edit
{
	[ToolbarPlugin("VIEW", "preview", "{url}", ToolbarArea.Preview, Targets.Preview, "~/N2/Resources/Img/Ico/Png/eye.png", 0, ToolTip = "Preview", GlobalResourceClassName = "Toolbar")]
	[ToolbarPlugin("PAGES", "tree", "Content/default.aspx?selected={selected}", ToolbarArea.Navigation, Targets.Top, "~/N2/Resources/Img/Ico/png/sitemap_color.png", -30,
		ToolTip = "hierarchical navigation", GlobalResourceClassName = "Toolbar", SortOrder = -1)]
	[ControlPanelLink("cpAdminister", "~/N2/Resources/Img/ico/png/application_side_tree.png", "Content/default.aspx?selected={Selected.Path}", "Manage content", -50, ControlPanelState.Visible, Target = Targets.Top)]
    [ControlPanelLink("cpView", "~/N2/Resources/Img/ico/png/eye.png", "{Selected.Url}", "View", -60, ControlPanelState.Visible, Target = Targets.Top)]
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
				Response.Redirect("../installation/begin/default.aspx");
			}

			base.OnInit(e);
		}
	}
}