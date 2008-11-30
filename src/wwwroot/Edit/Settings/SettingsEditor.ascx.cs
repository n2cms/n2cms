using System;
using N2.Edit.Web;

namespace N2.Edit.Settings
{
	public partial class SettingsEditor : EditUserControl, ISettingsEditor
	{
		NavigationSettings settings;

		protected override void OnInit(EventArgs e)
		{
			settings = Engine.Resolve<NavigationSettings>();
			chkShowDataItems.Checked = settings.DisplayDataItems;
			base.OnInit(e);
		}

		public void Save()
		{
			settings.DisplayDataItems = chkShowDataItems.Checked;
		}
	}
}