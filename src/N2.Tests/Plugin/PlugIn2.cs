using N2.Plugin;

namespace N2.Tests.PlugIn
{
	[AutoInitialize]
	public class PlugIn2 : IPluginInitializer
	{
        public static bool WasInitialized { get; set; }

		public void Initialize(N2.Engine.IEngine engine)
		{
            WasInitialized = true;
		}
	}
}
