using System.Configuration;
using System.Web.Configuration;
using System;

namespace N2.MediumTrust.Configuration
{
	[Obsolete]
	public class MediumTrustSectionHandler : ConfigurationSection
	{
		public MediumTrustSectionHandler()
		{
			throw new ConfigurationErrorsException("The MediumTrustSectionHandler has been deprecated. To configure assemblies use n2/engine/assemblies.");
		}

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

		[ConfigurationProperty("multipleSites", DefaultValue = false)]
		public bool MultipleSites
		{
			get { return (bool)base["multipleSites"]; }
			set { base["multipleSites"] = value; }
		}
	}
}