using System;

namespace N2.Plugin
{
	/// <summary>
	/// Use this attribute to denote plugins and reference a plugin initializer 
	/// that is invoked when the factory is started.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class PluginAttribute : InitializerCreatingAttribute
	{
		#region Private Fields
		private string name;
		private string title;
		#endregion

		#region Constructors
		/// <summary>Creates a new instance of the PlugInAttribute class.</summary>
		public PluginAttribute()
		{
		}

		/// <summary>Creates a new instance of the PlugInAttribute class.</summary>
		/// <param name="title">The title of the plugin.</param>
		/// <param name="name">The name of the plugin.</param>
		/// <param name="initializerTypeName">The name of the type responsible for initializing this plugin.</param>
		[Obsolete]
        public PluginAttribute(string title, string name, string initializerTypeName)
		{
			this.title = title;
			this.name = name;
			InitializerType = Utility.TypeFromName(initializerTypeName);
		}

		/// <summary>Creates a new instance of the PlugInAttribute class.</summary>
		/// <param name="title">The title of the plugin.</param>
		/// <param name="name">The name of the plugin.</param>
		/// <param name="initializerType">The name of the type responsible for initializing this plugin.</param>
		public PluginAttribute(string title, string name, Type initializerType)
		{
			this.title = title;
			this.name = name;
			InitializerType = initializerType;
		} 

		#endregion

		#region Properties
		/// <summary>Gets or sets the title of the plugin marked by this attribute.</summary>
		public string Title
		{
			get { return title; }
			set { title = value; }
		}

		/// <summary>Gets or sets the name of the plugin marked by this attribute.</summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}
		#endregion
	}
}
