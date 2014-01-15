using N2.Configuration;

namespace N2.Engine.Configuration
{
    /// <summary>
    /// Performs one-off configuration of the <see cref="IServiceContainer"/>.
    /// </summary>
    public interface IServiceContainerConfigurer
    {
        void Configure(IEngine engine, EngineSection engineConfig);
    }
}
