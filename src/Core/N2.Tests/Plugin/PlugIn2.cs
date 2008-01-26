using N2.Plugin;

namespace N2.Tests.PlugIn
{
	[AutoInitialize]
	public class PlugIn2 : IPluginInitializer
	{
		public void Initialize(Engine.IEngine engine)
		{
			object ignored = engine.Definitions;
		}
	}
}
