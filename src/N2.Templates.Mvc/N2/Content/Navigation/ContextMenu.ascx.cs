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
			foreach (NavigationPluginAttribute a in N2.Context.Current.EditManager.GetPlugins<NavigationPluginAttribute>(Page.User))
			{
				a.AddTo(plhMenuItems, new PluginContext(SelectedItem, ControlPanelState.Visible));
			}
			base.OnInit(e);
		}
	}
}