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
using System.Diagnostics;
using System.Web;
using Castle.MicroKernel;
using Castle.Windsor;
using Castle.Windsor.Configuration;
using Castle.Windsor.Configuration.Interpreters;
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
	/// This principal gateway to N2 serivecs. The class is responsible for 
	/// initializing and providing access to the services that compose N2.
	/// </summary>
	public class CmsEngine : IEngine
	{
		#region Private Fields

		private IWindsorContainer container;

		#endregion

		#region Constructors

		public CmsEngine(IWindsorContainer container)
		{
			this.container = container;
		}

		public CmsEngine()
			: this(new WindsorContainer(new XmlInterpreter()))
		{
		}

		public CmsEngine(IConfigurationInterpreter configurationInterpreter)
			: this(new WindsorContainer(configurationInterpreter))
		{
		}

		public CmsEngine(string xmlFile)
			: this(new WindsorContainer(xmlFile))
		{
		}

		#endregion

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

		#endregion

		#region Methods

		public void Attach(HttpApplication application)
		{
			Resolve<IRequestLifeCycleHandler>().Init(application);
		}

		public void InitializePlugIns()
		{
			Debug.WriteLine("DefaultFactory: initializing plugins");
			IPluginInitializerInvoker invoker = Container.Resolve<IPluginInitializerInvoker>();
			invoker.InitializePlugins(this, invoker.GetPluginDefinitions());
		}

		#endregion

		#region Container Methods

		/// <summary>Resolves a service configured in the factory.</summary>
		public T Resolve<T>() where T:class 
		{
			return container.Resolve<T>();
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