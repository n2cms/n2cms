using System;
using System.Collections.Generic;
using N2.Engine.Configuration;
using N2.Plugin;

namespace N2.Engine
{
	/// <summary>
	/// Wraps an inversion of control container. The default container used by N2 is Windsor.
	/// </summary>
	public interface IServiceContainer
	{
		/// <summary>
		/// Registers the service of the specified type with the container
		/// </summary>
		/// <param name="key"></param>
		/// <param name="serviceType"></param>
		/// <param name="classType"></param>
		void AddComponent(string key, Type serviceType, Type classType);

		/// <summary>
		/// Registers the specified instance to the container as for the given service
		/// </summary>
		/// <param name="key"></param>
		/// <param name="serviceType"></param>
		/// <param name="instance"></param>
		void AddComponentInstance(string key, Type serviceType, object instance);

		/// <summary>
		/// Registers the service of the specified type with the container as using the specified lifestyle
		/// </summary>
		/// <param name="key"></param>
		/// <param name="serviceType"></param>
		/// <param name="lifeStyle"></param>
		void AddComponentLifeStyle(string key, Type serviceType, ComponentLifeStyle lifeStyle);

		/// <summary>
		/// Registers the service of the specified type with the container, giving a number of properties for use during instantiation.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="serviceType"></param>
		/// <param name="classType"></param>
		/// <param name="properties"></param>
		void AddComponentWithParameters(string key, Type serviceType, Type classType, IDictionary<string, string> properties);

		/// <summary>
		/// Returns the first registered service of the given type
		/// </summary>
		/// <typeparam name="T">The type to resolve</typeparam>
		/// <returns></returns>
		T Resolve<T>();

		/// <summary>
		/// Returns the service registered for the given key.
		/// </summary>
		/// <param name="key"></param>
		/// <typeparam name="T">The type to resolve</typeparam>
		/// <returns></returns>
		T Resolve<T>(string key);

		/// <summary>
		/// Returns the first registered service of the given type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		object Resolve(Type type);

		/// <summary>
		/// Frees the resources assiciated with the given object instance
		/// </summary>
		/// <param name="instance"></param>
		void Release(object instance);

		/// <summary>
		/// Returns a component that performs Container-specific configuration, such as adding the core N2 and any user-defined services.
		/// </summary>
		IServiceContainerConfigurer ServiceContainerConfigurer { get; }

		/// <summary>
		/// Starts any <see cref="IAutoStart"/> components in the container
		/// </summary>
		void StartComponents();
	}
}