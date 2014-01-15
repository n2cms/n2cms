using N2.Plugin;

[assembly:Plugin("Testplugin", "testplugin", typeof(N2.Tests.Plugin.PlugIn1))]

namespace N2.Tests.Plugin
{
    public class PlugIn1 : IPluginInitializer
    {
        public static bool WasInitialized { get; set; }

        public void Initialize(N2.Engine.IEngine engine)
        {
            WasInitialized = true;
        }
    }
}
