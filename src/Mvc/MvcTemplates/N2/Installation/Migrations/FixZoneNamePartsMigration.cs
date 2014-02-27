using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Edit.Installation;
using N2.Persistence;
using N2.Collections;
using N2.Definitions;

namespace N2.Management.Installation
{
    [N2.Engine.Service(typeof(AbstractMigration))]
    public class FixZoneNamePartsMigration : AbstractMigration
    {
        private IContentItemRepository repository;
        private IDefinitionManager definitions;

        public FixZoneNamePartsMigration(IContentItemRepository repository, IDefinitionManager definitions)
        {
            this.repository = repository;
            this.definitions = definitions;

            Title = "Fixes zone name on parts with null zone name";
            Description = "In the database parts are defined by having a non-null ZoneName property. This migrations updates parts which have a null zone name and changes it to empty string.";
        }

        public override bool IsApplicable(DatabaseStatus status)
        {
            return status.DatabaseVersion < DatabaseStatus.RequiredDatabaseVersion || !status.HasSchema;
        }

        public override MigrationResult Migrate(DatabaseStatus preSchemaUpdateStatus)
        {
            int updatedItems = 0;
            using (var transaction = repository.BeginTransaction())
            {
                var nonNullZoneNameParts = repository.Find(Parameter.IsNull("ZoneName") & Parameter.TypeIn(definitions.GetDefinitions().Where(d => !d.IsPage).Select(d => d.Discriminator).ToArray()));
                foreach (var item in nonNullZoneNameParts)
                {
                    item.ZoneName = "";
                    repository.SaveOrUpdate(item);
                    updatedItems++;
                }

                transaction.Commit();
            }

            return new MigrationResult(this) { UpdatedItems = updatedItems };
        }
    }
}
