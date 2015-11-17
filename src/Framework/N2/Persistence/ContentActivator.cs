using System;
using System.Collections.Generic;
using N2.Edit.Workflow;
using N2.Engine;
using N2.Persistence.Proxying;
using N2.Definitions;
using N2.Definitions.Static;

namespace N2.Persistence
{
    [Service]
    public class ContentActivator
    {
        private readonly IItemNotifier notifier;
        private readonly StateChanger stateChanger;
        private readonly IProxyFactory interceptor;
        private readonly IDictionary<Type, ItemDefinition> contentBuilders;

        public ContentActivator(StateChanger changer, IItemNotifier notifier, IProxyFactory interceptor)
        {
            this.stateChanger = changer;
            this.notifier = notifier;
            this.interceptor = interceptor;
            this.contentBuilders = new Dictionary<Type, ItemDefinition>();
        }

        /// <summary>Creates an instance of a certain type of item. It's good practice to create new items through this method so the item's dependencies can be injected by the engine.</summary>
        /// <returns>A new instance of an item.</returns>
        public T CreateInstance<T>(ContentItem parentItem, string templateKey = null, bool asProxy = false) where T : ContentItem
        {
            return (T)CreateInstance(typeof(T), parentItem, templateKey, asProxy);
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
        public virtual ContentItem CreateInstance(Type itemType, ContentItem parentItem, string templateKey, bool asProxy = false, bool invokeBehaviors = true)
        {
            if (itemType == null) throw new ArgumentNullException("itemType");

            ContentItem item = null;
            ItemDefinition definition;
            if (asProxy)
                item = (ContentItem)interceptor.Create(itemType.FullName, 0);

            if (item == null)
            {
                if (contentBuilders.TryGetValue(itemType, out definition))
				{
					item = definition.CreateInstance(parentItem, applyDefaultValues: !asProxy);
				}
                else
				{
					item = Activator.CreateInstance(itemType, true) as ContentItem;
					item.AddTo(parentItem);
				}
			}
            if (templateKey != null)
                item.TemplateKey = templateKey;
            if (invokeBehaviors)
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
            foreach (var definition in contentTypes)
                contentBuilders[definition.ItemType] = definition;

            interceptor.Initialize(contentTypes);
        }

        /// <summary>Notifies subscriber that an item was created through a <see cref="CreateInstance"/> method.</summary>
        public event EventHandler<ItemEventArgs> ItemCreated;
    }
}
