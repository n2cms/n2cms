using System.Configuration;

namespace N2.MediumTrust.Configuration
{
	public class TypeCollection : ConfigurationElementCollection
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