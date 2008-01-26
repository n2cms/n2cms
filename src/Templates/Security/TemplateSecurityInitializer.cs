using N2.Plugin;

namespace N2.Templates.Security
{
	[AutoInitialize]
	public class TemplateSecurityInitializer : IPluginInitializer
	{
		#region IPluginInitializer Members

		public void Initialize(Engine.IEngine engine)
		{
			engine.AddComponent("n2.templates.security", typeof(ItemBridge));
		}

		#endregion
	}
}
