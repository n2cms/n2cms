using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace N2.Engine
{
	/// <summary>
	/// A class that finds types needed by N2 by looping assemblies in the 
	/// currently executing AppDomain. Only assemblies whose names matches
	/// certain patterns are investigated and an optional list of assemblies
	/// referenced by <see cref="AssemblyNames"/> are always investigated.
	/// </summary>
	public class AppDomainTypeFinder : ITypeFinder
	{
		#region Private Fields

		private bool loadAppDomainAssemblies = true;

		private Regex assemblySkipLoadingPattern = new Regex("^System|^mscorlib|^Microsoft|^CppCodeProvider|^VJSharpCodeProvider|^WebDev|^Castle|^Iesi|^log4net|^NHibernate|^nunit|^TestDriven|^MbUnit|^Rhino|^QuickGraph|^TestFu|^Telerik|^ComponentArt|^MvcContrib|^AjaxControlToolkit|^Antlr3|^Remotion|^Recaptcha|^Lucene|^Ionic|^HibernatingRhinos|^Spark|^SharpArch|^CommonServiceLocator|^Newtonsoft|^SMDiagnostics|^App_LocalResources|^AntiXSSLibrary|^dotless|^HtmlSanitizationLibrary|^sqlce|^WindowsBase|^Pandora|^PegBase|^DynamicProxyGenAssembly|^Anonymously Hosted DynamicMethods Assembly|^WebActivator|^Deleporter|^Elmah|^Markdown|^SimpleHttpClient|^StructureMap|^WebDriver|^MySql|^App_GlobalResources|^App_global|^App_Web_|^EntityFramework|^WebGrease|^App_global.asax");

		private Regex assemblyRestrictToLoadingPattern = new Regex(".*");
		private IList<string> assemblyNames = new List<string>();

		Logger<AppDomainTypeFinder> logger;

		#endregion

		#region Properties

		/// <summary>Gets or sets wether N2 should iterate assemblies in the app domain when loading N2 types. Loading patterns are applied when loading these assemblies.</summary>
		public bool LoadAppDomainAssemblies
		{
			get { return loadAppDomainAssemblies; }
			set { loadAppDomainAssemblies = value; }
		}

		/// <summary>Gets or sets assemblies loaded a startup in addition to those loaded in the AppDomain.</summary>
		public IList<string> AssemblyNames
		{
			get { return assemblyNames; }
			set { assemblyNames = value; }
		}

		/// <summary>Gets the pattern for dlls that we know don't need to be investigated for content items.</summary>
		public Regex AssemblySkipLoadingPattern
		{
			get { return assemblySkipLoadingPattern; }
			set { assemblySkipLoadingPattern = value; }
		}

		/// <summary>Gets or sets the pattern for dll that will be investigated. For ease of use this defaults to match all but to increase performance you might want to configure a pattern that includes N2 assemblies and your own.</summary>
		/// <remarks>If you change this so that N2 assemblies arn't investigated (e.g. by not including something like "^N2|..." you may break core functionality.</remarks>
		public Regex AssemblyRestrictToLoadingPattern
		{
			get { return assemblyRestrictToLoadingPattern; }
			set { assemblyRestrictToLoadingPattern = value; }
		}

		#endregion

		/// <summary>Finds types assignable from of a certain type in the app domain.</summary>
		/// <param name="requestedType">The type to find.</param>
		/// <returns>A list of types found in the app domain.</returns>
		public virtual IEnumerable<Type> Find(Type requestedType)
		{
			List<Type> types = new List<Type>();
			foreach (Assembly a in GetAssemblies())
			{
				types.AddRange(GetTypesInAssembly(requestedType, a));
			}

			logger.DebugFormat("Loading requested types {0}, found {1}", requestedType, types.Count);

			return types;
		}

		protected static IEnumerable<Type> GetTypesInAssembly(Type requestedType, Assembly a)
		{
			Type[] allTypes;
			try
			{
				allTypes = a.GetTypes();
			}
			catch (ReflectionTypeLoadException ex)
			{
				string loaderErrors = string.Empty;
				foreach (Exception loaderEx in ex.LoaderExceptions)
				{
					Engine.Logger.Error(loaderEx);
					loaderErrors += ", " + loaderEx.Message;
				}

				throw new N2Exception("Error getting types from assembly " + a.FullName + loaderErrors, ex);
			}

			foreach (Type t in allTypes)
			{
				if (requestedType.IsAssignableFrom(t))
					yield return t;
			}
		}

		public virtual IEnumerable<AttributedType<TAttribute>> Find<TAttribute>(Type requestedType, bool inherit = false) where TAttribute : class
		{
			return Find(requestedType)
				.SelectMany(t => SelectAttributedTypes<TAttribute>(t, inherit));
		}

		protected static IEnumerable<AttributedType<TAttribute>> SelectAttributedTypes<TAttribute>(Type type, bool inherit) where TAttribute : class
		{
			return type.GetCustomAttributes(typeof(TAttribute), inherit)
				.OfType<TAttribute>()
				.Select(a => new AttributedType<TAttribute> { Type = type, Attribute = a });
		}

		/// <summary>Gets tne assemblies related to the current implementation.</summary>
		/// <returns>A list of assemblies that should be loaded by the N2 factory.</returns>
		public virtual IEnumerable<Assembly> GetAssemblies()
		{
			var addedAssemblyNames = new HashSet<string>();
			List<Assembly> assemblies = new List<Assembly>();

			logger.Info("Getting assemblies");

			if (LoadAppDomainAssemblies)
			{
				assemblies.AddRange(GetAssembliesInAppDomain(addedAssemblyNames));
			}

			assemblies.AddRange(GetConfiguredAssemblies(addedAssemblyNames));

			return assemblies;
		}

		/// <summary>Iterates all assemblies in the AppDomain and if it's name matches the configured patterns add it to our list.</summary>
		/// <param name="previouslyAddedAssemblyNames"></param>
		/// <param name="assemblies"></param>
		protected IEnumerable<Assembly> GetAssembliesInAppDomain(HashSet<string> previouslyAddedAssemblyNames)
		{
			var assemblies = new List<Assembly>();
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (Matches(assembly.FullName))
				{
					if (!previouslyAddedAssemblyNames.Contains(assembly.FullName))
					{
						logger.InfoFormat("Adding {0}", assembly.FullName);

						assemblies.Add(assembly);
						previouslyAddedAssemblyNames.Add(assembly.FullName);
					}
				}
			}

			logger.InfoFormat("Added {0} assemblies in app domain", assemblies.Count);
			return assemblies;
		}

		/// <summary>Adds specificly configured assemblies.</summary>
		protected virtual IEnumerable<Assembly> GetConfiguredAssemblies(HashSet<string> previouslyAddedAssemblyNames)
		{
			var assemblies = new List<Assembly>();
			foreach (string assemblyName in AssemblyNames)
			{
				if (previouslyAddedAssemblyNames.Contains(assemblyName))
					continue;

				logger.Debug("Loading " + assemblyName);
				Assembly assembly = Assembly.Load(assemblyName);
				if (previouslyAddedAssemblyNames.Contains(assembly.FullName))
					continue;
				
				assemblies.Add(assembly);
				previouslyAddedAssemblyNames.Add(assembly.FullName);
			}

			logger.InfoFormat("Added {0} configured assemblies", assemblies.Count);
			return assemblies;
		}

		/// <summary>Check if a dll is one of the shipped dlls that we know don't need to be investigated.</summary>
		/// <param name="assemblyFullName">The name of the assembly to check.</param>
		/// <returns>True if the assembly should be loaded into N2.</returns>
		public virtual bool Matches(string assemblyFullName)
		{
			return !AssemblySkipLoadingPattern.IsMatch(assemblyFullName)
				&& AssemblyRestrictToLoadingPattern.IsMatch(assemblyFullName);
		}

		/// <summary>Makes sure matching assemblies in the supplied folder are loaded in the app domain.</summary>
		/// <param name="directoryPath">The physical path to a directory containing dlls to load in the app domain.</param>
		protected virtual IEnumerable<Assembly> LoadMatchingAssemblies(string directoryPath)
		{
			if (!Directory.Exists(directoryPath)) 
			{
				logger.InfoFormat("Probing path doesn't exist: {0}", directoryPath);
				return new Assembly[0];
			}
			var dlls = Directory.GetFiles(directoryPath, "*.dll");
			logger.DebugFormat("Analyzing {0} dlls in path {1}", dlls.Length, directoryPath);

			var assemblies = new List<Assembly>();
			foreach (string dllPath in dlls)
			{
				try
				{
					string assumedAssemblyName = Path.GetFileNameWithoutExtension(dllPath);
					if (Matches(assumedAssemblyName))
					{
						logger.Debug("Loading " + assumedAssemblyName);
						var assembly = AppDomain.CurrentDomain.Load(assumedAssemblyName);
						if (assembly != null)
							assemblies.Add(assembly);
					}
					else
						logger.Debug("Skipping " + assumedAssemblyName);
				}
				catch (BadImageFormatException ex)
				{
					logger.Error(ex);
				}
			}
			return assemblies;
		}
	}
}
