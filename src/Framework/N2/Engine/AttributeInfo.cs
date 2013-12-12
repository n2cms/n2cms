using System;

namespace N2.Engine
{
    /// <summary>
    /// Maps an attribute to the type it decorates.
    /// </summary>
    /// <typeparam name="T">The type of attribute.</typeparam>
    public class AttributeInfo<T>
    {
        /// <summary>An attribute retrieved from a type descriptor.</summary>
        public T Attribute { get; set; }

        /// <summary>The particular type the attribute was describing.</summary>
        public Type DecoratedType { get; set; }

        public override string ToString()
        {
            return "[" + typeof(T).Name + "] " + DecoratedType != null ? DecoratedType.Name : "(null)";
        }
    }
}
