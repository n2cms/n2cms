using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using N2.Definitions;
using NHibernate;
using NHibernate.Mapping;

namespace N2.Persistence.NH
{
	/// <summary>
	/// Builds NHibernate configuration by reading hbm files and generating 
	/// mappings for item types without hbm.xml mappings files.
	/// </summary>
	public class DefaultConfigurationBuilder : IConfigurationBuilder
	{
		#region Private Fields

		private readonly IDefinitionManager definitions;
		private IDictionary properties = new ListDictionary();
		private IList<Assembly> assemblies = new List<Assembly>();

		private string generatedHbmFormat =
			@"<?xml version=""1.0"" encoding=""utf-16""?>
<hibernate-mapping xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""urn:nhibernate-mapping-2.2"">
    <subclass name=""{0},{1}"" extends=""{2},{3}"" discriminator-value=""{4}"">
    </subclass>
</hibernate-mapping>
";

		private string defaultMapping = "N2.Mappings.Default.hbm.xml,N2";
		#endregion

		#region Properties

		/// <summary>Gets assemblies that will be added to the NHibernate configuration.</summary>
		public IList<Assembly> Assemblies
		{
			get { return assemblies; }
			set { assemblies = value; }
		}

		/// <summary>Gets or sets a hbm format stirng used when generating item class mappings.</summary>
		public string GeneratedHbmFormat
		{
			get { return generatedHbmFormat; }
			set { generatedHbmFormat = value; }
		}

		/// <summary>Gets NHibernate configuration properties.</summary>
		public IDictionary Properties
		{
			get { return properties; }
			set { properties = value; }
		}

		/// <summary>Gets or sets a path to an embedded mapping file that are always added to the NHibernate configuration. Items should be in manifest resource stream format followed by a comma and the assembly name, e.g. "N2.Mappings.MySQL.hbm.xml,N2".</summary>
		public string DefaultMapping
		{
			get { return defaultMapping; }
			set { defaultMapping = value; }
		}

		#endregion

		#region Constructor

		/// <summary>Creates a new instance of the <see cref="DefaultConfigurationBuilder"/>.</summary>
		public DefaultConfigurationBuilder(IDefinitionManager definitions)
		{
			this.definitions = definitions;
		}

		#endregion

		#region Methods

		/// <summary>Builds a <see cref="NHibernate.Cfg.Configuration"/> by adding properties, default assemblies and generating class mappings for unmapped types.</summary>
		/// <returns></returns>
		public virtual NHibernate.Cfg.Configuration BuildConfiguration()
		{
			Debug.WriteLine("DefaultConfigurationBuilder: Building configuration");
			NHibernate.Cfg.Configuration cfg = new NHibernate.Cfg.Configuration();
			AddProperties(cfg);
			AddDefaultMappings(cfg);
			AddAssemblies(cfg);
			GenerateMappings(cfg);
			Debug.WriteLine("DefaultConfigurationBuilder: Built configuration");

			return cfg;
		}

		/// <summary>Adds known mappings to the configuration.</summary>
		/// <param name="cfg">The configuration to add the mappings to.</param>
		protected virtual void AddDefaultMappings(NHibernate.Cfg.Configuration cfg)
		{
			if(!string.IsNullOrEmpty(DefaultMapping))
			{
				string[] pathAssemblyPair = DefaultMapping.Split(',');
				if (pathAssemblyPair.Length != 2) throw new ArgumentException( "Expected the property DefaultMapping to be in the format [manifest resource path],[assembly name] but was: " + DefaultMapping);

				Assembly a = Assembly.Load(pathAssemblyPair[1]);
				cfg.AddInputStream(a.GetManifestResourceStream(pathAssemblyPair[0]));
			}
		}

