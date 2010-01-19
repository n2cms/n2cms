using System;
using System.Web.UI;
using N2.Edit.Web;
using N2.Web.UI.WebControls;

namespace N2.Edit.Navigation
{
	public partial class ContextMenu : EditUserControl
	{
		protected override void OnInit(EventArgs e)
		{
			foreach (NavigationPluginAttribute a in Engine.EditManager.GetPlugins<NavigationPluginAttribute>(Page.User))
			{
				a.AddTo(plhMenuItems, new PluginContext(Selection.SelectedItem, null, ControlPanelState.Visible, Engine.EditManager.GetManagementInterfaceUrl()));
			}
			base.OnInit(e);
		}
	}
}