using System.Collections.Generic;
using System.Configuration;

namespace N2.Configuration
{
    public class ComponentParameterCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ComponentParameterElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var pe = (ComponentParameterElement) element;

            return pe.Name;
        }

        public IDictionary<string, string> ToDictionary()
        {
            var result = new Dictionary<string, string>();

            foreach(ComponentParameterElement value in this)
            {
                result.Add(value.Name, value.Value);
            }
            return result;
        }
    }
}
