using System;
using System.Collections.Generic;
using System.Reflection;
using N2.Details;
using N2.Engine;
using N2.Security;

namespace N2.Definitions
{
    /// <summary>
    /// Finds uniquely named attributes on a type. Attributes defined on the 
    /// class must have their name attribute set, and attributes defined on a
    /// property their name set to the property's name.
    /// </summary>
    [Service]
    public class AttributeExplorer
    {
        /// <summary>
        /// Finds attributes on a type.
        /// </summary>
        /// <typeparam name="T">The type of attribute to find.</typeparam>
        /// <param name="typeToExplore">The type to explore</param>
        /// <returns>A list of attributes defined on the class or it's properties.</returns>
        public IList<T> Find<T>(Type typeToExplore, bool onClass = true, bool onProperties = true) where T : IUniquelyNamed
        {
            List<T> attributes = new List<T>();

            if (onProperties)
                AddEditablesDefinedOnProperties(typeToExplore, attributes);
            if (onClass)
                AddEditablesDefinedOnClass(typeToExplore, attributes);

            attributes.Sort(
                (f, s) =>
                {
                    if (f is IComparable<T>)
                        return (f as IComparable<T>).CompareTo(s);
                    if (s is IComparable<T>)
                        return -(s as IComparable<T>).CompareTo(f);
                    return 0;
                });

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
                    editableOnProperty.Name = propertyOnItem.Name;
                    
                    if (attributes.Contains(editableOnProperty))
                        continue;
                    
                    attributes.Add(editableOnProperty);
                }
            }
        }

        private static void AddEditablesDefinedOnClass<T>(Type exploredType, ICollection<T> attributes) where T : IUniquelyNamed
        {
            foreach (Type t in EnumerateTypeAncestralHierarchy(exploredType))
            {
                foreach (T editableOnClass in t.GetCustomAttributes(typeof (T), true))
                {
                    if (editableOnClass.Name == null)
                        continue;

                    if (attributes.Contains(editableOnClass))
                        continue;

                    attributes.Add(editableOnClass);
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
