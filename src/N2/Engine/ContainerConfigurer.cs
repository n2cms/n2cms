using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Configuration;
using System.Configuration;
using N2.Web;

namespace N2.Engine
{
	/// <summary>
	/// Configures the inversion of control container with services used by N2 CMS.
	/// </summary>
	public class ContainerConfigurer
	{
		public virtual void Configure(IEngine engine, EventBroker broker, HostSection hostSection, EngineSection engineSection, DatabaseSection databaseSection, EditSection editSection, ConnectionStringsSection connectionStringsSection)
		{
			engine.Container.AddComponentInstance("n2.engine", typeof(IEngine), engine);
			engine.Container.AddComponentInstance("n2.container", typeof(IServiceContainer), engine.Container);
			engine.Container.AddComponentInstance("n2.containerConfigurer", typeof(ContainerConfigurer), this);

			AddComponentInstance(engine.Container, connectionStringsSection);
			AddComponentInstance(engine.Container, engineSection);
			if (engineSection != null)
				RegisterConfiguredComponents(engine.Container, engineSection);
			AddComponentInstance(engine.Container, hostSection);
			if (hostSection != null)
				InitializeEnvironment(engine.Container, hostSection);
			AddComponentInstance(engine.Container, databaseSection);
			AddComponentInstance(engine.Container, editSection);

			AddComponentInstance(engine.Container, broker);

			engine.Container.AddComponent("n2.typeFinder", typeof(ITypeFinder), typeof(WebAppTypeFinder));
			engine.Container.AddComponent("n2.webContext", typeof(N2.Web.IWebContext), typeof(N2.Web.AdaptiveContext));
			engine.Container.AddComponent("n2.serviceRegistrator", typeof(ServiceRegistrator), typeof(ServiceRegistrator));

			var registrator = engine.Container.Resolve<ServiceRegistrator>();
			registrator.RegisterServices(registrator.FindServices());
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
