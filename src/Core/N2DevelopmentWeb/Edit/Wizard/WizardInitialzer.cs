using N2.Edit.Wizard;
using N2.Engine;
using N2.Plugin;

namespace N2.Edit.Wizard
{
	[AutoInitialize]
	public class WizardInitialzer : IPluginInitializer
	{
		public void Initialize(IEngine engine)
		{
			engine.AddComponent("n2.wizard", typeof(LocationWizard));
		}
	}
}
