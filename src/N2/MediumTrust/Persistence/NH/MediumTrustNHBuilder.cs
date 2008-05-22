using System.Configuration;
using System.Web.Configuration;
using N2.Definitions;
using N2.MediumTrust.Configuration;
using N2.Persistence.NH;

namespace N2.MediumTrust.Persistence.NH
{
	public class MediumTrustNHBuilder : DefaultConfigurationBuilder
	{
		private readonly MediumTrustSectionHandler configSection;

		public MediumTrustNHBuilder(IDefinitionManager definitions)
			: base(definitions)
		{
			DefaultMapping = "N2.MediumTrust.Mappings.MediumTrust.hbm.xml,N2";

			configSection = (MediumTrustSectionHandler)WebConfigurationManager.GetSection("n2/mediumTrust");
			foreach(KeyValueConfigurationElement element in configSection.NHProperties)
			{
				Properties[element.Key] = element.Value;
			}

			NHibernate.Cfg.Environment.UseReflectionOptimizer = false;
		}
	}
}
