using N2.Plugin;

namespace N2.Tests.Plugin
{
    [AutoInitialize]
    public class ThrowingPlugin2 : IPluginInitializer
    {
        public static bool WasInitialized { get; set; }
        public static bool Throw { get; set; }
        public void Initialize(N2.Engine.IEngine engine)
        {
            WasInitialized = true;
            if (Throw)
                throw new N2Exception("ThrowingPlugin2 is really mad.");
        }
    }
}
