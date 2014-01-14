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
            mm.Class<FileSystemChunk>(FileSystemChunkCustomization);

            var compiledMapping = mm.CompileMappingForAllExplicitlyAddedEntities();
            cfg.AddDeserializedMapping(compiledMapping, "N2");
        }

        void FileSystemItemCustomization(IClassMapper<FileSystemItem> ca)
        {
            ca.Table(tablePrefix + "FSItem");
            ca.Id(x => x.ID, cm => { cm.Generator(Generators.Native); });
            ca.Lazy(false);
            ca.Cache(cm => { cm.Usage(CacheUsage.NonstrictReadWrite); });

            ca.Component(x => x.Path, cm =>
            {
                cm.Property(x => x.Parent, ccm => { ccm.Length(1024); });
                cm.Property(x => x.Name, ccm => { ccm.Length(255); });
                cm.Property(x => x.IsDirectory, ccm => { });
            });
            ca.Property(x => x.Created, cm => { cm.NotNullable(true); });
            ca.Property(x => x.Updated, cm => { cm.NotNullable(true); });
            ca.Property(x => x.Length, cm => { });
            //ca.Bag(x => x.Chunks, cm =>
            //    {
            //        cm.Key(k => k.Column("FileID"));
            //        cm.Inverse(true);
            //        cm.Cascade(Cascade.All);
            //        cm.OrderBy(ci => ci.Offset);
            //        cm.Lazy(CollectionLazy.Extra);
            //    }, cr => cr.OneToMany());
        }

        void FileSystemChunkCustomization(IClassMapper<FileSystemChunk> ca)
        {
            ca.Table(tablePrefix + "FSChunk");
            ca.Lazy(false);
            ca.Id(x => x.ID, cm => { cm.Generator(Generators.Native); });
            ca.ManyToOne(x => x.BelongsTo, m => { m.Column("FileID"); });
            ca.Property(x => x.Offset, cm => { });
            ca.Property(cm => cm.Data, cm => { cm.Type(NHibernateUtil.BinaryBlob); cm.Length(ConfigurationBuilder.BlobLength); cm.Lazy(false); });
        }

    }
}
