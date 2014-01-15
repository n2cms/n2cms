using System;
using System.Collections.Generic;
using System.Linq;
using N2.Definitions;
using N2.Engine;
using N2.Plugin;

namespace N2.Persistence
{
    /// <summary>
    /// Injects dependencies onto content items as they are created.
    /// </summary>
    [Service(typeof(IDependencyInjector))]
    public class ContentDependencyInjector : IAutoStart, IDependencyInjector
    {
        readonly IDefinitionManager definitions;
        readonly IServiceContainer services;
        readonly IItemNotifier notifier;
        readonly Dictionary<Type, IDependencySetter[]> injectorMap = new Dictionary<Type, IDependencySetter[]>();

        public ContentDependencyInjector(IServiceContainer services, IDefinitionManager definitions, IItemNotifier notifier)
        {
            this.services = services;
            this.definitions = definitions;
            this.notifier = notifier;
        }

        public virtual bool FulfilDependencies(ContentItem item)
        {
            bool dependenciesInjted = false;
            foreach (var provider in GetSetters(item.GetContentType()))
            {
                provider.Fulfil(item);
                dependenciesInjted = true;
            }
            return dependenciesInjted;
        }

        private void InitializeDependencyProviders()
        {
            foreach (var definition in definitions.GetDefinitions())
            {
                foreach (var interfaceType in definition.ItemType.GetInterfaces())
                {
                    if (interfaceType.IsGenericType && typeof(IInjectable<>) == interfaceType.GetGenericTypeDefinition())
                    {
                        Type genericType = interfaceType.GetGenericArguments()[0];
                        Type providerType = typeof(EntityDependencySetter<>).MakeGenericType(genericType);
                        var injector = services.Resolve(providerType) as IDependencySetter;
                        AddSetters(definition.ItemType, injector);
                    }
                }
            }
        }

        private IDependencySetter[] GetSetters(Type type)
        {
            IDependencySetter[] providers;
            if (!injectorMap.TryGetValue(type, out providers))
                providers = new IDependencySetter[0];
            return providers;
        }

        private void AddSetters(Type type, params IDependencySetter[] addedProviders)
        {
            IDependencySetter[] providers = GetSetters(type);
            injectorMap[type] = providers.Union(addedProviders).ToArray();
        }

        void notifier_ItemCreated(object sender, NotifiableItemEventArgs e)
        {
            e.WasModified = FulfilDependencies(e.AffectedItem);
        }

        #region IAutoStart Members

        public void Start()
        {
            InitializeDependencyProviders();

            notifier.ItemCreated += notifier_ItemCreated;
        }

        public void Stop()
        {
            notifier.ItemCreated -= notifier_ItemCreated;
        }

        #endregion
    }
}
