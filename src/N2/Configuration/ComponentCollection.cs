using System.Configuration;

namespace N2.Configuration
{
    /// <summary>
    /// Services to register instead of/in addition to existing N2 services.
    /// </summary>
    public class ComponentCollection : ConfigurationElementCollection
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
    }
}