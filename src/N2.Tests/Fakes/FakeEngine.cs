using System;
using System.Collections.Generic;
using N2.Definitions;
using N2.Engine;
using N2.Persistence;
using N2.Web;
using N2.Edit;
using N2.Security;
using N2.Integrity;

namespace N2.Tests.Fakes
{
	public class FakeEngine : IEngine
	{
		public FakeServiceContainer container = new FakeServiceContainer();

		#region IEngine Members

		public N2.Persistence.IPersister Persister
		{
			get { return container.Resolve<IPersister>(); }
		}

		public N2.Web.IUrlParser UrlParser
		{
			get { return container.Resolve<IUrlParser>(); }
		}

		public IDefinitionManager Definitions
		{
			get { return container.Resolve<IDefinitionManager>(); }
		}

		public N2.Integrity.IIntegrityManager IntegrityManager
		{
			get { return container.Resolve<IIntegrityManager>(); }
		}

		public N2.Security.ISecurityManager SecurityManager
		{
			get { return container.Resolve<ISecurityManager>(); }
		}

		public N2.Edit.IEditManager EditManager
		{
			get { return container.Resolve<IEditManager>(); }
		}

		public N2.Edit.IEditUrlManager ManagementPaths
		{
			get { return container.Resolve<IEditUrlManager>(); }
		}

		public N2.Web.IWebContext RequestContext
		{
			get { return container.Resolve<IWebContext>(); }
		}

		public N2.Web.IHost Host
		{
			get { return container.Resolve<IHost>(); }
		}

		public IServiceContainer Container
		{
			get { return container; }
		}

		public void Initialize()
		{
			throw new NotImplementedException();
		}

		public void Attach(EventBroker application)
		{
			throw new NotImplementedException();
		}

		public T Resolve<T>() where T : class
		{
			return Container.Resolve<T>();
		}

		public object Resolve(Type serviceType)
		{
			return Container.Resolve(serviceType);
		}

		public object Resolve(string key)
		{
			throw new NotImplementedException();
		}

		public void AddComponent(string key, Type serviceType)
		{
			AddComponent(key, serviceType, serviceType);
		}

		public void AddComponent(string key, Type serviceType, Type classType)
		{
			AddComponentInstance(key, serviceType, Activator.CreateInstance(classType));
		}

		public void AddComponentInstance(string key, Type serviceType, object instance)
		{
			container.AddComponentInstance(key, serviceType, instance);
		}

		public void AddComponentLifeStyle(string key, Type serviceType, ComponentLifeStyle lifeStyle)
		{
			container.AddComponent(key, serviceType, serviceType);
		}

		public void AddFacility(string key, object facility)
		{
			throw new NotImplementedException();
		}

		public void Release(object instance)
		{
			throw new NotImplementedException();
		}

		#endregion

		public class FakeServiceContainer : IServiceContainer
		{
			Dictionary<Type, object> services = new Dictionary<Type, object>();

			#region IServiceContainer Members

			public void AddComponent(string key, Type serviceType, Type classType)
			{
				services[serviceType] = Activator.CreateInstance(classType);
			}

			public void AddComponentInstance(string key, Type serviceType, object instance)
			{
				services[serviceType] = instance;
			}

			public void AddComponentLifeStyle(string key, Type serviceType, ComponentLifeStyle lifeStyle)
			{
				throw new NotImplementedException();
			}

			public void AddComponentWithParameters(string key, Type serviceType, Type classType, IDictionary<string, string> properties)
			{
				throw new NotImplementedException();
			}

			public T Resolve<T>()
			{
				if(services.ContainsKey(typeof(T)) == false)
					throw new InvalidOperationException("No component for service " + typeof(T).Name + " registered");

				return (T)services[typeof(T)];
			}

			public T Resolve<T>(string key)
			{
				return (T)services[typeof(T)];
			}

			public object Resolve(Type type)
			{
				return services[type];
			}

			public void Release(object instance)
			{
			}

			public Array ResolveAll(Type serviceType)
			{
				if (!this.services.ContainsKey(serviceType))
					return new object[0];

				return new object[] { Resolve(serviceType) };
			}

			public T[] ResolveAll<T>()
			{
				if (!this.services.ContainsKey(typeof(T)))
					return new T[0];

				return new T[] { Resolve<T>() };
			}

			public N2.Engine.Configuration.IServiceContainerConfigurer ServiceContainerConfigurer
			{
				get { throw new NotImplementedException(); }
			}

			public void StartComponents()
			{
				throw new NotImplementedException();
			}

			#endregion
		}

	}
}
