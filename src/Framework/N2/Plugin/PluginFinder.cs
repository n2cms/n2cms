using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Principal;
using N2.Configuration;
using N2.Engine;
using N2.Security;

namespace N2.Plugin
{
    /// <summary>
    /// Investigates the execution environment to find plugins.
    /// </summary>
    [Service(typeof(IPluginFinder))]
    public class PluginFinder : IPluginFinder
    {
        private IList<IPlugin> plugins = null;
        private readonly ITypeFinder typeFinder;
        private readonly ISecurityManager security;
        public IEnumerable<InterfacePluginElement> addedPlugins = new InterfacePluginElement[0];
        public IEnumerable<InterfacePluginElement> removedPlugins = new InterfacePluginElement[0];

        public PluginFinder(ITypeFinder typeFinder, ISecurityManager security, EngineSection config)
        {
            addedPlugins = config.InterfacePlugins.AllElements;
            removedPlugins = config.InterfacePlugins.RemovedElements;
            this.typeFinder = typeFinder;
            this.security = security;
            this.plugins = FindPlugins();
        }

        [Obsolete("Use constructor(ITypeFinder, ISecurityManager, EngineSection)", true)]
        public PluginFinder(ITypeFinder typeFinder)
        {
            this.typeFinder = typeFinder;
            this.plugins = FindPlugins();
        }

        /// <summary>Gets plugins found in the environment sorted and filtered by the given user.</summary>
        /// <typeparam name="T">The type of plugin to get.</typeparam>
        /// <param name="user">The user that should be authorized for the plugin.</param>
        /// <returns>An enumeration of plugins.</returns>
        public IEnumerable<T> GetPlugins<T>(IPrincipal user) where T : class, IPlugin
        {
            foreach (T plugin in GetPlugins<T>())
            {
                if(security.IsAuthorized(plugin, user, null))
                    yield return plugin;
            }
        }

        public IEnumerable<T> GetPlugins<T>() where T : class, IPlugin
        {
            foreach (IPlugin plugin in plugins)
                if (plugin is T)
                    yield return plugin as T;
        }

        /// <summary>Finds and sorts plugin defined in known assemblies.</summary>
        /// <returns>A sorted list of plugins.</returns>
        protected virtual IList<IPlugin> FindPlugins()
        {
            List<IPlugin> foundPlugins = new List<IPlugin>();
            foreach (Assembly assembly in typeFinder.GetAssemblies())
            {
                foreach (IPlugin plugin in assembly.GetCustomAttributes(typeof(IPlugin), false))
                {
                    AddPlugin(foundPlugins, assembly, plugin);
                }
            }
            foreach (var at in typeFinder.Find<IPlugin>(typeof(object), inherit: false))
            {
                at.Attribute.Decorates = at.Type;
                at.Attribute.Name = at.Attribute.Name ?? at.Type.Name;
                AddPlugin(foundPlugins, at.Type.Assembly, at.Attribute);
            }
            foundPlugins.Sort();
            return foundPlugins;
        }

        private void AddPlugin(List<IPlugin> foundPlugins, Assembly assembly, IPlugin plugin)
        {
            if (plugin.Name == null)
                throw new N2Exception("A plugin in the assembly '{0}' has no name. The plugin is likely defined on the assembly ([assembly:...]). Try assigning the plugin a unique name and recompiling.", assembly.FullName);
            if (foundPlugins.Contains(plugin))
                throw new N2Exception("A plugin of the type '{0}' named '{1}' is already defined, assembly: {2}", plugin.GetType().FullName, plugin.Name, assembly.FullName);

            if (!IsRemoved(plugin))
                foundPlugins.Add(plugin);
        }

        private bool IsRemoved(IPlugin plugin)
        {
            foreach (InterfacePluginElement configElement in removedPlugins)
            {
                if (plugin.Name == configElement.Name)
                    return true;
            }
            return false;
        }
    }
}
