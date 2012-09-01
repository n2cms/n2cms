using System;
using System.Collections.Generic;
using System.Linq;
using N2.Configuration;
using N2.Web;
using N2.Edit;
using N2.Persistence;

namespace N2.Engine
{
	/// <summary>
	/// Configures the inversion of control container with services used by N2 CMS.
	/// </summary>
	public class ContainerConfigurer
	{
		public virtual void Configure(IEngine engine, EventBroker broker, ConfigurationManagerWrapper configuration)
		{
			engine.Container.AddComponentInstance("n2.configuration", typeof(ConfigurationManagerWrapper), configuration);
			engine.Container.AddComponentInstance("n2.engine", typeof(IEngine), engine);
			engine.Container.AddComponentInstance("n2.container", typeof(IServiceContainer), engine.Container);
			engine.Container.AddComponentInstance("n2.containerConfigurer", typeof(ContainerConfigurer), this);

			AddComponentInstance(engine.Container, configuration.GetConnectionStringsSection());
			AddComponentInstance(engine.Container, configuration.Sections.Engine);
			RegisterConfiguredComponents(engine.Container, configuration.Sections.Engine);
			AddComponentInstance(engine.Container, configuration.Sections.Web);
			InitializeEnvironment(engine.Container, configuration.Sections);
			AddComponentInstance(engine.Container, configuration.Sections.Database);
			AddComponentInstance(engine.Container, configuration.Sections.Management);

			AddComponentInstance(engine.Container, broker);

			var skipTypes = configuration.Sections.Engine.Components.GetConfiguredServiceTypes();
			AddComponentUnlessConfigured(engine.Container, typeof(BasicTemporaryFileHelper), typeof(BasicTemporaryFileHelper), skipTypes);
			AddComponentUnlessConfigured(engine.Container, typeof(TypeCache), typeof(TypeCache), skipTypes);
			AddComponentUnlessConfigured(engine.Container, typeof(ITypeFinder), typeof(WebAppTypeFinder), skipTypes);
			AddComponentUnlessConfigured(engine.Container, typeof(ServiceRegistrator), typeof(ServiceRegistrator), skipTypes);

			var registrator = engine.Container.Resolve<ServiceRegistrator>();
			var services = registrator.FindServices();
			var configurationKeys = configuration.GetComponentConfigurationKeys();
			services = registrator.FilterServices(services, configurationKeys);
			services = registrator.FilterServices(services, skipTypes);
			registrator.RegisterServices(services);

			InitializeUrlParser(engine.Container);
		}

		private void AddComponentUnlessConfigured(IServiceContainer container, Type serviceType, Type instanceType, IEnumerable<Type> skipList)
		{
			if (skipList.Contains(serviceType))
				return;

			container.AddComponent(serviceType.FullName + "->" + instanceType.FullName, serviceType, instanceType);
		}

		private void AddComponentInstance(IServiceContainer container, object instance)
		{
			container.AddComponentInstance(instance.GetType().FullName, instance.GetType(), instance);
		}

		protected virtual void InitializeEnvironment(IServiceContainer container, ConfigurationManagerWrapper.ContentSectionTable config)
		{
			if (config.Web != null)
			{
				Url.DefaultExtension = config.Web.Web.Extension;
				PathData.PageQueryKey = config.Web.Web.PageQueryKey;
				PathData.ItemQueryKey = config.Web.Web.ItemQueryKey;
				PathData.PartQueryKey = config.Web.Web.PartQueryKey;
				PathData.PathKey = config.Web.Web.PathDataKey;

				var skipList = config.Engine.Components.GetConfiguredServiceTypes();
				if (config.Web.Web.IsWeb)
					AddComponentUnlessConfigured(container, typeof(N2.Web.IWebContext), typeof(N2.Web.AdaptiveContext), skipList);
				else
					AddComponentUnlessConfigured(container, typeof(N2.Web.IWebContext), typeof(N2.Web.ThreadContext), skipList);
			}
			if (config.Management != null)
			{
				SelectionUtility.SelectedQueryKey = config.Management.Paths.SelectedQueryKey;
				Url.SetToken("{Selection.SelectedQueryKey}", SelectionUtility.SelectedQueryKey);
			}
		}

		private void InitializeUrlParser(IServiceContainer container)
		{
			var config = container.Resolve<HostSection>();
			IUrlParser parser;
			if (config.MultipleSites)
				parser = new MultipleSitesParser(container.Resolve<IPersister>(), container.Resolve<IWebContext>(), container.Resolve<IHost>(), container.Resolve<Plugin.ConnectionMonitor>(), config);
			else
				parser = new UrlParser(container.Resolve<IPersister>(), container.Resolve<IWebContext>(), container.Resolve<IHost>(), container.Resolve<Plugin.ConnectionMonitor>(), config);

			if (config.Web.Urls.EnableCaching)
				parser = new CachingUrlParserDecorator(parser, container.Resolve<IPersister>(), container.Resolve<IWebContext>(), container.Resolve<CacheWrapper>());

			container.AddComponentInstance("n2.urlParser", typeof(IUrlParser), parser);
		}

		protected virtual void RegisterConfiguredComponents(IServiceContainer container, EngineSection engineConfig)
		{
			foreach (ComponentElement component in engineConfig.Components)
			{
				Type implementation = Type.GetType(component.Implementation);
				Type service = Type.GetType(component.Service);

				if (implementation == null)
					throw new ComponentRegistrationException(component.Implementation);

				if (service == null && !String.IsNullOrEmpty(component.Service))
					throw new ComponentRegistrationException(component.Service);

				if (service == null)
					service = implementation;

				string name = component.Key;
				if (string.IsNullOrEmpty(name))
					name = implementation.FullName;

				if (component.Parameters.Count == 0)
				{
					container.AddComponent(name, service, implementation);
				}
				else
				{
					container.AddComponentWithParameters(name, service, implementation,
														 component.Parameters.ToDictionary());
				}
			}
		}
	}
}
