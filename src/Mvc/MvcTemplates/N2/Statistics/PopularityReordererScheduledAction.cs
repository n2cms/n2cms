using N2.Definitions;
using N2.Persistence;
using N2.Plugin.Scheduling;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Statistics
{
	[ScheduleExecution(1, TimeUnit.Minutes)]
	public class PopularityReordererScheduledAction : ScheduledAction
	{
		private IDefinitionManager definitions;
		private IPersister persister;
		private IHost host;

		private DateTime? lastExecuted;
		private PopularityChildrenSorter sorter;
		
		public PopularityReordererScheduledAction(IDefinitionManager definitions, IPersister persister, IHost host, PopularityChildrenSorter sorter)
		{
			this.definitions = definitions;
			this.persister = persister;
			this.host = host;
			this.sorter = sorter;
		}

		public override bool ShouldExecute()
		{
			if (!base.ShouldExecute())
				return false;
			if (!DefinitionWithPopularityAttribute().Any())
				return false;

			lastExecuted = GetLastExecuted();

			return lastExecuted.Value.Date != Utility.CurrentTime().Date;
		}

		public override void Execute()
		{
			foreach (var attributedDefinition in DefinitionWithPopularityAttribute())
			{
				var attributedParents = persister.Repository.Find(Parameter.TypeEqual(attributedDefinition.Discriminator));
				foreach (var parent in attributedParents)
				{
					foreach (var updatedChild in sorter.ReorderChildren(parent))
					{
						persister.Repository.SaveOrUpdate(updatedChild);
					}
					persister.Repository.Flush();
				}
			}

			UpdateLastExecuted();
		}

		private IEnumerable<ItemDefinition> DefinitionWithPopularityAttribute()
		{
			return definitions.GetDefinitions().Where(d => d.GetCustomAttributes<SortChildrenAttribute>().Any(sc => sc.CustomSorterType == typeof(PopularityChildrenSorter)));
		}

		private DateTime? GetLastExecuted()
		{
			if (lastExecuted.HasValue)
				return lastExecuted;

			var root = persister.Get(host.DefaultSite.RootItemID);
			var value = root["ScheduledPopularityLastExecuted"] as DateTime?;
			if (value.HasValue)
				return value;

			return DateTime.MinValue;
		}

		private void UpdateLastExecuted()
		{
			lastExecuted = Utility.CurrentTime();
			var root = persister.Get(host.DefaultSite.RootItemID);
			root["ScheduledPopularityLastExecuted"] = lastExecuted;
			persister.Repository.SaveOrUpdate(root);
			persister.Repository.Flush();
		}
	}
}