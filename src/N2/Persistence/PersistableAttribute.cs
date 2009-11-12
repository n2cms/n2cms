using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace N2.Persistence
{
    /// <summary>
    /// Instructs the database mapping to generate store the property in a
    /// additional column with the same name in the n2item table. Adding this 
    /// attribute to an existing system requires the database to be updated.
    /// </summary>
    /// <example>
    /// [PageDefinition]
    /// public class PropertyItemType : ContentItem
    /// {
    ///     [Persistable, EditableTextBox("Author", 80)]
    ///     public virtual string Author { get; set; }
    /// }
    /// </example>
    [AttributeUsage(AttributeTargets.Property)]
    public class PersistableAttribute : Attribute
    {
        /// <summary>This property is required, i.i. not nullable.</summary>
        bool Required { get; set; }
        
        /// <summary>The length of this column (usually for string properties)</summary>
        public int Length { get; set; }

        /// <summary>Generates the mapping xml for this property.</summary>
        /// <param name="info">The property the attribute was added to.</param>
        /// <returns>An hbm xml snippet.</returns>
        public virtual string GenerateMapping(PropertyInfo info)
        {
            const string format = "<property name=\"{0}\" column=\"{1}\" type=\"{2}\" not-null=\"{3}\" length=\"{4}\" />";

            bool isNullable = (info.PropertyType.IsGenericType && info.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)));
            string typeName = isNullable
                ? info.PropertyType.GetGenericArguments()[0].Name
                : info.PropertyType.Name;
            bool notNull = Required || (info.PropertyType.IsValueType && !isNullable);
            string length = Length > 0 ? Length.ToString() : "{StringLength}";

            return string.Format(format, info.Name, info.Name, typeName, notNull.ToString().ToLower(), length);
        }
    }
}