		/// <summary>Generates subclasses nhibernate xml configuration as an alternative to NHibernate definition file and adds them to the configuration.</summary>
		/// <param name="cfg">The nhibernate configuration to build.</param>
		protected virtual void GenerateMappings(NHibernate.Cfg.Configuration cfg)
		{
			foreach(ItemDefinition definition in definitions.GetDefinitions())
			{
				if (!IsMapped(cfg, definition.ItemType))
				{
					foreach (Type t in EnumerateBaseTypes(definition.ItemType))
					{
						if (t.IsSubclassOf(typeof (ContentItem)))
						{
							if (!IsMapped(cfg, t))
							{
								Stream hbmXmlStream = t.Assembly.GetManifestResourceStream(t.FullName + ".hbm.xml");
								if (hbmXmlStream == null)
								{
									AddGeneratedClassMapping(cfg, t);
								}
								else
								{
									using (hbmXmlStream)
									{
										cfg.AddInputStream(hbmXmlStream);
									}
								}
							}
						}
					}
				}
			}
		}

		/// <summary>Enumerates base type chain of the supplied type.</summary>
		/// <param name="t">The type whose base types will be enumerated.</param>
		/// <returns>An enumerator of the supplied item and all it's base types.</returns>
		protected static IEnumerable<Type> EnumerateBaseTypes(Type t)
		{
			if (t != null)
			{
				foreach (Type baseType in EnumerateBaseTypes(t.BaseType))
					yield return baseType;
				yield return t;
			}
		}

		/// <summary>Adds default assemblies to NHibernate configuration.</summary>
		/// <param name="cfg"></param>
		protected virtual void AddAssemblies(NHibernate.Cfg.Configuration cfg)
		{
			foreach (Assembly a in Assemblies)
				cfg.AddAssembly(a);
			Debug.WriteLine(String.Format("Added {0} assemblies to configuration", Assemblies.Count));
		}

		/// <summary>Adds properties to NHibernate configuration.</summary>
		/// <param name="cfg"></param>
		protected virtual void AddProperties(NHibernate.Cfg.Configuration cfg)
		{
			cfg.AddProperties(Properties);
		}

		/// <summary>Builds a configuration and returns a new <see cref="NHibernate.ISessionFactory"/></summary>
		/// <returns>A new <see cref="NHibernate.ISessionFactory"/>.</returns>
		public ISessionFactory BuildSessionFactory()
		{
			Debug.WriteLine("Building Session Factory");
			return BuildConfiguration().BuildSessionFactory();
		}

		/// <summary>Checks whether a type's mapping is added to the NHibernate configuration.</summary>
		/// <param name="cfg">The nhibernate configuration to investigate.</param>
		/// <param name="type">The type to look for</param>
		/// <returns>True if the type is mapped</returns>
		protected virtual bool IsMapped(NHibernate.Cfg.Configuration cfg, Type type)
		{
			foreach (PersistentClass mapping in cfg.ClassMappings)
				if (mapping.MappedClass == type)
					return true;
			return false;
		}

		/// <summary>Generates the configuration xml for a subclass without any properties and adds it to the NHibernate configuration.</summary>
		/// <param name="cfg">The current nhhibernate configuration.</param>
		/// <param name="itemType">The type to to generate a subclassed NHibernate hbm xml for.</param>
		protected virtual void AddGeneratedClassMapping(NHibernate.Cfg.Configuration cfg, Type itemType)
		{
			string discriminator = GetDiscriminator(itemType);
			string xml = string.Format(GeneratedHbmFormat,
			                           itemType.FullName,
			                           itemType.Assembly.FullName.Split(',')[0],
			                           itemType.BaseType.FullName,
									   itemType.BaseType.Assembly.FullName.Split(',')[0],
			                           discriminator);
			cfg.AddXml(xml);
		}

		private string GetDiscriminator(Type itemType)
		{
			ItemDefinition definition = definitions.GetDefinition(itemType);
			if (definition != null)
				return definition.Discriminator;
			else
				return itemType.Name;
		}

		#endregion
	}
}