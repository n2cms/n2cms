#region License

/* Copyright (C) 2006-2007 Cristian Libardo
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 */

#endregion

using System;
using System.Configuration;
using Castle.Windsor;
using N2.Configuration;
using N2.Definitions;
using N2.Edit;
using N2.Integrity;
using N2.Persistence;
using N2.Plugin;
using N2.Security;
using N2.Web;

namespace N2.Engine
{
	/// <summary>
	/// This principal gateway to N2 services. The class is responsible for 
	/// initializing and providing access to the services that compose N2.
	/// </summary>
	public class ContentEngine : IEngine
	{
		private readonly IServiceContainer container;

		/// <summary>
		/// Creates an instance of the content engine using known configuration sections.
		/// </summary>
		public ContentEngine()
			: this(new WindsorServiceContainer())
		{
		}

		public ContentEngine(EventBroker broker)
			: this(new WindsorServiceContainer(), broker)
		{
		}

		/// <summary>Sets the windsor container to the given container.</summary>
		/// <param name="container">An previously prepared windsor container.</param>
		public ContentEngine(IWindsorContainer container)
			: this(new WindsorServiceContainer(container))
		{
		}

		/// <summary>Sets the current container to the given container.</summary>
		/// <param name="container">An previously prepared service container.</param>
		public ContentEngine(IServiceContainer container)
			:this(container, EventBroker.Instance)
		{
		}

		/// <summary>Sets the current container to the given container.</summary>
		/// <param name="container">An previously prepared service container.</param>
		/// <param name="broker"></param>
		public ContentEngine(IServiceContainer container, EventBroker broker)
		{
			this.container = container;

			AddComponentInstance(broker);

			Configure();
		}

		/// <summary>Tries to determine runtime parameters from the given configuration.</summary>
		/// <param name="config">The configuration to use.</param>
		/// <param name="sectionGroup">The configuration section to retrieve configuration from</param>
		/// <param name="broker">Web ap event provider</param>
		public ContentEngine(System.Configuration.Configuration config, string sectionGroup, EventBroker broker)
		{
			if (string.IsNullOrEmpty(sectionGroup))
				throw new ArgumentException("Must be non-empty and match a section group in the configuration file.", "sectionGroup");

			container = new WindsorServiceContainer();

			RegisterConfigurationSections(config, sectionGroup);
			HostSection hostConfig = AddComponentInstance(config.GetSection(sectionGroup + "/host") as HostSection);
			EngineSection engineConfig = AddComponentInstance(config.GetSection(sectionGroup + "/engine") as EngineSection);
			AddComponentInstance(config.GetSection(sectionGroup + "/database") as DatabaseSection);
			AddComponentInstance(config.GetSection(sectionGroup + "/edit") as EditSection);
			AddComponentInstance(config.GetSection("connectionStrings") as ConnectionStringsSection);
			InitializeEnvironment(hostConfig, engineConfig);

			container.Configure(this, engineConfig);
			AddComponentInstance(broker);
		}

		protected void Configure()
		{
			HostSection hostConfig = AddComponentInstance(ConfigurationManager.GetSection("n2/host") as HostSection);
			EngineSection engineConfig = AddComponentInstance(ConfigurationManager.GetSection("n2/engine") as EngineSection);
			AddComponentInstance(ConfigurationManager.GetSection("n2/database") as DatabaseSection);
			AddComponentInstance(ConfigurationManager.GetSection("n2/edit") as EditSection);
			AddComponentInstance(ConfigurationManager.GetSection("connectionStrings") as ConnectionStringsSection);

			InitializeEnvironment(hostConfig, engineConfig);
			container.Configure(this, engineConfig);
		}

		protected void InitializeEnvironment(HostSection hostConfig, EngineSection engineConfig)
		{
			if (hostConfig != null && engineConfig != null)
			{
				Url.DefaultExtension = hostConfig.Web.Extension;

				RegisterConfiguredComponents(engineConfig);

				if (!hostConfig.Web.IsWeb)
					container.AddComponentInstance("n2.webContext.notWeb", typeof (IWebContext), new ThreadContext());

				if (hostConfig.Web.Urls.EnableCaching)
					container.AddComponent("n2.web.cachingUrlParser", typeof (IUrlParser), typeof (CachingUrlParserDecorator));
				DetermineUrlParser(hostConfig);
			}
		}

		private void DetermineUrlParser(HostSection hostConfig)
		{
			if (hostConfig.MultipleSites)
				container.AddComponent("n2.multipleSitesParser", typeof(IUrlParser), typeof(MultipleSitesParser));
			else
				container.AddComponent("n2.urlParser", typeof(IUrlParser), typeof(UrlParser));
		}

