using N2.Plugin;

namespace N2.Tests.Plugin
{
    [AutoInitialize]
    public class ThrowingPlugin1 : IPluginInitializer
    {
        public static bool WasInitialized { get; set; }
        public static bool Throw { get; set; }
        public void Initialize(N2.Engine.IEngine engine)
        {
            WasInitialized = true;
            if (Throw)
                throw new SomeException("ThrowingPlugin1 isn't happy.");
        }
    }
}
