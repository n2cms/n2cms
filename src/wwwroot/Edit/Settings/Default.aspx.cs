using System;
using N2.Configuration;
using System.Configuration;
using System.Web.UI;

namespace N2.Edit.Settings
{
	[ToolbarPlugin("CMS Settings", "settings", "settings/default.aspx?selected={selected}", ToolbarArea.Search, "navigation", "~/Edit/Img/Ico/wrench.gif", -10, ToolTip = "settings", GlobalResourceClassName = "Toolbar")]
	public partial class Default : Web.EditPage
	{
		protected override void OnInit(EventArgs e)
		{
            hlCancel.NavigateUrl = GetNavigationUrl(Selection.SelectedItem);

			EditSection config = Engine.Resolve<EditSection>();
			if (config == null) throw new ConfigurationErrorsException("Cannot find configuration n2/edit");

			foreach(SettingsEditorElement element in config.SettingsEditors)
			{
				Control editor = LoadControl(element.Path);
				phSettings.Controls.Add(editor);
			}

			base.OnInit(e);
		}

		protected void btnSave_Click(object sender, EventArgs e)
		{
			foreach(Control editor in phSettings.Controls)
			{
				if(editor is ISettingsEditor)
					(editor as ISettingsEditor).Save();
			}
            Response.Redirect(GetNavigationUrl(Selection.SelectedItem));
		}
	}
}
