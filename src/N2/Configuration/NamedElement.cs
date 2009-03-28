using System.Configuration;

namespace N2.Configuration
{
	public class NamedElement : ConfigurationElement
	{
		[ConfigurationProperty("name", IsRequired = true, IsKey = true)]
		public string Name
		{
			get { return (string)base["name"]; }
			set { base["name"] = value; }
		}
	}
}