		private void RegisterConfiguredComponents(EngineSection engineConfig)
		{
			foreach (ComponentElement component in engineConfig.Components)
			{
				Type implementation = Type.GetType(component.Implementation);
				Type service = Type.GetType(component.Service) ?? implementation;
				if (service != null)
					container.AddComponent(service.FullName, service, implementation);
			}
		}

		#region Properties

		public IServiceContainer Container
		{
			get { return container; }
		}

		/// <summary>Gets N2 persistence manager used for database persistence of content.</summary>
		public IPersister Persister
		{
			get { return Resolve<IPersister>(); }
		}

		/// <summary>Gets N2 definition manager</summary>
		public IDefinitionManager Definitions
		{
			get { return Resolve<IDefinitionManager>(); }
		}

		/// <summary>Gets N2 integrity manager </summary>
		public IIntegrityManager IntegrityManager
		{
			get { return Resolve<IIntegrityManager>(); }
		}

		/// <summary>Gets N2 security manager responsible of item access checks.</summary>
		public ISecurityManager SecurityManager
		{
			get { return Resolve<ISecurityManager>(); }
		}

		/// <summary>Gets the url parser responsible of mapping urls to items and back again.</summary>
		public IUrlParser UrlParser
		{
			get { return Resolve<IUrlParser>(); }
		}

		/// <summary>Gets the edit manager responsible for plugins in edit mode.</summary>
		public IEditManager EditManager
		{
			get { return Resolve<IEditManager>(); }
		}

		public IWebContext RequestContext
		{
			get { return Resolve<IWebContext>(); }
		}

		public IHost Host
		{
			get { return Resolve<IHost>(); }
		}

		#endregion

		#region Methods

		public void Initialize()
		{
			AddComponentInstance<IEngine>(this);
            AddComponentInstance<IServiceContainer>(this.Container);

			var invoker = Resolve<IPluginBootstrapper>();
			invoker.InitializePlugins(this, invoker.GetPluginDefinitions());

			container.StartComponents();
		}

		/// <summary>Registers configuration sections into the container. These may be used as input for various components.</summary>
		/// <param name="config">The congiuration file.</param>
		/// <param name="sectionGroup">The config section that contains the configuration.</param>
		protected void RegisterConfigurationSections(System.Configuration.Configuration config, string sectionGroup)
		{
			object nhConfiguration = config.GetSection("hibernate-configuration");
			if (nhConfiguration != null)
				container.AddComponentInstance("hibernate-configuration", nhConfiguration.GetType(), nhConfiguration);
			var n2Group = config.GetSectionGroup(sectionGroup) as SectionGroup;
			if (n2Group != null)
			{
				foreach (ConfigurationSection section in n2Group.Sections)
				{
					container.AddComponentInstance(section.SectionInformation.Name, section.GetType(), section);
				}
			}
		}

		#endregion

		#region Container Methods

		/// <summary>Resolves a service configured in the factory.</summary>
		public T Resolve<T>() where T : class
		{
			return (T) Container.Resolve(typeof (T));
		}

		public object Resolve(Type serviceType)
		{
			return Container.Resolve(serviceType);
		}

		/// <summary>Resolves a named service configured in the factory.</summary>
		/// <param name="key">The name of the service to resolve.</param>
		/// <returns>An instance of the resolved service.</returns>
		public object Resolve(string key)
		{
			return Container.Resolve(key);
		}

		/// <summary>Registers a component in the IoC container.</summary>
		/// <param name="key">A unique key.</param>
		/// <param name="serviceType">The type of component to register.</param>
		public virtual void AddComponent(string key, Type serviceType)
		{
			AddComponent(key, serviceType, serviceType);
		}

		/// <summary>Registers a component in the IoC container.</summary>
		/// <param name="key">A unique key.</param>
		/// <param name="serviceType">The type of service to provide.</param>
		/// <param name="classType">The type of component to register.</param>
		public virtual void AddComponent(string key, Type serviceType, Type classType)
		{
			Container.AddComponent(key, serviceType, classType);
		}

		/// <summary>Adds a compnent instance to the container.</summary>
		/// <param name="key">A unique key.</param>
		/// <param name="serviceType">The type of service to provide.</param>
		/// <param name="instance">The service instance to add.</param>
		public void AddComponentInstance(string key, Type serviceType, object instance)
		{
			Container.AddComponentInstance(key, serviceType, instance);
		}

		public void AddComponentLifeStyle(string key, Type classType, ComponentLifeStyle lifeStyle)
		{
			Container.AddComponentLifeStyle(key, classType, lifeStyle);
		}

		public void AddFacility(string key, object facility)
		{
			Container.AddFacility(key, facility);
		}

		public void Release(object instance)
		{
			Container.Release(instance);
		}

		public T AddComponentInstance<T>(T instance) where T : class
		{
			if (instance != null)
			{
				AddComponentInstance(typeof (T).Name, typeof (T), instance);
			}
			return instance;
		}

		#endregion
	}
}