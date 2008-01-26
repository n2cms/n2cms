using System;
using System.Web.UI;

namespace N2.Edit.Navigation
{
	public partial class ContextMenu : UserControl
	{
		protected override void OnInit(EventArgs e)
		{
			ContentItem selectedItem = ((NavigationPage) Page).SelectedItem;

			foreach (NavigationPlugInAttribute a in N2.Context.Instance.EditManager.GetNavigationPlugIns(Page.User))
			{
				a.AddTo(plhMenuItems, selectedItem);
			}
			base.OnInit(e);
		}
	}
}