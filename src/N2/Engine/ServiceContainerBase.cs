using System;
using System.Collections.Generic;
using N2.Configuration;

namespace N2.Engine
{
    /// <summary>
    /// Wraps an inversion of control container. The default container used by N2 is Windsor.
    /// </summary>
	public abstract class ServiceContainerBase : IServiceContainer
	{
		#region IServiceContainer Members

		public abstract void AddFacility(string key, object facility);
		public abstract void AddComponent(string key, Type serviceType, Type classType);
		public abstract void AddComponentWithParameters(string key, Type serviceType, Type classType, IDictionary<string, string> properties1);
		public abstract void AddComponentInstance(string key, Type serviceType, object instance);
		public abstract void AddComponentLifeStyle(string key, Type serviceType, ComponentLifeStyle lifeStyle);
		public abstract object Resolve(Type type);
		public abstract object Resolve(string key);
		public abstract void Release(object instance);
		public abstract void Configure(IEngine engine, EngineSection engineConfig);
		public abstract void StartComponents();

		public virtual T Resolve<T>()
		{
			return (T)Resolve(typeof(T));
		}

		public virtual T Resolve<T>(string key)
		{
			return (T)Resolve(key);
		}

		#endregion
	}
}