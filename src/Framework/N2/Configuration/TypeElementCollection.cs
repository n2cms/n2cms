using System.Configuration;
using System;

namespace N2.Configuration
{
    [ConfigurationCollection(typeof(TypeElement))]
    public class TypeElementCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new TypeElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((TypeElement) element).TypeName;
		}
	}
}