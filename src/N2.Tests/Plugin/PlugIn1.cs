using N2.Plugin;

[assembly:Plugin("Testplugin", "testplugin", typeof(N2.Tests.PlugIn.PlugIn1))]

namespace N2.Tests.PlugIn
{
	public class PlugIn1 : IPluginInitializer
	{
		public void Initialize(N2.Engine.IEngine engine)
		{
			object ignored = engine.Persister;
		}
	}
}
