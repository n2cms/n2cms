using System;
using System.Collections.Generic;
using N2.Edit.Workflow;
using N2.Engine;
using N2.Persistence.Proxying;
using N2.Definitions;

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
		/// <param name="itemType">Type of item to create</param>
		/// <param name="parentItem">Parent of the item to create.</param>
		/// <returns>A new instance of an item.</returns>
		public virtual ContentItem CreateInstance(Type itemType, ContentItem parentItem)
		{
			return CreateInstance(itemType, parentItem, null);
		}

        /// <summary>Creates an instance of a certain type of item. It's good practice to create new items through this method so the item's dependencies can be injected by the engine.</summary>
        /// <param name="itemType">Type of item to create</param>
        /// <param name="parentItem">Parent of the item to create.</param>
        /// <param name="templateKey">The type of template the item is associated with.</param>
        /// <returns>A new instance of an item.</returns>
        public virtual ContentItem CreateInstance(Type itemType, ContentItem parentItem, string templateKey)
        {
			if (itemType == null) throw new ArgumentNullException("itemType");

			object intercepted = null;// interceptor.Create(itemType.FullName, 0);
            ContentItem item = (intercepted ?? Activator.CreateInstance(itemType, true))
                as ContentItem;
            if (templateKey != null)
                item.TemplateKey = templateKey;
            OnItemCreating(item, parentItem);
            return item;
        }

        public virtual void NotifyCreated(ContentItem item)
        {
            notifier.NotifiyCreated(item);
            if (ItemCreated != null)
                ItemCreated.Invoke(this, new ItemEventArgs(item));
        }

		protected virtual void OnItemCreating(ContentItem item, ContentItem parentItem)
		{
			stateChanger.ChangeTo(item, ContentState.New);
            item.Parent = parentItem;
            NotifyCreated(item);
		}

		public virtual void Initialize(IEnumerable<ItemDefinition> contentTypes)
		{
			interceptor.Initialize(contentTypes);
		}

		/// <summary>Notifies subscriber that an item was created through a <see cref="CreateInstance"/> method.</summary>
		public event EventHandler<ItemEventArgs> ItemCreated;
	}
}
