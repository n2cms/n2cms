using System;
using N2.Plugin;
using System.ComponentModel;

namespace N2.PlugIn
{
	/// <summary>
	/// Use this attribute to denote plugins and reference a plugin initializer 
	/// that is invoked when the factory is started.
	/// </summary>
	[Obsolete("Changed casing to N2.Plugin.PluginAttribute. Please also check your IPluginInitializers.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class PlugInAttribute : PluginAttribute
	{
		/// <summary>Creates a new instance of the PlugInAttribute class.</summary>
		public PlugInAttribute()
		{
		}

		/// <summary>Creates a new instance of the PlugInAttribute class.</summary>
		/// <param name="title">The title of the plugin.</param>
		/// <param name="name">The name of the plugin.</param>
		/// <param name="initializerTypeName">The name of the type responsible for initializing this plugin.</param>
		public PlugInAttribute(string title, string name, string initializerTypeName)
			: base(title, name, initializerTypeName)
		{
		}

		/// <summary>Creates a new instance of the PlugInAttribute class.</summary>
		/// <param name="title">The title of the plugin.</param>
		/// <param name="name">The name of the plugin.</param>
		/// <param name="initializerType">The name of the type responsible for initializing this plugin.</param>
		public PlugInAttribute(string title, string name, Type initializerType)
			: base(title, name, initializerType)
		{
		} 
	}
}
