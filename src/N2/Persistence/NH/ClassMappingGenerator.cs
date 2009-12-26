using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions;
using System.Reflection;
using N2.Persistence;
using System.Diagnostics;

namespace N2.Persistence.NH
{
    /// <summary>
    /// Generates nhibernate mappings for item types.
    /// </summary>
    public class ClassMappingGenerator
    {
        private string classFormat = @"<subclass name=""{0}"" extends=""{1}"" discriminator-value=""{2}"" lazy=""false"">{3}</subclass>";

        /// <summary>Gets the mapping xml for a type</summary>
        /// <param name="definition">The type to generate mapping for</param>
        /// <param name="allDefinitions">All definitions in the system.</param>
        /// <returns>An xml string</returns>
        public virtual string GetMapping(ItemDefinition definition, ICollection<ItemDefinition> allDefinitions)
        {
            string typeName = GetName(definition.ItemType);
            string discriminator = GetDiscriminator(definition);
            string parentName = GetParent(definition, allDefinitions);
            string properties = GetProperties(definition.ItemType);

            Trace.WriteLine("Generating mapping for {type = " + definition.ItemType + ", discriminator=" + discriminator + ", parent: " + parentName + ", properties: " + properties.Length + "}");
            return string.Format(classFormat, typeName, parentName, discriminator, properties);
        }

        private string GetParent(ItemDefinition currentDefinition, ICollection<ItemDefinition> allDefinitions)
        {
            // try to find a defined item in the type tree
            foreach (var baseType in Utility.GetBaseTypes(currentDefinition.ItemType))
            {
                foreach (var definition in allDefinitions)
                {
                    if (definition.ItemType == baseType)
                    {
                        return GetName(baseType);
                    }
                }
            }
            // or revert to content item
            return GetName(typeof(ContentItem));
        }

        private string GetProperties(Type attributedType)
        {
            string properties = "";
            foreach (PropertyInfo info in attributedType.GetProperties())
            {
                foreach (PersistableAttribute attribute in info.GetCustomAttributes(typeof(PersistableAttribute), false))
                {
                    properties += attribute.GenerateMapping(info);
                }
            }
            return properties;
        }

        private static string GetName(Type t)
        {
            return t.FullName + ", " + t.Assembly.FullName.Split(',')[0];
        }

        private Type GetFirstSuitableBaseType(Type itemType)
        {
            if (itemType == typeof(ContentItem))
                return itemType;
            if (GeneratorHelper.IsUnsuiteableForMapping(itemType))
                return GetFirstSuitableBaseType(itemType.BaseType);

            return itemType;
        }

        private string GetDiscriminator(ItemDefinition definition)
        {
            if (definition != null)
                return definition.Discriminator;
            else
                return definition.ItemType.Name;
        }
    }

}
