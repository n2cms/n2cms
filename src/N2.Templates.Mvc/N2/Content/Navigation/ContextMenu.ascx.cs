using System;
using N2.Edit.Web;
using N2.Web.UI.WebControls;
using N2.Web;

namespace N2.Edit.Navigation
{
	public partial class ContextMenu : EditUserControl
	{
		protected override void OnInit(EventArgs e)
		{
			var start = Engine.Resolve<IUrlParser>().StartPage;
			var root = Engine.Persister.Repository.Load(Engine.Resolve<IHost>().CurrentSite.RootItemID);
			foreach (NavigationPluginAttribute a in Engine.EditManager.GetPlugins<NavigationPluginAttribute>(Page.User))
			{
				a.AddTo(plhMenuItems, new PluginContext(Selection.SelectedItem, null, start, root, ControlPanelState.Visible,
				                                        Engine.ManagementPaths));
			}
			base.OnInit(e);
		}
	}
}