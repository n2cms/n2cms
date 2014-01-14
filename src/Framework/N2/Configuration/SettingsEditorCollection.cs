using System.Configuration;

namespace N2.Configuration
{
    [ConfigurationCollection(typeof(SettingsEditorElement))]
    public class SettingsEditorCollection : LazyRemovableCollection<SettingsEditorElement>
    {
        public SettingsEditorCollection()
        {
            AddDefault(new SettingsEditorElement("default", "{ManagementUrl}/Content/Settings/SettingsEditor.ascx"));
        }
    }
}
