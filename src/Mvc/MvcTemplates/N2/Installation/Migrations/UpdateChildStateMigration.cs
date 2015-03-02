using System;
using N2.Edit.Installation;
using N2.Persistence;
using N2.Collections;

namespace N2.Management.Installation
{
    [N2.Engine.Service(typeof(AbstractMigration))]
    public class UpdateChildStateMigration : AbstractMigration
    {
        private readonly IContentItemRepository repository;

        public UpdateChildStateMigration(IContentItemRepository repository)
        {
            this.repository = repository;
            Title = "Upgrades child state flag to v2.3 model";
            Description = "Child state is used to avoid querying for the presence of children";
        }

        public override bool IsApplicable(DatabaseStatus status)
        {
	        return status.DatabaseVersion < DatabaseStatus.RequiredDatabaseVersion
	               || !status.HasSchema
	               || repository.Count(new Parameter("ChildState", Collections.CollectionState.Unknown)) > 0;
        }

        public override MigrationResult Migrate(DatabaseStatus preSchemaUpdateStatus)
        {
            int updatedItems = 0;
            using (var transaction = repository.BeginTransaction())
            {
                foreach (var item in repository.Find("ChildState", Collections.CollectionState.Unknown))
                {
                    item.ChildState = item.Children.CalculateState();
                    repository.SaveOrUpdate(item);
                    updatedItems++;
                }

                transaction.Commit();
            }

            return new MigrationResult(this) { UpdatedItems = updatedItems };
        }
    }
}
