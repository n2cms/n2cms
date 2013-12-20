using N2.Plugin;

namespace N2.Addons.MyAddon.Services
{
    /// <summary>
    /// The [AutoInitialize] attribute in combination with the IPluginInitializer 
    /// interface ensures this code is run during startup. This is where we can add
    /// components to the system.
    /// </summary>
    [AutoInitialize]
    public class MyInitializer : IPluginInitializer
    {
        public void Initialize(N2.Engine.IEngine engine)
        {
            engine.AddComponent("myComponent", typeof(Services.MyComponent));
        }
    }
}
