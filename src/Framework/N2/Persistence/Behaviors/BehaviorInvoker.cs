
namespace N2.Persistence.Behaviors
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using N2.Plugin;
	using N2.Persistence;
	using N2.Engine;
	using N2.Definitions.Static;

	[Service]
	public class BehaviorInvoker : IAutoStart
	{
		private readonly IPersister persister;
		private readonly DefinitionMap definitionMap;
		private readonly IEventsManager eventsManager;
		private readonly IBehavior[] generalBehaviors;

		public BehaviorInvoker(IPersister persister, DefinitionMap definitionMap, IEventsManager eventsManager, params IBehavior[] generalBehaviors)
		{
			this.persister = persister;
			this.definitionMap = definitionMap;
			this.eventsManager = eventsManager;
			this.generalBehaviors = generalBehaviors;
		}

		void OnItemSaving(object sender, CancellableItemEventArgs e)
		{
			InvokeBehaviors<ISavingBehavior>(
				e.AffectedItem,
				e.AffectedItem.Parent,
				"Saving",
				(behavior, ctx) => behavior.OnSavingChild(ctx),
				(behavior, ctx) => behavior.OnSaving(ctx));

			if (e.AffectedItem.ID == 0)
			{
				InvokeBehaviors<IAddingBehavior>(
					e.AffectedItem,
					e.AffectedItem.Parent,
					"Adding",
					(behavior, ctx) => behavior.OnAddingChild(ctx),
					(behavior, ctx) => behavior.OnAdding(ctx));
			}
		}

		void OnItemDeleting(object sender, CancellableItemEventArgs e)
		{
			InvokeBehaviors<IRemovingBehavior>(
				e.AffectedItem,
				e.AffectedItem.Parent,
				"Removing",
				(behavior, ctx) => behavior.OnRemovingChild(ctx),
				(behavior, ctx) => behavior.OnRemoving(ctx));

			InvokeBehaviors<IDeletingBehavior>(
				e.AffectedItem,
				e.AffectedItem.Parent,
				"Deleting",
				(behavior, ctx) => behavior.OnDeletingChild(ctx),
				(behavior, ctx) => behavior.OnDeleting(ctx));
		}

		void OnItemCopying(object sender, CancellableDestinationEventArgs e)
		{
			InvokeBehaviors<IAddingBehavior>(
				e.AffectedItem,
				e.Destination,
				"Adding",
				(behavior, ctx) => behavior.OnAddingChild(ctx),
				(behavior, ctx) => behavior.OnAdding(ctx));
		}

		void OnItemMoving(object sender, CancellableDestinationEventArgs e)
		{
			InvokeBehaviors<IRemovingBehavior>(
				e.AffectedItem,
				e.AffectedItem.Parent,
				"Removing",
				(behavior, ctx) => behavior.OnRemovingChild(ctx),
				(behavior, ctx) => behavior.OnRemoving(ctx));

			InvokeBehaviors<IAddingBehavior>(
				e.AffectedItem,
				e.Destination,
				"Adding",
				(behavior, ctx) => behavior.OnAddingChild(ctx),
				(behavior, ctx) => behavior.OnAdding(ctx));
		}

		private IEnumerable<T> GetBehviors<T>(ContentItem item) where T : class
		{
			if (item == null)
				return Enumerable.Empty<T>();

			var behaviors = definitionMap.GetOrCreateDefinition(item).GetCustomAttributes<T>();
			if (generalBehaviors.Length > 0)
				behaviors = behaviors.Union(generalBehaviors.OfType<T>());
			if (item is T)
				behaviors = behaviors.Union(new[] { item as T });
			return behaviors;
		}

		private void InvokeBehaviors<T>(ContentItem affectedItem, ContentItem parent, string action, Action<T, BehaviorContext> childBehaviorIvoker, Action<T, BehaviorContext> itemBehaviorInvoker)
			where T : class
		{
			var ctx = new BehaviorContext { AffectedItem = affectedItem, Parent = parent, Action = action };

			foreach (var behavior in GetBehviors<T>(parent))
				childBehaviorIvoker(behavior, ctx);

			foreach (var behavior in GetBehviors<T>(affectedItem))
				itemBehaviorInvoker(behavior, ctx);

			using (var tx = persister.Repository.BeginTransaction())
			{
				foreach (var unsavedItem in ctx.UnsavedItems)
					persister.Sources.Save(unsavedItem);
				tx.Commit();
			}
		}

		public void Start()
		{
			this.eventsManager.ItemSaving += OnItemSaving;
			persister.ItemDeleting += OnItemDeleting;
			persister.ItemMoving += OnItemMoving;
			persister.ItemCopying += OnItemCopying;
		}

		public void Stop()
		{
			persister.ItemCopying += OnItemCopying;
			persister.ItemMoving += OnItemMoving;
			persister.ItemDeleting -= OnItemDeleting;
			this.eventsManager.ItemSaving += OnItemSaving;
		}
	}
}
