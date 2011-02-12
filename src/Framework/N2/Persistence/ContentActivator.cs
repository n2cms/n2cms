using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Edit.Workflow;
using N2.Persistence.Proxying;
using N2.Engine;

namespace N2.Persistence
{
	[Service]
	public class ContentActivator
	{
		private readonly IItemNotifier notifier;
		private readonly StateChanger stateChanger;
		private readonly IProxyFactory interceptor;

		public ContentActivator(StateChanger changer, IItemNotifier notifier, IProxyFactory interceptor)
		{
			this.stateChanger = changer;
			this.notifier = notifier;
			this.interceptor = interceptor;
		}

		/// <summary>Creates an instance of a certain type of item. It's good practice to create new items through this method so the item's dependencies can be injected by the engine.</summary>
		/// <returns>A new instance of an item.</returns>
		public T CreateInstance<T>(ContentItem parentItem) where T : ContentItem
		{
			return (T)CreateInstance(typeof(T), parentItem);
		}

		/// <summary>Creates an instance of a certain type of item. It's good practice to create new items through this method so the item's dependencies can be injected by the engine.</summary>
		/// <returns>A new instance of an item.</returns>
		public virtual ContentItem CreateInstance(Type itemType, ContentItem parentItem)
		{
			object intercepted = interceptor.Create(itemType.FullName, 0);
			ContentItem item = (intercepted ?? Activator.CreateInstance(itemType, true))
				as ContentItem;
			stateChanger.ChangeTo(item, ContentState.New);
			OnItemCreating(item, parentItem);
			return item;
		}

		protected virtual void OnItemCreating(ContentItem item, ContentItem parentItem)
		{
			item.Parent = parentItem;
			notifier.NotifiyCreated(item);
			if (ItemCreated != null)
				ItemCreated.Invoke(this, new ItemEventArgs(item));
		}

		public virtual void Initialize(IEnumerable<Type> contentTypes)
		{
			interceptor.Initialize(contentTypes);
		}

		/// <summary>Notifies subscriber that an item was created through a <see cref="CreateInstance"/> method.</summary>
		public event EventHandler<ItemEventArgs> ItemCreated;
	}
}
