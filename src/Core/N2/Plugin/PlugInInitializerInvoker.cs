using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using N2.Engine;

namespace N2.Plugin
{
	/// <summary>
	/// Finds plugins and calls their initializer.
	/// </summary>
	public class PluginInitializerInvoker : IPluginInitializerInvoker
	{
		private readonly ITypeFinder typeFinder;

		public PluginInitializerInvoker(ITypeFinder typeFinder)
		{
			this.typeFinder = typeFinder;
		}

		/// <summary>Gets plugins in the current app domain using the type finder.</summary>
		/// <returns>An enumeration of available plugins.</returns>
		public IEnumerable<IPluginDefinition> GetPluginDefinitions()
		{
			foreach (Assembly assembly in typeFinder.GetAssemblies())
			{
				foreach (PluginAttribute plugin in assembly.GetCustomAttributes(typeof(PluginAttribute), false))
				{
					yield return plugin;
				}
			}
			
			foreach(Type type in typeFinder.Find(typeof(IPluginInitializer)))
			{
				foreach (AutoInitializeAttribute plugin in type.GetCustomAttributes(typeof(AutoInitializeAttribute), true))
				{
					plugin.InitializerType = type;
					yield return plugin;
				}
			}
		}

		/// <summary>Invokes the initialize method on the supplied plugins.</summary>
		public void InitializePlugins(IEngine engine, IEnumerable<IPluginDefinition> plugins)
		{
			foreach (IPluginDefinition plugin in plugins)
				plugin.Initialize(engine);
		}
	}
}
