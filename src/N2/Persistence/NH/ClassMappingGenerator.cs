using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions;
using System.Reflection;
using N2.Persistence;

namespace N2.Persistence.NH
{
    /// <summary>
    /// Generates nhibernate mappings for item types.
    /// </summary>
    public class ClassMappingGenerator
    {
        readonly IDefinitionManager definitions;
        private string classFormat = @"<subclass name=""{0}"" extends=""{1}"" discriminator-value=""{2}"" lazy=""false"">{3}</subclass>";

        public ClassMappingGenerator(IDefinitionManager definitions)
        {
            this.definitions = definitions;
        }

        public virtual string GetMapping(Type type)
        {
            string typeName = GetName(type);
            string discriminator = GetDiscriminator(type);
            string parentName = GetName(GetFirstSuitableBaseType(type.BaseType));
            string properties = GetProperties(type);

            return string.Format(classFormat, typeName, parentName, discriminator, properties);
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

        private string GetDiscriminator(Type itemType)
        {
            ItemDefinition definition = definitions.GetDefinition(itemType);
            if (definition != null)
                return definition.Discriminator;
            else
                return itemType.Name;
        }
    }

}
