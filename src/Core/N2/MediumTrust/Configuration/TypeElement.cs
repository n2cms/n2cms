using System.Configuration;

namespace N2.MediumTrust.Configuration
{
	public class TypeElement : ConfigurationElement
	{
		[ConfigurationProperty("typeName", IsKey = true)]
		public string TypeName
		{
			get { return (string)base["typeName"]; }
		}
	}
}