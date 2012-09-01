using System;
using System.Collections.Generic;
using N2.Plugin;

namespace N2.Engine
{
	/// <summary>
	/// Wraps an inversion of control container. The default container used by N2 is Windsor.
	/// </summary>
	public interface IServiceContainer
	{
		/// <summary>Registers the service of the specified type with the container.</summary>
		void AddComponent(string key, Type serviceType, Type classType);

		/// <summary>Registers the specified instance to the container as for the given service.</summary>
		void AddComponentInstance(string key, Type serviceType, object instance);

		/// <summary>Registers the service of the specified type with the container as using the specified lifestyle.</summary>
		void AddComponentLifeStyle(string key, Type serviceType, ComponentLifeStyle lifeStyle);

		/// <summary>Registers the service of the specified type with the container, giving a number of properties for use during instantiation.</summary>
		void AddComponentWithParameters(string key, Type serviceType, Type classType, IDictionary<string, string> properties);

		/// <summary>Returns the first registered service of the given type.</summary>
		T Resolve<T>() where T : class;

		/// <summary>Returns the service registered for the given key.</summary>
		T Resolve<T>(string key) where T : class;

		/// <summary>Returns the first registered service of the given type.</summary>
		object Resolve(Type type);

		/// <summary>Frees the resources assiciated with the given object instance</summary>
		void Release(object instance);

		/// <summary>Resolves all services serving the given interface.</summary>
		/// <param name="serviceType">The type of service to resolve.</param>
		/// <returns>All services registered to serve the provided interface.</returns>
		IEnumerable<object> ResolveAll(Type serviceType);

		/// <summary>Retrieves registered types.</summary>
		/// <returns>All registered services.</returns>
		IEnumerable<ServiceInfo> Diagnose();

		/// <summary>Resolves all services of the given type.</summary>
		/// <typeparam name="T">The type of service to resolve.</typeparam>
		/// <returns>All services registered to serve the provided interface.</returns>
		IEnumerable<T> ResolveAll<T>() where T : class;

		/// <summary>Starts any <see cref="IAutoStart"/> components in the container.</summary>
		void StartComponents();
	}
}