using N2.Definitions;
using N2.Persistence.Finder;
using N2.Plugin.Scheduling;

namespace N2.Management.Content
{
	[ScheduleExecution(1, TimeUnit.Hours)]
	public class NumberOfItemsScheduledAction : ScheduledAction
	{
		IDefinitionManager definitions;
		IItemFinder finder;

		public override void Execute()
		{
			this.definitions = Engine.Resolve<IDefinitionManager>();
			this.finder = Engine.Resolve<IItemFinder>();

			foreach (var definition in definitions.GetDefinitions())
			{
				definition.NumberOfItems = finder.Where.Type.Eq(definition.ItemType).Count();
			}
		}
	}
}
