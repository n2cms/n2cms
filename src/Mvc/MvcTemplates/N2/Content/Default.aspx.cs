using System;
using N2.Configuration;
using N2.Security;
using N2.Web.UI.WebControls;
using System.Web.Security;

namespace N2.Edit
{
    public class SwitchToManagementControlPanelLinkAttribute : ControlPanelLinkAttribute
    {
        public SwitchToManagementControlPanelLinkAttribute()
            : base ("cpAdminister", "{ManagementUrl}/Resources/icons/application_side_expand.png", "{ManagementUrl}/?{Selection.SelectedQueryKey}={Selected.Path}", "Manage content", -50, ControlPanelState.Visible)
        {
            CssClass = "complementary toggled";
            Target = Targets.Top;
            RequiredPermission = Permission.Read;
            IconClass = "fa fa-eye-slash";
			Legacy = true;
        }

        protected override N2.Web.Url GetNavigateUrl(PluginContext context)
        {
            if (!Engine.Config.Sections.Management.Legacy)
                return base.GetNavigateUrl(context);

            return context.Rebase(context.Format("{ManagementUrl}/Content/Default.aspx?{Selection.SelectedQueryKey}={Selected.Path}", UrlEncode));
        }
    }

    [ToolbarPlugin("PAGES", "tree", "{ManagementUrl}/Content/Default.aspx?{Selection.SelectedQueryKey}={selected}", ToolbarArea.Navigation, Targets.Top, "{ManagementUrl}/Resources/icons/sitemap_color.png", -30,
        ToolTip = "show navigation",
        GlobalResourceClassName = "Toolbar", SortOrder = -1,
        OptionProvider = typeof(ViewOptionProvider),
        Legacy = true)]
    [ToolbarPlugin("VIEW", "preview", "{url}", ToolbarArea.Preview | ToolbarArea.Files, Targets.Preview, "{ManagementUrl}/Resources/icons/eye.png", 0, ToolTip = "Preview",
        GlobalResourceClassName = "Toolbar",
        Legacy = true)]
    [SwitchToManagementControlPanelLink]
    [ControlPanelLink("cpView", "{ManagementUrl}/Resources/icons/application_side_contract.png", "{Selected.Url}", "View", -60, ControlPanelState.Visible, 
        CssClass = "",
        Target = Targets.Top,
        IconClass = "fa fa-eye",
		Legacy = true)]
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
