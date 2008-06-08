using System.Configuration;
using System;

namespace N2.MediumTrust.Configuration
{
	[Obsolete]
	public class TypeElement : ConfigurationElement
	{
		[ConfigurationProperty("typeName", IsKey = true)]
		public string TypeName
		{
			get { return (string)base["typeName"]; }
		}
	}
}