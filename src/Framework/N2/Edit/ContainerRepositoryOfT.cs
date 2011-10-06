using System;
using System.Linq;
using N2.Engine;
using N2.Persistence;
using N2.Persistence.Finder;
using N2.Web;

namespace N2.Edit
{
	/// <summary>
	/// Gets and creates a container for management items.
	/// </summary>
	[Service]
	public class ContainerRepository<T> where T: ContentItem
	{
		IItemFinder finder;
		IPersister persister;
		IHost host;
		ContentActivator activator;

		/// <summary>Instructs this class to navigate the content hierarchy rather than query for items.</summary>
		public bool Navigate { get; set; }

		/// <summary>Stores dependencies</summary>
		/// <param name="finder"></param>
		/// <param name="persister"></param>
		/// <param name="activator"></param>
		public ContainerRepository(IPersister persister, IItemFinder finder, IHost host, ContentActivator activator)
		{
			this.finder = finder;
			this.persister = persister;
			this.host = host;
			this.activator = activator;
		}

		/// <summary>Gets a container below the start page or null if no container exists.</summary>
		/// <returns></returns>
		public virtual T GetBelowStart()
		{
			return Get(persister.Get(host.CurrentSite.StartPageID));
		}

		/// <summary>Gets a container below the root page or null if no container exists.</summary>
		/// <returns></returns>
		public virtual T GetBelowRoot()
		{
			return Get(persister.Get(host.CurrentSite.RootItemID));
		}

		/// <summary>Gets a container below the start page and creates it if no container exists.</summary>
		/// <param name="setupCreatedItem"></param>
		/// <returns></returns>
		public virtual T GetOrCreateBelowStart(Action<T> setupCreatedItem)
		{
			return GetOrCreate(persister.Get(host.CurrentSite.StartPageID), setupCreatedItem);
		}

		/// <summary>Gets a container below the root page and creates it if no container exists.</summary>
		/// <param name="setupCreatedItem"></param>
		/// <returns></returns>
		public virtual T GetOrCreateBelowRoot(Action<T> setupCreatedItem)
		{
			return GetOrCreate(persister.Get(host.CurrentSite.RootItemID), setupCreatedItem);
		}

		/// <summary>Gets a container or null if no container exists.</summary>
		/// <param name="containerContainer"></param>
		/// <returns></returns>
		public virtual T Get(ContentItem containerContainer, string name = null) 
		{
			if (Navigate)
			{
				var q = containerContainer.Children.Query().OfType<T>();
				return q.FirstOrDefault();
			}
			else
			{
				var q = finder.Where.Parent.Eq(containerContainer)
					.And.Type.Eq(typeof(T));
				if (!string.IsNullOrEmpty(name))
					q = q.And.Name.Eq(name);
				var items = q.MaxResults(1).Select<T>();
				return items.Count > 0 ? items[0] : null;
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
			setupCreatedItem(container);
			persister.Save(container);
			return container;
		}
	}
}
