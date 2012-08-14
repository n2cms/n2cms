using System;
using N2.Configuration;
using N2.Security;
using N2.Web.UI.WebControls;
using System.Web.Security;

namespace N2.Edit
{
	[ToolbarPlugin("PAGES", "tree", "{ManagementUrl}/Content/Default.aspx?{Selection.SelectedQueryKey}={selected}", ToolbarArea.Navigation, Targets.Top, "{ManagementUrl}/Resources/icons/sitemap_color.png", -30,
		ToolTip = "show navigation",
		GlobalResourceClassName = "Toolbar", SortOrder = -1)]
	[ToolbarPlugin("VIEW", "preview", "{url}", ToolbarArea.Preview | ToolbarArea.Files, Targets.Preview, "{ManagementUrl}/Resources/icons/eye.png", 0, ToolTip = "Preview", 
		GlobalResourceClassName = "Toolbar")]
	[ControlPanelLink("cpAdminister", "{ManagementUrl}/Resources/icons/application_side_expand.png", "{ManagementUrl}/Content/Default.aspx?{Selection.SelectedQueryKey}={Selected.Path}", "Manage content", -50, ControlPanelState.Visible,
		Target = Targets.Top,
		RequiredPermission = Permission.Read)]
	[ControlPanelLink("cpView", "{ManagementUrl}/Resources/icons/application_side_contract.png", "{Selected.Url}", "View", -60, ControlPanelState.Visible, 
		Target = Targets.Top)]
	public partial class Default : Web.EditPage
	{
		private readonly Engine.Logger<Default> logger;
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
				logger.Error(ex);
				Response.Redirect(N2.Web.Url.ResolveTokens(Engine.Resolve<EditSection>().Installer.WelcomeUrl));
			}

			Resources.Register.JQueryUi(Page);
			Resources.Register.JQueryPlugins(Page);

			base.OnInit(e);
		}
	}
}