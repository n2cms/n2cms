using System.Configuration;
using System.Web.Configuration;

namespace N2.MediumTrust.Configuration
{
	public class MediumTrustSectionHandler : ConfigurationSection
	{
		[ConfigurationProperty("assemblies")]
		public AssemblyCollection Assemblies
		{
			get
			{
				return (AssemblyCollection) base["assemblies"];
			}
		}

		[ConfigurationProperty("itemTypes")]
		public TypeCollection ItemTypes
		{
			get
			{
				return (TypeCollection)base["itemTypes"];
			}
		}

		[ConfigurationProperty("nhProperties")]
		public KeyValueConfigurationCollection NHProperties
		{
			get
			{
				return (KeyValueConfigurationCollection)base["nhProperties"];
			}
		}

		[ConfigurationProperty("rootItemID", DefaultValue = 1)]
		public int RootItemID
		{
			get { return (int)base["rootItemID"]; ; }
			set { base["rootItemID"] = value; }
		}

		[ConfigurationProperty("startPageID", DefaultValue = 1)]
		public int StartPageID
		{
			get { return (int)base["startPageID"]; ; }
			set { base["startPageID"] = value; }
		}
	}
}