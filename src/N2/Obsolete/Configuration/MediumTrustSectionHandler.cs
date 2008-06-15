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
	}
}