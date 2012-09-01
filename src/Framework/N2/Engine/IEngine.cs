using System;
using N2.Definitions;
using N2.Edit;
using N2.Integrity;
using N2.Persistence;
using N2.Security;
using N2.Web;

namespace N2.Engine
{
	/// <summary>
	/// Classes implementing this interface can serve as a portal for the 
	/// various services composing the N2 engine. Edit functionality, modules
	/// and implementations access most N2 functionality through this 
	/// interface.
	/// </summary>
	public interface IEngine
	{
		/// <summary>Gets the persistence manager responsible of storing content items to persistence medium (database).</summary>
		IPersister Persister { get; }

		/// <summary>Gets the url parser responsible of mapping managementUrls to items and back again.</summary>
		IUrlParser UrlParser { get; }

		/// <summary>Gets the definition manager responsible of maintaining available item definitions.</summary>
		IDefinitionManager Definitions { get; }

		/// <summary>Gets the integrity manager used to control which items are allowed below which.</summary>
		IIntegrityManager IntegrityManager { get; }

		/// <summary>Gets the security manager responsible of controlling access to items.</summary>
		ISecurityManager SecurityManager { get; }

		/// <summary>Gets the class responsible for plugins in edit mode.</summary>
		IEditManager EditManager { get; }

		IEditUrlManager ManagementPaths { get; }

		/// <summary>Contextual data associated with the current request.</summary>
		IWebContext RequestContext { get; }

		/// <summary>The base of the web site.</summary>
		IHost Host { get; }

		/// <summary>The inversion of control container supporting this application.</summary>
		IServiceContainer Container { get; }

		/// <summary>
		/// Initialize components and plugins in the N2 CMS environment.
		/// </summary>
		void Initialize();

		/// <summary>Resolves a service configured for the factory.</summary>
		/// <typeparam name="T">The type of service to resolve.</typeparam>
		/// <returns>An instance of the resolved service.</returns>
		T Resolve<T>() where T : class;

		/// <summary>Resolves a service configured for the factory.</summary>
		/// <param name="serviceType">The type of service to resolve.</param>
		/// <returns>An instance of the resolved service.</returns>
		object Resolve(Type serviceType);

		/// <summary>Registers a component in the IoC container.</summary>
		/// <param name="key">The name of the component.</param>
		/// <param name="classType">The type of component.</param>
		[Obsolete("Use Container.AddComponent")]
		void AddComponent(string key, Type classType);

		/// <summary>Registers a component in the IoC container.</summary>
		/// <param name="key">The name of the component.</param>
		/// <param name="serviceType">The service interface of the component.</param>
		/// <param name="classType">The type of component.</param>
		[Obsolete("Use Container.AddComponent")]
		void AddComponent(string key, Type serviceType, Type classType);

		/// <summary>Adds a compnent instance to the container.</summary>
		/// <param name="key">A unique key.</param>
		/// <param name="serviceType">The type of service to provide.</param>
		/// <param name="instance">The service instance to add.</param>
		[Obsolete("Use Container.AddComponentInstance")]
		void AddComponentInstance(string key, Type serviceType, object instance);

		/// <summary>Adds a compnent instance to the container.</summary>
		/// <param name="key">A unique key.</param>
		/// <param name="classType">The type of component.</param>
		/// <param name="lifeStyle">The lifestyle that the component will be instantiated with.</param>
		[Obsolete("Use Container.AddComponentLifeStyle")]
		void AddComponentLifeStyle(string key, Type classType, ComponentLifeStyle lifeStyle);

		/// <summary>Releases a component from the IoC container.</summary>
		/// <param name="instance">The component instance to release.</param>
		[Obsolete("Use Container.Release")]
		void Release(object instance);

		ContentHelperBase Content { get; }
	}

	public enum ComponentLifeStyle
	{
		Singleton = 0,
		Transient = 1,
	}
}