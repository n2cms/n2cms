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
using System.Web;
using System.Diagnostics;
using System.Configuration;

using Castle.MicroKernel;
using Castle.Core.Resource;
using Castle.Windsor;
using Castle.Windsor.Configuration;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Windsor.Installer;
using Castle.Windsor.Configuration.AppDomain;

using N2.Definitions;
using N2.Edit;
using N2.Integrity;
using N2.Persistence;
using N2.Plugin;
using N2.Security;
using N2.Web;
using N2.Configuration;

namespace N2.Engine
{
	/// <summary>
	/// This principal gateway to N2 serivecs. The class is responsible for 
	/// initializing and providing access to the services that compose N2.
	/// </summary>
	public class ContentEngine : IEngine
	{
		private IWindsorContainer container;

        public ContentEngine()
        {
            container = new WindsorContainer();

            HostSection hostConfig = AddComponentInstance<HostSection>(ConfigurationManager.GetSection("n2/host"));
            EngineSection engineConfig = AddComponentInstance<EngineSection>(ConfigurationManager.GetSection("n2/engine"));
            DatabaseSection dbConfig = AddComponentInstance<DatabaseSection>(ConfigurationManager.GetSection("n2/database"));
            EditSection editConfig = AddComponentInstance<EditSection>(ConfigurationManager.GetSection("n2/edit"));

            InitializeEnvironment(hostConfig, engineConfig);
            IResource resource = DetermineResource(engineConfig, ConfigurationManager.GetSection("castle") != null);
            ProcessResource(resource);
            InstallComponents();
        }

        protected T AddComponentInstance<T>(object instance) where T: class
        {
            AddComponentInstance(typeof(T).Name, typeof(T), instance);
            return instance as T;
        }

		/// <summary>Sets the windsdor container to the given container.</summary>
		/// <param name="container">An previously prepared windsor container.</param>
		public ContentEngine(IWindsorContainer container)
		{
			this.container = container;
		}

		/// <summary>Tries to determine runtime parameters from the given configuration.</summary>
		/// <param name="config">The configuration to use.</param>
		public ContentEngine(System.Configuration.Configuration config)
		{
			container = new WindsorContainer();
			
			RegisterConfigurationSections(config);
			HostSection hostConfig = (HostSection)config.GetSection("n2/host");
			EngineSection engineConfig = (EngineSection)config.GetSection("n2/engine");
            InitializeEnvironment(hostConfig, engineConfig);
			IResource resource = DetermineResource(engineConfig, config.GetSection("castle") != null);
			ProcessResource(resource);
            InstallComponents();
		}

        private void InitializeEnvironment(HostSection hostConfig, EngineSection engineConfig)
        {
            if (hostConfig != null && engineConfig != null)
            {
                Url.DefaultExtension = hostConfig.Web.Extension;
                if (!hostConfig.Web.IsWeb)
                    container.Kernel.AddComponentInstance("n2.webContext.notWeb", typeof(IWebContext), new ThreadContext());

                if (hostConfig.MultipleSites)
                {
                    ProcessResource(new AssemblyResource(engineConfig.MultipleSitesConfiguration));
                }
            }
        }

		#region Properties

		public IWindsorContainer Container
		{
			get { return container; }
			set { container = value; }
		}

		/// <summary>Gets N2 persistence manager used for database persistence of content.</summary>
		public IPersister Persister
		{
			get { return container.Resolve<IPersister>(); }
		}

		/// <summary>Gets N2 definition manager</summary>
		public IDefinitionManager Definitions
		{
			get { return container.Resolve<IDefinitionManager>(); }
		}

		/// <summary>Gets N2 integrity manager </summary>
		public IIntegrityManager IntegrityManager
		{
			get { return container.Resolve<IIntegrityManager>(); }
		}

		/// <summary>Gets N2 security manager responsible of item access checks.</summary>
		public ISecurityManager SecurityManager
		{
			get { return container.Resolve<ISecurityManager>(); }
		}

		/// <summary>Gets the url parser responsible of mapping urls to items and back again.</summary>
		public IUrlParser UrlParser
		{
			get { return container.Resolve<IUrlParser>(); }
		}

		/// <summary>Gets the url rewriter responsible for passing request to the correct page template.</summary>
		public IUrlRewriter Rewriter
		{
			get { return container.Resolve<IUrlRewriter>(); }
		}

