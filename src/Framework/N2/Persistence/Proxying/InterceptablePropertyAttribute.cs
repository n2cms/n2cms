using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;

namespace N2.Persistence.Proxying
{
    /// <summary>
    /// Attribute used by the proxying system to enable proxying a property to a detail.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class InterceptablePropertyAttribute : Attribute, IInterceptableProperty
    {
        public InterceptablePropertyAttribute()
        {
            PersistAs = PropertyPersistenceLocation.Detail;
        }

        public PropertyPersistenceLocation PersistAs { get; set; }

        public object DefaultValue { get; set; }
    }
}
