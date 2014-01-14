using System;
using System.Configuration;
using System.Web.UI;
using N2.Configuration;
using N2.Web;

namespace N2.Edit.Settings
{
    [ToolbarPlugin("SETTINGS", "settings", "{ManagementUrl}/Content/Settings/Default.aspx?{Selection.SelectedQueryKey}={selected}", ToolbarArea.Options, "navigation", "{ManagementUrl}/Resources/icons/wrench.png", 1000, 
        ToolTip = "settings",
        GlobalResourceClassName = "Toolbar",
        Legacy = true)]
    public partial class Default : Web.EditPage
    {
        protected override void OnInit(EventArgs e)
        {
            EditSection config = Engine.Resolve<EditSection>();
            if (config == null) throw new ConfigurationErrorsException("Cannot find configuration n2/edit");

            foreach(SettingsEditorElement element in config.SettingsEditors.AllElements)
            {
                Control editor = LoadControl(Url.ResolveTokens(element.Path));
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
            Refresh(Selection.SelectedItem);
        }
    }
}
