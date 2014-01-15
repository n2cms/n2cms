using System.Collections.Generic;
using N2.Engine;

namespace N2.Plugin
{
    /// <summary>
    /// Finds plugins and calls their initializer.
    /// </summary>
    public interface IPluginBootstrapper
    {
        IEnumerable<IPluginDefinition> GetPluginDefinitions();
        void InitializePlugins(IEngine engine, IEnumerable<IPluginDefinition> plugins);
    }
}
