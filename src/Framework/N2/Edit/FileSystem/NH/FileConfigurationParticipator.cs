using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Persistence.NH;
using N2.Engine;
using NHibernate.Mapping.ByCode;
using NHibernate;
using N2.Configuration;

namespace N2.Edit.FileSystem.NH
{
	/// <summary>
	/// Configures the database file system in the NHibernate configuration.
	/// </summary>
	[Service(typeof(ConfigurationBuilderParticipator), Configuration = "dbfs")]
	public class FileConfigurationParticipator : ConfigurationBuilderParticipator
	{
		string tablePrefix = "n2";
		int chunkSize = 1024 * 1024;


		public FileConfigurationParticipator(DatabaseSection config)
		{
			tablePrefix = config.TablePrefix;
			chunkSize = config.Files.ChunkSize;
		}
	

		public override void AlterConfiguration(NHibernate.Cfg.Configuration cfg)
		{
			ModelMapper mm = new ModelMapper();
			mm.Class<FileSystemItem>(FileSystemItemCustomization);

			var compiledMapping = mm.CompileMappingForAllExplicitlyAddedEntities();
			cfg.AddDeserializedMapping(compiledMapping, "N2");
		}

		void FileSystemItemCustomization(IClassMapper<FileSystemItem> ca)
		{
			ca.Table(tablePrefix + "File");
			ca.Id(x => x.ID, cm => { cm.Generator(Generators.Native); });

			ca.Component(x => x.Path, cm =>
			{
				cm.Property(x => x.Parent, ccm => { ccm.Length(1024); });
				cm.Property(x => x.Name, ccm => { ccm.Length(255); });
				cm.Property(x => x.IsDirectory, ccm => { });
			});
			ca.Property(x => x.Created, cm => { cm.NotNullable(true); });
			ca.Property(x => x.Updated, cm => { cm.NotNullable(true); });
			ca.Property(x => x.Offset, cm => { });
			ca.Property(x => x.Length, cm => { });
			ca.Property(cm => cm.Data, cm => { cm.Type(NHibernateUtil.BinaryBlob); cm.Length(2147483647); cm.Lazy(true); });
		}

	}
}
