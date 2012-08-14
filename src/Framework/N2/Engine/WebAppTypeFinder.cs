using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using N2.Configuration;
using System;
using System.Text.RegularExpressions;

namespace N2.Engine
{
	/// <summary>
	/// Provides information about types in the current web application. 
	/// Optionally this class can look at all assemblies in the bin folder.
	/// </summary>
	public class WebAppTypeFinder : AppDomainTypeFinder
	{
		Logger<WebAppTypeFinder> logger;
		private bool dynamicDiscovery = true;
		private bool enableTypeCache = true;
		private TypeCache assemblyCache;
		
		public WebAppTypeFinder(TypeCache assemblyCache, EngineSection engineConfiguration)
		{
			this.assemblyCache = assemblyCache;
			this.dynamicDiscovery = engineConfiguration.DynamicDiscovery;
			this.enableTypeCache = engineConfiguration.Assemblies.EnableTypeCache;
			if (!string.IsNullOrEmpty(engineConfiguration.Assemblies.SkipLoadingPattern))
				this.AssemblySkipLoadingPattern = new Regex(engineConfiguration.Assemblies.SkipLoadingPattern);
			if (!string.IsNullOrEmpty(engineConfiguration.Assemblies.RestrictToLoadingPattern))
				this.AssemblyRestrictToLoadingPattern = new Regex(engineConfiguration.Assemblies.RestrictToLoadingPattern);
			logger.DebugFormat("EnableTypeCache: {0}, DynamicDiscovery: {1}, AssemblySkipLoadingPattern:{2}, AssemblyRestrictToLoadingPattern: {3}", enableTypeCache, dynamicDiscovery, AssemblySkipLoadingPattern, AssemblyRestrictToLoadingPattern);
			foreach (var assembly in engineConfiguration.Assemblies.AllElements)
			{
				logger.DebugFormat("Adding configured assembly {0}", assembly.Assembly);
				AssemblyNames.Add(assembly.Assembly);
			}
		}

		#region Methods

		public override IEnumerable<System.Type> Find(System.Type requestedType)
		{
			if (enableTypeCache)
				return assemblyCache.GetTypes(requestedType.FullName, GetAssemblies, (a) => GetTypesInAssembly(requestedType, a));
			else
				return base.Find(requestedType);
		}

		public override IEnumerable<AttributedType<TAttribute>> Find<TAttribute>(System.Type requestedType, bool inherit = false)
		{
			if (enableTypeCache)
			{
				string key = requestedType.FullName + "[" + typeof(TAttribute).FullName + "]";
				return assemblyCache.GetTypes(key, GetAssemblies, (a) =>
					GetTypesInAssembly(requestedType, a).SelectMany(t => GetTypesWithAttribute<TAttribute>(t, inherit)))
					.SelectMany(t => SelectAttributedTypes<TAttribute>(t, inherit));
			}
			else
				return base.Find<TAttribute>(requestedType, inherit);
		}

		protected static IEnumerable<Type> GetTypesWithAttribute<TAttribute>(Type type, bool inherit)
		{
			if (type.GetCustomAttributes(typeof(TAttribute), inherit).OfType<TAttribute>().Any())
				yield return type;
			else
				yield break;
		}

		public override IEnumerable<Assembly> GetAssemblies()
		{
			if (enableTypeCache)
				return assemblyCache.GetAssemblies(GetAssembliesInternal);
			else
				return GetAssembliesInternal();
		}

		private IEnumerable<Assembly> GetAssembliesInternal()
		{
			if (dynamicDiscovery)
			{
				var assemblies = assemblyCache.GetProbingPaths()
					.SelectMany(pp => LoadMatchingAssemblies(pp))
					.ToList();
				
				var addedAssemblyNames = new HashSet<string>(assemblies.Select(a => a.FullName));
				assemblies.AddRange(GetConfiguredAssemblies(addedAssemblyNames));
				return assemblies;
			}

			return base.GetAssemblies();
		} 
		#endregion
	}
}
