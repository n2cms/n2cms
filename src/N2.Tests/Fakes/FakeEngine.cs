using System;
using System.Collections.Generic;
using N2.Definitions;
using N2.Engine;

namespace N2.Tests.Fakes
{
	public class FakeEngine : IEngine
	{
		Dictionary<Type, object> services = new Dictionary<Type, object>();
		#region IEngine Members

		public N2.Persistence.IPersister Persister
		{
			get { throw new NotImplementedException(); }
		}

		public N2.Web.IUrlParser UrlParser
		{
			get { throw new NotImplementedException(); }
		}

		public IDefinitionManager Definitions
		{
			get { return Resolve<IDefinitionManager>(); }
		}

		public N2.Integrity.IIntegrityManager IntegrityManager
		{
			get { throw new NotImplementedException(); }
		}

		public N2.Security.ISecurityManager SecurityManager
		{
			get { throw new NotImplementedException(); }
		}

		public N2.Edit.IEditManager EditManager
		{
			get { throw new NotImplementedException(); }
		}

		public N2.Web.IWebContext RequestContext
		{
			get { throw new NotImplementedException(); }
		}

		public N2.Web.IHost Host
		{
			get { throw new NotImplementedException(); }
		}

		public void Initialize()
		{
			throw new NotImplementedException();
		}

		public void Attach(System.Web.HttpApplication application)
		{
			throw new NotImplementedException();
		}

		public T Resolve<T>() where T : class
		{
			return (T) services[typeof (T)];
		}

		public object Resolve(Type serviceType)
		{
			return services[serviceType];
		}

		public object Resolve(string key)
		{
			throw new NotImplementedException();
		}

		public void AddComponent(string key, Type classType)
		{
			AddComponent(key, classType, classType);
		}

		public void AddComponent(string key, Type serviceType, Type classType)
		{
			AddComponentInstance(key, serviceType, Activator.CreateInstance(classType));
		}

		public void AddComponentInstance(string key, Type serviceType, object instance)
		{
			services[serviceType] = instance;
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
	}
}
