using N2.Definitions;
using N2.Edit.Installation;
using N2.Persistence;
using N2.Security;
using System.Linq;

namespace N2.Management.Installation
{
    [N2.Engine.Service(typeof(AbstractMigration))]
    public class ConvertEnumsMigration : AbstractMigration
    {
        InstallationManager installer;
        IRepository<ContentItem> repository;
        private IDefinitionManager definitions;

        public ConvertEnumsMigration(IRepository<ContentItem> repository, InstallationManager installer, IDefinitionManager definitions)
        {
            this.repository = repository;
            this.installer = installer;
            this.definitions = definitions;

            Title = "Convert enum detail types to new storage format";
            Description = "Enums values are stored as string (+ integer) in this version of N2. To support querying, previously stored enums must be converted.";
        }

        public override bool IsApplicable(DatabaseStatus status)
        {
            return definitions.GetDefinitions().SelectMany(d => d.Properties.Values).Any(p => p.PropertyType.IsEnum);
        }

        public override MigrationResult Migrate(DatabaseStatus preSchemaUpdateStatus)
        {
            var enumProperties = definitions.GetDefinitions()
                .SelectMany(d => d.Properties.Values.Where(p => p.PropertyType.IsEnum)
                    .Select(p => new { Definition = d, Property = p }));

            using (var transaction = repository.BeginTransaction())
            {
                int updatedItems = 0;
                foreach (var enumProperty in enumProperties)
                {
                    var items = repository.Find(Parameter.TypeEqual(enumProperty.Definition.Discriminator) & Parameter.IsNotNull(enumProperty.Property.Name).Detail());
                    foreach (var item in items)
                    {
                        item[enumProperty.Property.Name] = item[enumProperty.Property.Name];
                        repository.SaveOrUpdate(item);
                    }
                }
                transaction.Commit();
                return new MigrationResult(this) { UpdatedItems = updatedItems };
            }
        }
    }
}
