using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Plugin.Scheduling;
using N2.Engine;
using N2.Definitions;
using N2.Persistence;
using N2.Persistence.Finder;

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
