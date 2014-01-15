using System;
using System.Linq;
using N2.Engine;
using N2.Persistence;
using N2.Persistence.Finder;
using N2.Web;
using N2.Definitions.Static;

namespace N2.Edit
{
    /// <summary>
    /// Gets and creates a container for management items.
    /// </summary>
    [Service]
    public class ContainerRepository<T> where T: ContentItem
    {
        IRepository<ContentItem> repository;
        IHost host;
        ContentActivator activator;
        private DefinitionMap map;

        /// <summary>Instructs this class to navigate the content hierarchy rather than query for items.</summary>
        public bool Navigate { get; set; }

        /// <summary>Stores dependencies</summary>
        /// <param name="finder"></param>
        /// <param name="repository"></param>
        /// <param name="activator"></param>
        public ContainerRepository(IRepository<ContentItem> repository, IHost host, ContentActivator activator, DefinitionMap map)
        {
            this.repository = repository;
            this.host = host;
            this.activator = activator;
            this.map = map;
        }

        /// <summary>Gets a container below the start page or null if no container exists.</summary>
        /// <returns></returns>
        public virtual T GetBelowStart()
        {
            return Get(repository.Get(host.CurrentSite.StartPageID));
        }

        /// <summary>Gets a container below the root page or null if no container exists.</summary>
        /// <returns></returns>
        public virtual T GetBelowRoot()
        {
            return Get(repository.Get(host.CurrentSite.RootItemID));
        }

        /// <summary>Gets a container below the start page and creates it if no container exists.</summary>
        /// <param name="setupCreatedItem"></param>
        /// <returns></returns>
        public virtual T GetOrCreateBelowStart(Action<T> setupCreatedItem)
        {
            return GetOrCreate(repository.Get(host.CurrentSite.StartPageID), setupCreatedItem);
        }

        /// <summary>Gets a container below the root page and creates it if no container exists.</summary>
        /// <param name="setupCreatedItem"></param>
        /// <returns></returns>
        public virtual T GetOrCreateBelowRoot(Action<T> setupCreatedItem)
        {
            return GetOrCreate(repository.Get(host.CurrentSite.RootItemID), setupCreatedItem);
        }

        /// <summary>Gets a container or null if no container exists.</summary>
        /// <param name="containerContainer"></param>
        /// <returns></returns>
        public virtual T Get(ContentItem containerContainer, string name = null) 
        {
            if (Navigate)
            {
                var q = containerContainer.Children.Query().OfType<T>();
                if (!string.IsNullOrEmpty(name))
                    q = q.Where(i => string.Equals(i.Name, name, StringComparison.InvariantCultureIgnoreCase));
                return q.FirstOrDefault();
            }
            else
            {
                var parameters = Parameter.Equal("Parent", containerContainer) & Parameter.TypeEqual(map.GetOrCreateDefinition(typeof(T)).Discriminator);
                if (!string.IsNullOrEmpty(name))
                    parameters.Add(Parameter.Like("Name", name));

                var items = repository.Find(parameters.Take(1));
                return items.OfType<T>().FirstOrDefault();
            }
        }

        /// <summary>Gets or creates a container.</summary>
        /// <param name="containerContainer"></param>
        /// <param name="setupCreatedItem"></param>
        /// <returns></returns>
        public virtual T GetOrCreate(ContentItem containerContainer, Action<T> setupCreatedItem, string name = null)
        {
            return Get(containerContainer, name) ?? Create(containerContainer, setupCreatedItem, name);
        }

        /// <summary>Creates a container.</summary>
        /// <param name="containerContainer"></param>
        /// <param name="setupCreatedItem"></param>
        /// <returns></returns>
        protected virtual T Create(ContentItem containerContainer, Action<T> setupCreatedItem, string name = null)
        {
            var container = activator.CreateInstance<T>(containerContainer);
            container.Name = name;
            container.AddTo(containerContainer);
            setupCreatedItem(container);
            
            repository.SaveOrUpdate(container);
            return container;
        }
    }
}
