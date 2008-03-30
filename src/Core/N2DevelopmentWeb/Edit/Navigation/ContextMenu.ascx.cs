using System;
using System.Web.UI;

namespace N2.Edit.Navigation
{
	public partial class ContextMenu : UserControl
	{
		protected override void OnInit(EventArgs e)
		{
			ContentItem selectedItem = ((NavigationPage) Page).SelectedItem;

			foreach (NavigationPluginAttribute a in N2.Context.Current.EditManager.GetNavigationPlugIns(Page.User))
			{
				a.AddTo(plhMenuItems);
			}
			base.OnInit(e);
		}
	}
}