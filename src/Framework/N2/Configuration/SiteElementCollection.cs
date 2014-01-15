using System.Configuration;

namespace N2.Configuration
{
    [ConfigurationCollection(typeof(SiteElement))]
    public class SiteElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new SiteElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SiteElement)element).Name;
        }

        public void Add(SiteElement site)
        {
            BaseAdd(site);
        }

        public void Remove(string siteName)
        {
            BaseRemove(siteName);
        }
    }
}
