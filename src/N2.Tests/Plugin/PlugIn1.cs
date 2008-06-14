using N2.Plugin;

[assembly:Plugin("Testplugin", "testplugin", typeof(N2.Tests.PlugIn.PlugIn1))]

namespace N2.Tests.PlugIn
{
	public class PlugIn1 : IPluginInitializer
	{
        public bool IsInitialized { get; set; }

		public void Initialize(N2.Engine.IEngine engine)
		{
            IsInitialized = true;
		}
	}
}
