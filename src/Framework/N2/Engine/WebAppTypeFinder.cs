using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using N2.Configuration;

namespace N2.Engine
{
	/// <summary>
	/// Provides information about types in the current web application. 
	/// Optionally this class can look at all assemblies in the bin folder.
	/// </summary>
	public class WebAppTypeFinder : AppDomainTypeFinder
	{
		private Web.IWebContext webContext;
		private bool ensureBinFolderAssembliesLoaded = true;
		private bool binFolderAssembliesLoaded = false; 

		public WebAppTypeFinder(Web.IWebContext webContext)
		{
			this.webContext = webContext;
		}

		public WebAppTypeFinder(Web.IWebContext webContext, EngineSection engineConfiguration)
		{
			this.webContext = webContext;
			this.ensureBinFolderAssembliesLoaded = engineConfiguration.DynamicDiscovery;
			foreach (var assembly in engineConfiguration.Assemblies.AllElements)
				AssemblyNames.Add(assembly.Assembly);
		}

		#region Properties
		/// <summary>Gets or sets wether assemblies in the bin folder of the web application should be specificly checked for beeing loaded on application load. This is need in situations where plugins need to be loaded in the AppDomain after the application been reloaded.</summary>
		public bool EnsureBinFolderAssembliesLoaded
		{
			get { return ensureBinFolderAssembliesLoaded; }
			set { ensureBinFolderAssembliesLoaded = value; }
		}
	
		#endregion

		#region Methods

		Assembly[] assemblyCache;
		public override IList<Assembly> GetAssemblies()
		{
			if (assemblyCache != null)
				return assemblyCache;

			if (EnsureBinFolderAssembliesLoaded && !binFolderAssembliesLoaded)
			{
				binFolderAssembliesLoaded = true;
				LoadMatchingAssemblies(webContext.MapPath("~/bin"));
			}

			return assemblyCache = base.GetAssemblies().ToArray();
		} 
		#endregion
	}
}
