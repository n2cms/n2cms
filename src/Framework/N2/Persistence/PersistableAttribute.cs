using System;
using System.Reflection;
using N2.Persistence.Proxying;
using NHibernate.Cfg.MappingSchema;
using N2.Definitions;
using N2.Collections;

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
    public class PersistableAttribute : Attribute, IInterceptableProperty, IUniquelyNamed, IPersistableProperty
    {
        public PersistableAttribute()
        {
            PersistAs = PropertyPersistenceLocation.Column;
        }

        /// <summary>The length of this column (usually for string properties)</summary>
        public int Length { get; set; }

        /// <summary>An alternative name of the column (optional).</summary>
        public string Column { get; set; }

        /// <summary>Where to store this property (default is column).</summary>
        public PropertyPersistenceLocation PersistAs { get; set; }

        public object GetPropertyMapping(PropertyInfo info, Func<string, string> formatter)
        {
            string columnName = Column ?? info.Name;
            string length = Length > 0 ? Length.ToString() : "{StringLength}";
            bool isNullable = (info.PropertyType.IsGenericType && info.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)));
            var propertyType = isNullable
                ? info.PropertyType.GetGenericArguments()[0]
                : info.PropertyType;
            string typeName = GetTypeName(propertyType);

            if (typeof(ContentItem).IsAssignableFrom(propertyType))
                return new HbmManyToOne { name = info.Name, column = formatter(columnName), @class = typeName };

            return new HbmProperty { name = info.Name, column = formatter(columnName), type = new HbmType { name = typeName }, length = formatter(length) };
        }

        /// <summary>Generates the mapping xml for this property.</summary>
        /// <param name="info">The property the attribute was added to.</param>
        /// <returns>An hbm xml snippet.</returns>
        [Obsolete("Use GetPropertyMapping", true)]
        public virtual string GenerateMapping(PropertyInfo info)
        {
            const string propertyFormat = "<property name=\"{0}\" column=\"{1}\" type=\"{2}\" length=\"{3}\" />";
            const string relationFormat = "<many-to-one name=\"{0}\" column=\"{1}\" class=\"{2}\" not-null=\"false\" />";

            string columnName = Column ?? info.Name;
            string length = Length > 0 ? Length.ToString() : "{StringLength}";
            bool isNullable = (info.PropertyType.IsGenericType && info.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)));
            var propertyType = isNullable
                ? info.PropertyType.GetGenericArguments()[0]
                : info.PropertyType;
            string typeName = GetTypeName(propertyType);
            string format = propertyFormat;
            if (typeof(ContentItem).IsAssignableFrom(propertyType))
                format = relationFormat;
            return string.Format(format, info.Name, columnName, typeName, length);
        }

        private static string GetTypeName(Type propertyType)
        {
            if(propertyType == typeof(string))
                return "StringClob";
            else if(propertyType == typeof(bool))
                return "Boolean";
            else if(propertyType == typeof(int))
                return "Int32";
            else if(propertyType == typeof(double))
                return "Double";
            else if(propertyType == typeof(DateTime))
                return "DateTime";
            else
                return propertyType.FullName + ", " + propertyType.Assembly.FullName.Split(',')[0];
        }

        #region IInterceptableProperty Members

        object IInterceptableProperty.DefaultValue
        {
            get { return null; }
        }

        #endregion

        #region IUniquelyNamed Members

        string IUniquelyNamed.Name { get; set; }
         
        #endregion

        #region INameable Members

        string INameable.Name { get { return ((IUniquelyNamed)this).Name; } }

        #endregion
    }
}
