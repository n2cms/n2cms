using N2.Engine;
using N2.Plugin;

namespace N2.Edit.Settings
{
	[AutoInitialize]
	public class SettingsInitializer : IPluginInitializer
	{
		public void Initialize(IEngine engine)
		{
			engine.AddFacility("n2.edit.settingsFacility", new SettingsFinder());
		}
	}
}
