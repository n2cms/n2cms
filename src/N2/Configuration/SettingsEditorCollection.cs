using System.Configuration;

namespace N2.Configuration
{
	[ConfigurationCollection(typeof(SettingsEditorElement))]
	public class SettingsEditorCollection : ConfigurationElementCollection
	{
		public SettingsEditorCollection()
		{
			BaseAdd(new SettingsEditorElement("default", "~/N2/Content/Settings/SettingsEditor.ascx"));
		}
		protected override ConfigurationElement CreateNewElement()
		{
			return new SettingsEditorElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((SettingsEditorElement) element).Name;
		}
	}
}