		/// <summary>Gets the edit manager responsible for plugins in edit mode.</summary>
		public IEditManager EditManager
		{
			get { return container.Resolve<IEditManager>(); }
		}

        public IHost Host
        {
            get { return container.Resolve<IHost>(); }
        }
		#endregion

		#region Methods

		/// <summary>Either reads the castle configuration from the castle configuration section or uses a default configuration compiled into the n2 assembly.</summary>
		/// <param name="n2group">The n2 configuration group from the configuration file.</param>
		/// <returns>A castle IResource used to build the inversion of control container.</returns>
        protected IResource DetermineResource(EngineSection engineConfig, bool hasCastleSection)
		{
            if (engineConfig != null)
			{
				if (!string.IsNullOrEmpty(engineConfig.CastleSection))
                    return new ConfigResource(engineConfig.CastleSection);
                else
                    return new AssemblyResource(engineConfig.CastleConfiguration);
			}
            else if (hasCastleSection)
            {
                return new ConfigResource();
            }
            else 
			{
				throw new ConfigurationErrorsException("Couldn't find a suitable configuration section for N2 CMS. Either add an n2/engine or a castle configuartion section to web.config. Note that this section may have changed from previous versions. Please verify that the configuartion is properly updated.");
			}
		}


		/// <summary>Registers configuration sections into the container. These may be used as input for various components.</summary>
		/// <param name="n2group">The n2 congiuration group.</param>
		protected void RegisterConfigurationSections(System.Configuration.Configuration config)
		{
			object nhConfiguration = config.GetSection("hibernate-configuration");
			if (nhConfiguration != null)
				container.Kernel.AddComponentInstance("hibernate-configuration", nhConfiguration);
			SectionGroup n2group = config.GetSectionGroup("n2") as SectionGroup;
			if (n2group != null)
			{
				foreach (ConfigurationSection section in n2group.Sections)
				{
					container.Kernel.AddComponentInstance(section.SectionInformation.Name, section);
				}
			}
		}

		/// <summary>Processes the castle resource and build the castle configuration store.</summary>
		/// <param name="resource">The resource to use. This may be derived from the castle section in web.config or a default xml compiled into the assembly.</param>
		protected void ProcessResource(IResource resource)
		{
			XmlInterpreter interpreter = new XmlInterpreter(resource);
			interpreter.ProcessResource(resource, container.Kernel.ConfigurationStore);
		}

		/// <summary>Sets up components in the inversion of control container.</summary>
		protected void InstallComponents()
		{
			DefaultComponentInstaller installer = new DefaultComponentInstaller();
			installer.SetUp(container, container.Kernel.ConfigurationStore);
		}


		public void Attach(HttpApplication application)
		{
			Resolve<IRequestLifeCycleHandler>().Init(application);
		}

		public void Initialize()
		{
			IPluginBootstrapper invoker = Container.Resolve<IPluginBootstrapper>();
			invoker.InitializePlugins(this, invoker.GetPluginDefinitions());
		}

		#endregion

		#region Container Methods

		/// <summary>Resolves a service configured in the factory.</summary>
		public T Resolve<T>() where T:class 
		{
			return Container.Resolve<T>();
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
		/// <param name="classType">The type of component to register.</param>
		public void AddComponent(string key, Type classType)
		{
			Container.AddComponent(key, classType);
		}

		/// <summary>Registers a component in the IoC container.</summary>
		/// <param name="key">A unique key.</param>
		/// <param name="serviceType">The type of service to provide.</param>
		/// <param name="classType">The type of component to register.</param>
		public void AddComponent(string key, Type serviceType, Type classType)
		{
			Container.AddComponent(key, serviceType, classType);
		}

		/// <summary>Adds a compnent instance to the container.</summary>
		/// <param name="key">A unique key.</param>
		/// <param name="serviceType">The type of service to provide.</param>
		/// <param name="instance">The service instance to add.</param>
		public void AddComponentInstance(string key, Type serviceType, object instance)
		{
			Container.Kernel.AddComponentInstance(key, serviceType, instance);
		}

		public void AddFacility(string key, object facility)
		{
			if (facility is IFacility)
				Container.AddFacility(key, facility as IFacility);
			else
				throw new ArgumentException("Only classes implementing Castle.MicroKernel.IFacilty are supported.");
		}

		public void Release(object instance)
		{
			Container.Release(instance);
		}

		#endregion
    }
}