using System;
using System.Linq;
using N2.Configuration;
using N2.Web;
using System.Collections.Generic;

namespace N2.Engine
{
	/// <summary>
	/// Configures the inversion of control container with services used by N2 CMS.
	/// </summary>
	public class ContainerConfigurer
	{
		/// <summary>
		/// Known configuration keys used to configure services.
		/// </summary>
		public static class ConfigurationKeys
		{
			/// <summary>Key used to configure services intended for medium trust.</summary>
			public const string MediumTrust = "MediumTrust";
			/// <summary>Key used to configure services intended for full trust.</summary>
			public const string FullTrust = "FullTrust";
		}

		public virtual void Configure(IEngine engine, EventBroker broker, ConfigurationManagerWrapper configuration)
		{
			configuration.Start();
			engine.Container.AddComponentInstance("n2.configuration", typeof(ConfigurationManagerWrapper), configuration);
			engine.Container.AddComponentInstance("n2.engine", typeof(IEngine), engine);
			engine.Container.AddComponentInstance("n2.container", typeof(IServiceContainer), engine.Container);
			engine.Container.AddComponentInstance("n2.containerConfigurer", typeof(ContainerConfigurer), this);

			AddComponentInstance(engine.Container, configuration.GetConnectionStringsSection());
			AddComponentInstance(engine.Container, configuration.Sections.Engine);
			if (configuration.Sections.Engine != null)
				RegisterConfiguredComponents(engine.Container, configuration.Sections.Engine);
			AddComponentInstance(engine.Container, configuration.Sections.Web);
			if (configuration.Sections.Web != null)
				InitializeEnvironment(engine.Container, configuration.Sections.Web);
			AddComponentInstance(engine.Container, configuration.Sections.Database);
			AddComponentInstance(engine.Container, configuration.Sections.Management);

			AddComponentInstance(engine.Container, broker);

			if(configuration.Sections.Web.Web.IsWeb)
				engine.Container.AddComponent("n2.typeFinder", typeof(ITypeFinder), typeof(WebAppTypeFinder));
			else
				engine.Container.AddComponent("n2.typeFinder", typeof(ITypeFinder), typeof(AppDomainTypeFinder));
			engine.Container.AddComponent("n2.webContext", typeof(N2.Web.IWebContext), typeof(N2.Web.AdaptiveContext));
			engine.Container.AddComponent("n2.serviceRegistrator", typeof(ServiceRegistrator), typeof(ServiceRegistrator));

			var registrator = engine.Container.Resolve<ServiceRegistrator>();
			var services = registrator.FindServices();
			var configurations = GetComponentConfigurations(configuration);
			services = registrator.FilterServices(services, configurations);
			registrator.RegisterServices(services);
		}

		protected virtual string[] GetComponentConfigurations(ConfigurationManagerWrapper configuration)
		{
			List<string> configurations = new List<string>();
			string trustConfiguration = (Utility.GetTrustLevel() > System.Web.AspNetHostingPermissionLevel.Medium)
				? ConfigurationKeys.FullTrust
				: ConfigurationKeys.MediumTrust;
			configurations.Add(trustConfiguration);
			var configured = configuration.GetContentSection<EngineSection>("engine").ComponentConfigurations;
			configurations.AddRange(configured.AddedElements.Select(e => e.Name));
			configurations.RemoveAll(c => configured.RemovedElements.Any(e => c == e.Name));
			return configurations.ToArray();
		}

		private void AddComponentInstance(IServiceContainer container, object instance)
		{
			container.AddComponentInstance(instance.GetType().FullName, instance.GetType(), instance);
		}

		protected virtual void InitializeEnvironment(IServiceContainer container, HostSection hostConfig)
		{
			if (hostConfig != null)
			{
				Url.DefaultExtension = hostConfig.Web.Extension;
				PathData.PageQueryKey = hostConfig.Web.PageQueryKey;
				PathData.ItemQueryKey = hostConfig.Web.ItemQueryKey;
				PathData.PartQueryKey = hostConfig.Web.PartQueryKey;

				if (!hostConfig.Web.IsWeb)
					container.AddComponentInstance("n2.webContext.notWeb", typeof(IWebContext), new ThreadContext());

				if (hostConfig.Web.Urls.EnableCaching)
					container.AddComponent("n2.web.cachingUrlParser", typeof(IUrlParser), typeof(CachingUrlParserDecorator));

				if (hostConfig.MultipleSites)
					container.AddComponent("n2.multipleSitesParser", typeof(IUrlParser), typeof(MultipleSitesParser));
				else
					container.AddComponent("n2.urlParser", typeof(IUrlParser), typeof(UrlParser));
			}
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
