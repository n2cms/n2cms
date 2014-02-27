using System.Configuration;

namespace N2.Configuration
{
    /// <summary>
    /// A list of mappings that will be added to the NHibernate configuration.
    /// </summary>
    public class MappingCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new MappingElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            MappingElement me = element as MappingElement;
            return me.Name;
        }
    }
}
