using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using N2.Definitions.Static;
using N2.Engine;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;

namespace N2.Persistence.NH
{
    /// <summary>
    /// Generates nhibernate mappings for item types.
    /// </summary>
	[Service]
    public class ClassMappingGenerator
    {
		private readonly Engine.Logger<ClassMappingGenerator> logger;
		DefinitionMap map;

		public ClassMappingGenerator(DefinitionMap map)
		{
			this.map = map;
		}

        private string classFormat = @"<subclass name=""{0}"" extends=""{1}"" discriminator-value=""{2}"" lazy=""false"">{3}</subclass>";

        /// <summary>Gets the mapping xml for a type</summary>
        /// <param name="definition">The type to generate mapping for</param>
        /// <param name="allDefinitions">All definitions in the system.</param>
        /// <returns>An xml string</returns>
		[Obsolete]
        public virtual string GetMapping(Type entityType, Type parentType, string discriminator)
        {
			string typeName = GetName(entityType);
			string parentName = GetName(parentType);
            string properties = GetProperties(entityType);

            logger.Info("Generating mapping for {type = " + entityType + ", discriminator=" + discriminator + ", parent: " + parentName + ", properties: " + properties.Length + "}");
            return string.Format(classFormat, typeName, parentName, discriminator, properties);
        }

		[Obsolete]
        private string GetProperties(Type attributedType)
        {
			StringBuilder properties = new StringBuilder();
            foreach (var p in GetPersistables(attributedType))
            {
				properties.Append(p.Attribute.GenerateMapping(p.DeclaringProperty));
            }
            return properties.ToString();
        }

		private IEnumerable<Pair> GetPersistables(Type attributedType)
		{
			return attributedType.GetProperties()
				.Where(pi => pi.DeclaringType == attributedType)
				.SelectMany(pi => pi.GetCustomAttributes(typeof(PersistableAttribute), false)
					.OfType<PersistableAttribute>()
					.Where(a => a.PersistAs == PropertyPersistenceLocation.Column)
					.Select(a => new Pair { Attribute = a, DeclaringProperty = pi }));
		}

        private static string GetName(Type t)
        {
			return t.FullName + ", " + t.Assembly.FullName.Split(',')[0];
        }

		// helper classes

		class Pair
		{
			public PersistableAttribute Attribute { get; set; }
			public PropertyInfo DeclaringProperty { get; set; }
		}
		
		public virtual void MapTypes(List<Type> allTypes, NHibernate.Cfg.Configuration cfg, Func<string, string> formatter)
		{
			var m = new HbmMapping();
			m.Items = allTypes.Select(t =>
				{
					var sc = new HbmSubclass();
					sc.name = GetName(t);
					sc.extends = GetName(t.BaseType);
					sc.discriminatorvalue = map.GetOrCreateDefinition(t).Discriminator ?? t.Name;
					sc.lazy = false;
					sc.lazySpecified = true;

					var propertyMappings = GetPersistables(t)
						.Select(p => p.Attribute.GetPropertyMapping(p.DeclaringProperty, formatter))
						.ToList();
					if (propertyMappings.Count > 0)
					{
						if (sc.Items == null)
							sc.Items = propertyMappings.ToArray();
						else
							sc.Items = sc.Items.Union(propertyMappings).ToArray();
					}

					return sc;
				}).ToArray();
			if (Debugger.IsAttached)
			{
				var dbg = m.AsString();
			}
			cfg.AddDeserializedMapping(m, "N2");
		}
	}

}
