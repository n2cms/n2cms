using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace N2.Configuration
{
    /// <summary>
    /// Services to register instead of/in addition to existing N2 services.
    /// </summary>
    [ConfigurationCollection(typeof(ComponentElement))]
    public class ComponentCollection : LazyRemovableCollection<ComponentElement>
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ComponentElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            ComponentElement se = element as ComponentElement;
            
            if (!string.IsNullOrEmpty(se.Service))
                return se.Service;
            
            return se.Implementation;
        }

        public virtual IEnumerable<Type> GetConfiguredServiceTypes()
        {
            return new HashSet<Type>(AllElements.Where(c => c.PreventDefault).Select(c => Type.GetType(c.Service)).Where(t => t != null));
        }
    }
}
