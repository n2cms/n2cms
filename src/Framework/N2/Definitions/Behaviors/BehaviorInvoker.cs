using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Plugin;
using N2.Persistence;
using N2.Engine;
using N2.Definitions.Static;
using N2.Definitions;

namespace N2.Definitions.Behaviors
{
	[Service]
	public class BehaviorInvoker : IAutoStart
	{
		private IPersister persister;
		private DefinitionMap definitionMap;

		public BehaviorInvoker(IPersister persister, DefinitionMap definitionMap)
		{
			this.persister = persister;
			this.definitionMap = definitionMap;
		}

		void OnItemSaving(object sender, CancellableItemEventArgs e)
		{
			var item = e.AffectedItem;
			var ctx = new BehaviorContext { AffectedItem = item };

			if (item.Parent is ISavingBehavior)
				(item.Parent as ISavingBehavior).OnSavingChild(ctx);
			foreach (var behavior in GetBehviors<ISavingBehavior>(item.Parent))
				behavior.OnSavingChild(ctx);

			if (item is ISavingBehavior)
				(item as ISavingBehavior).OnSaving(ctx);
			foreach (var behavior in GetBehviors<ISavingBehavior>(item))
				behavior.OnSaving(ctx);

			foreach (var unsavedItem in ctx.UnsavedItems)
				persister.Repository.SaveOrUpdate(unsavedItem);
		}

		private IEnumerable<T> GetBehviors<T>(ContentItem item)
		{
			if (item == null)
				return Enumerable.Empty<T>();

			return definitionMap.GetOrCreateDefinition(item).GetCustomAttributes<T>();
		}

		public void Start()
		{
			persister.ItemSaving += OnItemSaving;
		}

		public void Stop()
		{
			persister.ItemSaving += OnItemSaving;
		}
	}
}
