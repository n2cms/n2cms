using N2.Definitions;
using N2.Details;
using N2.Edit.Installation;
using N2.Persistence;
using System.Linq;

namespace N2.Management.Installation
{
    [N2.Engine.Service(typeof(AbstractMigration))]
    public class EditableItemMigration : AbstractMigration
    {
        private IDefinitionManager definitions;
        IRepository<ContentItem> itemRepository;

        public EditableItemMigration(IDefinitionManager definitions, IRepository<ContentItem> itemRepository)
        {
            this.definitions = definitions;
            this.itemRepository = itemRepository;

            Title = "Upgrade parts and pages using [EditableItem]";
            Description = "This migration will change the name of sub-items and remove the reference from the containing page.";
        }

        public override bool IsApplicable(DatabaseStatus status)
        {
            return status.NeedsUpgrade || definitions.GetDefinitions().Any(d => d.Properties.Any(p => p.Value.Editable is EditableItemAttribute));
        }

        public override MigrationResult Migrate(DatabaseStatus preSchemaUpdateStatus)
        {
            if (IsApplicable(preSchemaUpdateStatus))
            {
                int updatedItems = 0;
                foreach (var d in definitions.GetDefinitions())
                {
                    foreach (var p in d.Properties.Values.Where(p => p.Editable is EditableItemAttribute))
                    {
                        using (var tx = itemRepository.BeginTransaction())
                        {
                            var containers = itemRepository.Find(Parameter.IsNotNull(p.Name).Detail() & Parameter.Equal("class", d.Discriminator)).ToList();
                            foreach (var container in containers)
                            {
                                var detail = container.Details[p.Name];
                                if (detail == null)
                                    continue;
                                if (!detail.LinkedItem.HasValue)
                                    continue;
                                if (detail.LinkedItem.Parent != container)
                                    continue;

                                detail.LinkedItem.Value.Name = p.Name;
                                itemRepository.SaveOrUpdate(detail.LinkedItem);

                                container.Details.Remove(detail);
                                itemRepository.SaveOrUpdate(container);

                                updatedItems++;
                            }
                            tx.Commit();
                        }
                    }
                }

                return new MigrationResult(this) { UpdatedItems = updatedItems };
            }
            else
                return new MigrationResult(this);
        }
    }
}
