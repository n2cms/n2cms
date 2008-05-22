using N2.Plugin;

namespace N2.Security
{
	[AutoInitialize]
	public class SecurityInitializer : IPluginInitializer
	{
		public void Initialize(Engine.IEngine engine)
		{
			engine.AddComponent("n2.securityProvider", typeof(ItemBridge));
		}
	}
}
