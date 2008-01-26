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
			configSection = (MediumTrustSectionHandler)WebConfigurationManager.GetSection("n2/mediumTrust");
			foreach(KeyValueConfigurationElement element in configSection.NHProperties)
			{
				Properties[element.Key] = element.Value;
			}

			NHibernate.Cfg.Environment.UseReflectionOptimizer = false;
		}

		protected override void AddDefaultMappings(NHibernate.Cfg.Configuration cfg)
		{
			string[] mappings = new string[]
				{
					"N2.MediumTrust.Mappings.AuthorizedRole.hbm.xml",
					"N2.MediumTrust.Mappings.ContentDetail.hbm.xml",
					"N2.MediumTrust.Mappings.DetailCollection.hbm.xml",
					"N2.MediumTrust.Mappings.ContentItem.hbm.xml"
				};
			foreach(string mapping in mappings)
			{
				cfg.AddInputStream(GetType().Assembly.GetManifestResourceStream(mapping));
			}
		}
	}
}
