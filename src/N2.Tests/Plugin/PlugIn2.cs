using N2.Plugin;

namespace N2.Tests.PlugIn
{
	[AutoInitialize]
	public class PlugIn2 : IPluginInitializer
	{
        public bool IsInitialized { get; set; }

		public void Initialize(N2.Engine.IEngine engine)
		{
		}
	}
}
