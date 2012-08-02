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
		private bool dynamicDiscovery = true;
		private AssemblyCache assemblyCache;
		
		public WebAppTypeFinder(AssemblyCache assemblyCache, EngineSection engineConfiguration)
		{
			this.assemblyCache = assemblyCache;
			this.dynamicDiscovery = engineConfiguration.DynamicDiscovery;
			if (!string.IsNullOrEmpty(engineConfiguration.Assemblies.SkipLoadingPattern))
				this.AssemblySkipLoadingPattern = engineConfiguration.Assemblies.SkipLoadingPattern;
			if (!string.IsNullOrEmpty(engineConfiguration.Assemblies.RestrictToLoadingPattern)) 
				this.AssemblyRestrictToLoadingPattern = engineConfiguration.Assemblies.RestrictToLoadingPattern;
			foreach (var assembly in engineConfiguration.Assemblies.AllElements)
				AssemblyNames.Add(assembly.Assembly);
		}

		#region Methods

		public override IEnumerable<System.Type> Find(System.Type requestedType)
		{
			return assemblyCache.GetTypes(requestedType.FullName, () => base.Find(requestedType));
		}

		public override IEnumerable<AttributedType<TAttribute>> Find<TAttribute>(System.Type requestedType, bool inherit = true)
		{
			return assemblyCache.GetTypes(
				requestedType.FullName + "[" + typeof(TAttribute).FullName + "]",
				() => base.Find<TAttribute>(requestedType, inherit).Select(at => at.Type).Distinct())
				.SelectMany(t => SelectAttributedTypes<TAttribute>(t, inherit));
		}

		public override IEnumerable<Assembly> GetAssemblies()
		{
			return assemblyCache.GetAssemblies(GetAssembliesInternal);
		}

		private IEnumerable<Assembly> GetAssembliesInternal()
		{
			if (dynamicDiscovery)
			{
				var assemblies = assemblyCache.GetProbingPaths()
					.SelectMany(pp => LoadMatchingAssemblies(pp))
					.ToList();

				var addedAssemblyNames = new HashSet<string>(assemblies.Select(a => a.GetName().Name));
				assemblies.AddRange(GetConfiguredAssemblies(addedAssemblyNames));
				return assemblies;
			}

			return base.GetAssemblies();
		} 
		#endregion
	}
}
