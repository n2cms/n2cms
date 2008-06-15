using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Web.Configuration;

namespace N2.Configuration
{
    /// <summary>
    /// Configuration related to inversion of control and the dynamic aspects 
    /// of n2 definition.
    /// </summary>
	public class EngineSection : ConfigurationSection
	{
		[ConfigurationProperty("castleConfiguration", DefaultValue = "assembly://N2/Configuration/castle.config")]
		public string CastleConfiguration
		{
			get { return (string)base["castleConfiguration"]; }
			set { base["castleConfiguration"] = value; }
		}

		[ConfigurationProperty("multipleSitesConfiguration", DefaultValue = "assembly://N2/Web/n2.multiplesites.xml")]
		public string MultipleSitesConfiguration
		{
			get { return (string)base["multipleSitesConfiguration"]; }
			set { base["multipleSitesConfiguration"] = value; }
		}

		/// <summary>In addition to configured assemblies load assemblies in the bin directory..</summary>
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
        }

        ///// <summary>Types not to include when investigating the environment.</summary>
        //[ConfigurationProperty("excludeTypes")]
        //public TypeElementCollection ExcludeTypes
        //{
        //    get { return (TypeElementCollection)base["excludeTypes"]; }
        //}
	}
}
