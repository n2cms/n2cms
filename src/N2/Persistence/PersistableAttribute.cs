using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using N2.Persistence.Proxying;

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
	public class PersistableAttribute : Attribute, IInterceptableProperty
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

        /// <summary>Generates the mapping xml for this property.</summary>
        /// <param name="info">The property the attribute was added to.</param>
        /// <returns>An hbm xml snippet.</returns>
        public virtual string GenerateMapping(PropertyInfo info)
        {
            const string propertyFormat = "<property name=\"{0}\" column=\"{1}\" type=\"{2}\" length=\"{3}\" />";
			const string relationFormat = "<many-to-one name=\"{0}\" column=\"{1}\" class=\"{2}\" not-null=\"false\" />";

			string columnName = Column ?? info.Name;
			string length = Length > 0 ? Length.ToString() : "{StringLength}";
            bool isNullable = (info.PropertyType.IsGenericType && info.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)));
			var type = isNullable
                ? info.PropertyType.GetGenericArguments()[0]
                : info.PropertyType;
			string typeName = type.FullName + ", " + info.PropertyType.Assembly.FullName.Split(',')[0];

			string format = propertyFormat;
			if (typeof(ContentItem).IsAssignableFrom(type))
				format = relationFormat;
			return string.Format(format, info.Name, columnName, typeName, length);
        }

		#region IInterceptableProperty Members

		object IInterceptableProperty.DefaultValue
		{
			get { return null; }
		}

		#endregion
	}
}
