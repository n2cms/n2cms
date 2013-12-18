using System.Linq;
using N2.Definitions;
using N2.Persistence.Finder;
using N2.Plugin.Scheduling;
using N2.Persistence;
using N2.Web;

namespace N2.Management.Content
{
    [ScheduleExecution(1, TimeUnit.Hours)]
    public class NumberOfItemsScheduledAction : ScheduledAction
    {
        private IHost host;
        private IDefinitionManager definitions;
        private IContentItemRepository repository;

        public NumberOfItemsScheduledAction(IHost host, IDefinitionManager definitions, IContentItemRepository repository)
        {
            this.host = host;
            this.definitions = definitions;
            this.repository = repository;
        }
        
        public override void Execute()
        {
            var root = repository.Get(host.CurrentSite.RootItemID);
            var discriminators = repository.FindDescendantDiscriminators(root).ToDictionary(d => d.Discriminator, d => d.Count);

            foreach (var definition in definitions.GetDefinitions())
            {
                int numberOfItems;
                discriminators.TryGetValue(definition.Discriminator, out numberOfItems);
                definition.NumberOfItems = numberOfItems;
            }
        }
    }
}
