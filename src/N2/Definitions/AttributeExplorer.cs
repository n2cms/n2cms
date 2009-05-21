using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using N2.Details;
using N2.Security;

namespace N2.Definitions
{
	/// <summary>
	/// Finds uniquely named attributes on a type. Attributes defined on the 
	/// class must have their name attribute set, and attributes defined on a
	/// property their name set to the property's name.
	/// </summary>
	public class AttributeExplorer
	{
		/// <summary>
		/// Finds attributes on a type.
		/// </summary>
		/// <typeparam name="T">The type of attribute to find.</typeparam>
		/// <param name="typeToExplore">The type to explore</param>
		/// <returns>A list of attributes defined on the class or it's properties.</returns>
		public IList<T> Find<T>(Type typeToExplore) where T : IUniquelyNamed
		{
			List<T> attributes = new List<T>();

			AddEditablesDefinedOnProperties(typeToExplore, attributes);
			AddEditablesDefinedOnClass(typeToExplore, attributes);

			if (attributes.Count > 1 && (attributes[0] is IComparable || attributes[0] is IComparable<T>))
				attributes.Sort();

			return attributes;
		}

        /// <summary>
        /// Maps properties on the class and it's properties to a dictionary.
        /// </summary>
		/// <typeparam name="T">The type of attribute to find.</typeparam>
		/// <param name="typeToExplore">The type to explore.</param>
        /// <returns>A dictionary of atributes.</returns>
		public IDictionary<string, T> Map<T>(Type typeToExplore) where T : IUniquelyNamed
		{
			IList<T> attributes = Find<T>(typeToExplore);
			Dictionary<string, T> map = new Dictionary<string, T>();
			foreach(T a in attributes)
			{
				map[a.Name] = a;
			}
			return map;
		}

		#region Helpers
		private static void AddEditablesDefinedOnProperties<T>(Type exploredType, ICollection<T> attributes) where T : IUniquelyNamed
		{
			foreach (PropertyInfo propertyOnItem in exploredType.GetProperties())
			{
				foreach (T editableOnProperty in propertyOnItem.GetCustomAttributes(typeof(T), false))
				{
					if (!attributes.Contains(editableOnProperty))
					{
						editableOnProperty.Name = propertyOnItem.Name;
						if (editableOnProperty is ISecurable)
						{
							foreach (DetailAuthorizedRolesAttribute rolesAttribute in propertyOnItem.GetCustomAttributes(typeof (DetailAuthorizedRolesAttribute), false))
							{
								ISecurable s = editableOnProperty as ISecurable;
								s.AuthorizedRoles = rolesAttribute.Roles;
							}
						}
						attributes.Add(editableOnProperty);
					}
				}
			}
		}

		private static void AddEditablesDefinedOnClass<T>(Type exploredType, ICollection<T> attributes) where T : IUniquelyNamed
		{
			foreach (Type t in EnumerateTypeAncestralHierarchy(exploredType))
			{
				foreach (T editableOnClass in t.GetCustomAttributes(typeof (T), true))
				{
					if (!attributes.Contains(editableOnClass))
					{
						if (editableOnClass.Name == null)
							throw new N2Exception(
								"The attribute {0} does not have a Name defined. Since it's defined on the class instead of a property it must have a name.",
								editableOnClass);

						if (!attributes.Contains(editableOnClass))
							attributes.Add(editableOnClass);
					}
				}
			}
		}

		private static IEnumerable<Type> EnumerateTypeAncestralHierarchy(Type type)
		{
			while (type != typeof(object))
			{
				yield return type;
				type = type.BaseType;
			}
		}
		#endregion
	}
}
