using System;
using System.Collections.Generic;
using System.Text;
using N2.Engine;
using System.Reflection;
using System.Security.Principal;

namespace N2.Plugin
{
    public class PluginFinder : IPluginFinder
    {
        private IList<IPlugin> plugins = null;
        private readonly ITypeFinder typeFinder;

        public PluginFinder(ITypeFinder typeFinder)
        {
            this.typeFinder = typeFinder;
            this.plugins = FindPlugins();
        }

        public IEnumerable<T> GetPlugins<T>(IPrincipal user) where T : class, IPlugin
        {
            foreach (T plugin in GetPlugins<T>())
                if (plugin.IsAuthorized(user))
                    yield return plugin;
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
                foreach (IPlugin plugin in FindPluginsIn(assembly))
                {
                    if (plugin.Name == null)
                        throw new N2Exception("A plugin in the assembly '{0}' has no name. The plugin is likely defined on the assembly ([assembly:...]). Try assigning the plugin a unique name and recompiling.", assembly.FullName);
                    else if (foundPlugins.Contains(plugin))
                        throw new N2Exception("A plugin of the type '{0}' named '{1}' is already defined, assembly: {2}", plugin.GetType().FullName, plugin.Name, assembly.FullName);

                    foundPlugins.Add(plugin);
                }
            }
            foundPlugins.Sort();
            return foundPlugins;
        }

        private IEnumerable<IPlugin> FindPluginsIn(Assembly a)
        {
            foreach (IPlugin attribute in a.GetCustomAttributes(typeof(IPlugin), false))
            {
                yield return attribute;
            }
            foreach (Type t in a.GetTypes())
            {
                foreach (IPlugin attribute in t.GetCustomAttributes(typeof(IPlugin), false))
                {
                    if (attribute.Name == null)
                        attribute.Name = t.Name;

                    yield return attribute;
                }
            }
        }
    }
}
