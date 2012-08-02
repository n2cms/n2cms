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
		private bool ensureBinFolderAssembliesLoaded = true;
		private bool binFolderAssembliesLoaded = false;
		private AssemblyCache assemblyCache;
		
		public WebAppTypeFinder(AssemblyCache assemblyCache, EngineSection engineConfiguration)
		{
			this.assemblyCache = assemblyCache;
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

		public override IEnumerable<Assembly> GetAssemblies()
		{
			return assemblyCache.GetAssemblies(GetAssembliesInternal);
		}

		private IEnumerable<Assembly> GetAssembliesInternal()
		{
			if (EnsureBinFolderAssembliesLoaded && !binFolderAssembliesLoaded)
			{
				binFolderAssembliesLoaded = true;
				foreach (var probingPath in assemblyCache.GetProbingPaths())
					LoadMatchingAssemblies(probingPath);
			}

			return base.GetAssemblies();
		} 
		#endregion
	}
}
