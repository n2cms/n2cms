using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions;
using System.Reflection;
using N2.Persistence;
using System.Diagnostics;
using N2.Engine;

namespace N2.Persistence.NH
{
    /// <summary>
    /// Generates nhibernate mappings for item types.
    /// </summary>
	[Service]
    public class ClassMappingGenerator
    {
        private string classFormat = @"<subclass name=""{0}"" extends=""{1}"" discriminator-value=""{2}"" lazy=""false"">{3}</subclass>";

        /// <summary>Gets the mapping xml for a type</summary>
        /// <param name="definition">The type to generate mapping for</param>
        /// <param name="allDefinitions">All definitions in the system.</param>
        /// <returns>An xml string</returns>
        public virtual string GetMapping(Type entityType, Type parentType, string discriminator)
        {
			string typeName = GetName(entityType);
			string parentName = GetName(parentType);
            string properties = GetProperties(entityType);

            Trace.WriteLine("Generating mapping for {type = " + entityType + ", discriminator=" + discriminator + ", parent: " + parentName + ", properties: " + properties.Length + "}");
            return string.Format(classFormat, typeName, parentName, discriminator, properties);
        }

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
    }

}
