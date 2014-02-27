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
using N2.Configuration;
using N2.Definitions;
using N2.Edit;
using N2.Integrity;
using N2.Persistence;
using N2.Plugin;
using N2.Security;
using N2.Web;
using System.Diagnostics;

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
        /// Creates an instance of the content engine using default settings and configuration.
        /// </summary>
        public ContentEngine()
            : this(new TinyIoC.TinyIoCServiceContainer(), EventBroker.Instance, new ContainerConfigurer())
        {
        }

        /// <summary>
        /// Creates an instance of the content engine using default settings and configuration.
        /// </summary>
        public ContentEngine(IServiceContainer container)
            : this(container, EventBroker.Instance, new ContainerConfigurer())
        {
        }

        /// <summary>Sets the current container to the given container.</summary>
        /// <param name="container">An previously prepared service container.</param>
        /// <param name="broker"></param>
        public ContentEngine(IServiceContainer container, EventBroker broker, ContainerConfigurer configurer)
        {
            this.container = container;
            configurer.Configure(this, broker, new ConfigurationManagerWrapper());
        }

        /// <summary>Tries to determine runtime parameters from the given configuration.</summary>
        /// <param name="config">The configuration to use.</param>
        /// <param name="sectionGroup">The configuration section to retrieve configuration from</param>
        /// <param name="broker">Web ap event provider</param>
        public ContentEngine(System.Configuration.Configuration config, string sectionGroup, IServiceContainer container, EventBroker broker, ContainerConfigurer configurer)
        {
            if (config == null) throw new ArgumentNullException("config");
            if (string.IsNullOrEmpty(sectionGroup)) throw new ArgumentException("Must be non-empty and match a section group in the configuration file.", "sectionGroup");

            this.container = container;
            configurer.Configure(this, broker, new ConfigurationReadingWrapper(config, sectionGroup));
        }

        private class ConfigurationReadingWrapper : ConfigurationManagerWrapper
        {
            System.Configuration.Configuration config;

            public ConfigurationReadingWrapper(System.Configuration.Configuration config, string sectionGroup)
                : base(sectionGroup)
            {
                this.config = config;
            }

            public override T GetSection<T>(string sectionName, bool required = true)
            {
                return config.GetSection(sectionName) as T;
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

        /// <summary>Gets the url parser responsible of mapping managementUrls to items and back again.</summary>
        public IUrlParser UrlParser
        {
            get { return Resolve<IUrlParser>(); }
        }

        /// <summary>Gets the edit manager responsible for plugins in edit mode.</summary>
        public IEditManager EditManager
        {
            get { return Resolve<IEditManager>(); }
        }

        public IEditUrlManager ManagementPaths
        {
            get { return Resolve<IEditUrlManager>(); }
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
            var bootstrapper = container.Resolve<IPluginBootstrapper>();
            var plugins = bootstrapper.GetPluginDefinitions();
            bootstrapper.InitializePlugins(this, plugins);

            container.StartComponents();
        }

        #endregion

        #region Container Methods

        /// <summary>Resolves a service configured in the factory.</summary>
        [DebuggerStepThrough]
        public T Resolve<T>() where T : class
        {
            return (T) Container.Resolve(typeof (T));
        }

        public object Resolve(Type serviceType)
        {
            return Container.Resolve(serviceType);
        }

        /// <summary>Registers a component in the IoC container.</summary>
        /// <param name="key">A unique key.</param>
        /// <param name="serviceType">The type of component to register.</param>
        [Obsolete("Use Container.AddComponent")]
        public virtual void AddComponent(string key, Type serviceType)
        {
            AddComponent(key, serviceType, serviceType);
        }

        /// <summary>Registers a component in the IoC container.</summary>
        /// <param name="key">A unique key.</param>
        /// <param name="serviceType">The type of service to provide.</param>
        /// <param name="classType">The type of component to register.</param>
        [Obsolete("Use Container.AddComponent")]
        public virtual void AddComponent(string key, Type serviceType, Type classType)
        {
            Container.AddComponent(key, serviceType, classType);
        }

        /// <summary>Adds a compnent instance to the container.</summary>
        /// <param name="key">A unique key.</param>
        /// <param name="serviceType">The type of service to provide.</param>
        /// <param name="instance">The service instance to add.</param>
        [Obsolete("Use Container.AddComponentInstance")]
        public void AddComponentInstance(string key, Type serviceType, object instance)
        {
            Container.AddComponentInstance(key, serviceType, instance);
        }

        [Obsolete("Use Container.AddComponentLifeStyle")]
        public void AddComponentLifeStyle(string key, Type classType, ComponentLifeStyle lifeStyle)
        {
            Container.AddComponentLifeStyle(key, classType, lifeStyle);
        }

        [Obsolete("Use Container.Release")]
        public void Release(object instance)
        {
            Container.Release(instance);
        }

        [Obsolete("Use Container.AddComponentInstance")]
        public T AddComponentInstance<T>(T instance) where T : class
        {
            if (instance != null)
            {
                AddComponentInstance(typeof (T).Name, typeof (T), instance);
            }
            return instance;
        }

        public ContentHelperBase Content
        {
            get { return new ContentHelperBase(() => this, () => RequestContext.CurrentPath); }
        }

        #endregion

        public ConfigurationManagerWrapper Config
        {
            get { return Resolve<ConfigurationManagerWrapper>(); }
        }
    }
}
