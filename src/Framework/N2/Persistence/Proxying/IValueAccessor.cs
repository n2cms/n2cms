using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace N2.Persistence.Proxying
{
    public class ValueAccessorContext
    {
        public IInterceptableType Instance { get; set; }
        public PropertyInfo Property { get; set; }
        public Func<object> BackingPropertyGetter { get; set; }
        public Action<object> BackingPropertySetter { get; set; }
    }

    /// <summary>
    /// When used on an attriute decorating a class, this interface is called to get and set the value
    /// on the class instance. This is used for accessing by [EditableItem] and [ditableChildren] to
    /// retrieve values from the child collection.
    /// </summary>
    public interface IValueAccessor
    {
        /// <summary>Gets the value from the context's instance.</summary>
        /// <param name="context">The context containing the instance to get the value from.</param>
        /// <param name="propertyName">The property whose value to get.</param>
        /// <param name="value">The value to get.</param>
        /// <returns>The value of the property.</returns>
        object GetValue(ValueAccessorContext context, string propertyName);
        
        /// <summary>Sets the value on the context's instance.</summary>
        /// <param name="context">The context containing the instance to apply the value on.</param>
        /// <param name="propertyName">The property to apply the value to.</param>
        /// <param name="value">The value to apply.</param>
        /// <returns>True if the value resulted in changes to the item.</returns>
        bool SetValue(ValueAccessorContext context, string propertyName, object value);
    }
}
