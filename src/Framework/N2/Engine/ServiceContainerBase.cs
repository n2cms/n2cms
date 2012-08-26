using System;
using System.Collections.Generic;
using N2.Plugin;

namespace N2.Engine
{
	/// <summary>
	/// Wraps an inversion of control container. The default container used by N2 is Windsor.
	/// </summary>
	public abstract class ServiceContainerBase : IServiceContainer
	{
		#region IServiceContainer Members

		/// <summary>Registers the service of the specified type with the container.</summary>
		public abstract void AddComponent(string key, Type serviceType, Type classType);

		/// <summary>Registers the service of the specified type with the container, giving a number of properties for use during instantiation.</summary>
		public abstract void AddComponentWithParameters(string key, Type serviceType, Type classType, IDictionary<string, string> properties1);

		/// <summary>Registers the specified instance to the container as for the given service.</summary>
		public abstract void AddComponentInstance(string key, Type serviceType, object instance);

		/// <summary>Registers the service of the specified type with the container as using the specified lifestyle.</summary>
		public abstract void AddComponentLifeStyle(string key, Type serviceType, ComponentLifeStyle lifeStyle);

		/// <summary>Returns the first registered service of the given type.</summary>
		public abstract object Resolve(Type type);

		/// <summary>Returns the service registered for the given key.</summary>
		public abstract object Resolve(string key);

		/// <summary>Frees the resources assiciated with the given object instance</summary>
		public abstract void Release(object instance);

		/// <summary>Returns the service registered for the given key.</summary>
		public virtual T Resolve<T>() where T : class
		{
			return (T)Resolve(typeof(T));
		}

		/// <summary>Returns the first registered service of the given type.</summary>
		public virtual T Resolve<T>(string key) where T : class
		{
			return (T)Resolve(key);
		}

		/// <summary>Starts any <see cref="IAutoStart"/> components in the container.</summary>
		public abstract void StartComponents();

		/// <summary>Resolves all services serving the given interface.</summary>
		/// <param name="serviceType">The type of service to resolve.</param>
		/// <returns>All services registered to serve the provided interface.</returns>
		public abstract IEnumerable<object> ResolveAll(Type serviceType);

		/// <summary>Retrieves registered types.</summary>
		/// <returns>All registered services.</returns>
		public abstract IEnumerable<ServiceInfo> Diagnose();

		/// <summary>Resolves all services of the given type.</summary>
		/// <typeparam name="T">The type of service to resolve.</typeparam>
		/// <returns>All services registered to serve the provided interface.</returns>
		public abstract IEnumerable<T> ResolveAll<T>() where T : class;

		#endregion
	}
}