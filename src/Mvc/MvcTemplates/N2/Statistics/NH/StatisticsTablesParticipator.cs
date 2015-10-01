using N2.Configuration;
using N2.Engine;
using N2.Persistence.NH;
using NHibernate.Mapping.ByCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Statistics.NH
{
    [Service(typeof(ConfigurationBuilderParticipator))]
    public class StatisticsTablesParticipator : ConfigurationBuilderParticipator
	{
        string tablePrefix = "n2";

		public StatisticsTablesParticipator(DatabaseSection config)
        {
            tablePrefix = config.TablePrefix;
		}

		public override void AlterConfiguration(NHibernate.Cfg.Configuration cfg)
		{
            ModelMapper mm = new ModelMapper();
            mm.Class<Bucket>(BucketCustomization);
			

			var compiledMapping = mm.CompileMappingForAllExplicitlyAddedEntities();
			cfg.AddDeserializedMapping(compiledMapping, "N2");
		}

		private void BucketCustomization(IClassMapper<Bucket> ca)
		{
			ca.Table(tablePrefix + "statisticsBucket");
			ca.Id(x => x.ID, cm => { cm.Generator(Generators.Native); });
			ca.Lazy(false);
			ca.Property(x => x.PageID, cm => { cm.NotNullable(true); });
			ca.Property(x => x.TimeSlot, cm => { cm.NotNullable(true); });
			ca.Property(x => x.Count, cm => { cm.NotNullable(true); });
		}
	}
}