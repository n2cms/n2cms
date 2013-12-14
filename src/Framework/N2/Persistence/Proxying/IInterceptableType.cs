using System;
using System.Collections;
using Castle.DynamicProxy;
using System.Collections.Generic;

namespace N2.Persistence.Proxying
{
    /// <summary>
    /// Marks a type available for interception.
    /// </summary>
    public interface IInterceptableType
    {
        /// <summary>The actual content type the proxied object represents.</summary>
        /// <returns>The actual type.</returns>
        Type GetContentType();

        /// <summary>Sets a value to the underlying detail store.</summary>
        /// <param name="detailName">The name of the detail.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="valueType">The type of value to assign.</param>
        void SetValue(string detailName, object value, Type valueType);

        /// <summary>Gets a value from the underlying detail store.</summary>
        /// <param name="detailName">The name of the detail.</param>
        /// <returns>The value of the given detial name or null.</returns>
        object GetValue(string detailName);

        /// <summary>Sets values to the underlying detail store.</summary>
        /// <param name="detailCollectionName">The name of the detail.</param>
        /// <param name="values">The values to set.</param>
        void SetValues(string detailCollectionName, IEnumerable values);

        /// <summary>Gets values from the underlying detail store.</summary>
        /// <param name="detailCollectionName">The name of the detail.</param>
        /// <returns>The value of the given detial name or null.</returns>
        IEnumerable GetValues(string detailCollectionName);

        object GetChild(string childName);

        void SetChild(string childName, object child);

        IEnumerable GetChildren(string zoneName);

        void SetChildren(string zoneName, IEnumerable children);
    }
}
