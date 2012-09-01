using System.Linq;
using System.Configuration;
using N2.Engine;
using System.Collections.Generic;

namespace N2.Configuration
{
	public class NameCollection : LazyRemovableCollection<NamedElement>
	{
	}

    /// <summary>
    /// Configuration related to inversion of control and the dynamic aspects 
    /// of n2 definition.
    /// </summary>
	public class EngineSection : ContentConfigurationSectionBase
	{
		/// <summary>A custom <see cref="IEngine"/> to manage the application instead of the default.</summary>
		[ConfigurationProperty("engineType")]
		public string EngineType
		{
			get { return (string)base["engineType"]; }
			set { base["engineType"] = value; }
		}

		/// <summary>A custom <see cref="IServiceContainer"/> to manage the application instead of the default.</summary>
		[ConfigurationProperty("containerType")]
		public string ContainerType
		{
			get { return (string)base["containerType"]; }
			set { base["containerType"] = value; }
		}

		[ConfigurationProperty("castleConfiguration", DefaultValue = "assembly://N2/Configuration/castle.config")]
		public string CastleConfiguration
		{
			get { return (string)base["castleConfiguration"]; }
			set { base["castleConfiguration"] = value; }
		}

		/// <summary>In addition to configured assemblies examine and load assemblies in the bin directory.</summary>
		[ConfigurationProperty("dynamicDiscovery", DefaultValue = true)]
		public bool DynamicDiscovery
		{
			get { return (bool)base["dynamicDiscovery"]; }
			set { base["dynamicDiscovery"] = value; }
        }

        /// <summary>Additional assemblies assemblies investigated while investigating the environemnt, e.g. to find item definitions.</summary>
		[ConfigurationProperty("assemblies")]
		public AssemblyCollection Assemblies
		{
			get { return (AssemblyCollection)base["assemblies"]; }
            set { base["assemblies"] = value; }
        }

        [ConfigurationProperty("errors")]
        public ErrorsElement Errors
        {
            get { return (ErrorsElement)base["errors"]; }
            set { base["errors"] = value; }
        }

        [ConfigurationProperty("globalization")]
        public GlobalizationElement Globalization
        {
            get { return (GlobalizationElement)base["globalization"]; }
            set { base["globalization"] = value; }
        }

        /// <summary>Scheduler related configuration.</summary>
        [ConfigurationProperty("scheduler")]
        public SchedulerElement Scheduler
        {
            get { return (SchedulerElement)base["scheduler"]; }
            set { base["scheduler"] = value; }
        }

        /// <summary>A collection of services that are registered in the container before the default ones. This is a place through which core services can be replaced.</summary>
        [ConfigurationProperty("components")]
        public ComponentCollection Components
        {
            get { return (ComponentCollection)base["components"]; }
            set { base["components"] = value; }
		}

		/// <summary>Component configuration keys used register further services.</summary>
		[ConfigurationProperty("componentConfigurations")]
		public NameCollection ComponentConfigurations
		{
			get { return (NameCollection)base["componentConfigurations"]; }
			set { base["componentConfigurations"] = value; }
		}

		/// <summary>Add or remove plugin initializers. This is most commonly used to remove automatic plugin initializers in an external assembly.</summary>
		[ConfigurationProperty("pluginInitializers")]
		public PluginInitializerCollection PluginInitializers
		{
			get { return (PluginInitializerCollection)base["pluginInitializers"]; }
			set { base["pluginInitializers"] = value; }
		}

		/// <summary>Add or remove UI plugins. This is most commonly used to remove unwanted toolbar buttons.</summary>
		[ConfigurationProperty("interfacePlugins")]
		public InterfacePluginCollection InterfacePlugins
		{
			get { return (InterfacePluginCollection)base["interfacePlugins"]; }
			set { base["interfacePlugins"] = value; }
		}

		/// <summary>Add or remove item definitions. This is most commonly used to prevent unwanted item definitions appearing.</summary>
		[ConfigurationProperty("definitions")]
		public DefinitionCollection Definitions
		{
			get { return (DefinitionCollection)base["definitions"]; }
			set { base["definitions"] = value; }
		}

		public override void ApplyComponentConfigurationKeys(List<string> configurationKeys)
		{
			configurationKeys.Add(GetTrustLevelComponentConfigurationKey());

			configurationKeys.AddRange(ComponentConfigurations.AddedElements.Select(e => e.Name));
			configurationKeys.RemoveAll(c => ComponentConfigurations.RemovedElements.Any(e => c == e.Name));
		}

		private static string GetTrustLevelComponentConfigurationKey()
		{
			return (Utility.GetTrustLevel() > System.Web.AspNetHostingPermissionLevel.Medium)
							? ConfigurationKeys.FullTrust
							: ConfigurationKeys.MediumTrust;
		}

		/// <summary>
		/// Known configuration keys used to configure services.
		/// </summary>
		private static class ConfigurationKeys
		{
			/// <summary>Key used to configure services intended for medium trust.</summary>
			public const string MediumTrust = "MediumTrust";
			/// <summary>Key used to configure services intended for full trust.</summary>
			public const string FullTrust = "FullTrust";
		}
	}
}
