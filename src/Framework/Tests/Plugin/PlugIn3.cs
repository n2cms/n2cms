using N2.Plugin;

namespace N2.Tests.Plugin
{
    public class PlugIn3 : IPluginInitializer
    {
        public static bool WasInitialized { get; set; }

        public void Initialize(N2.Engine.IEngine engine)
        {
            WasInitialized = true;
        }
    }
}
