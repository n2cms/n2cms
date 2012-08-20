using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using N2.Plugin;

namespace N2.Engine.MediumTrust
{
	public class MediumTrustServiceContainer : ServiceContainerBase
	{
		bool isInitialized = false;

		private readonly Engine.Logger<MediumTrustServiceContainer> logger;
		private readonly IDictionary<Type, Type> waitingList = new Dictionary<Type, Type>();
		private readonly IDictionary<Type, object> container = new Dictionary<Type, object>();
		private readonly IDictionary<Type, Func<Type, object>> resolvers = new Dictionary<Type, Func<Type, object>>();
		private readonly IDictionary<Type, Func<Type, IEnumerable<object>>> listResolvers = new Dictionary<Type, Func<Type, IEnumerable<object>>>();

		public override void AddComponentLifeStyle(string key, Type serviceType, ComponentLifeStyle lifeStyle)
		{
			switch (lifeStyle)
			{
				case ComponentLifeStyle.Transient:
					RegisterTransientResolver(key, serviceType, serviceType);
					break;
				default:
					RegisterSingletonResolver(key, serviceType, serviceType);
					CheckForAutoStart(key, serviceType, serviceType);
					break;
			}
		}

		public override void AddComponentWithParameters(string key, Type serviceType, Type classType,
		                                                IDictionary<string, string> properties)
		{
			throw new NotImplementedException();
		}

		public override void AddComponentInstance(string key, Type serviceType, object instance)
		{
			if (resolvers.ContainsKey(serviceType))
				return;

			container[serviceType] = instance;
			resolvers[serviceType] = ReturnContainerInstance;

			if (instance is IAutoStart)
			{
				logger.Info("Starting " + instance);
				(instance as IAutoStart).Start();
			}
		}

		public override void AddComponent(string key, Type serviceType, Type classType)
		{
			RegisterSingletonResolver(key, serviceType, classType);
			CheckForAutoStart(key, serviceType, classType);
		}

		public override object Resolve(string key)
		{
			return Resolve(Type.GetType(key));
		}

		public override object Resolve(Type serviceType)
		{
			if (resolvers.ContainsKey(serviceType))
				return resolvers[serviceType](serviceType);

			if (serviceType.IsGenericType)
			{
				Type baseDefinition = serviceType.GetGenericTypeDefinition();

				if (resolvers.ContainsKey(baseDefinition))
				{
					return resolvers[baseDefinition](serviceType);
				}
			}

			throw new N2Exception("Couldn't find any service of the type " + serviceType);
		}

		/// <summary>Resolves all services serving the given interface.</summary>
		/// <param name="serviceType">The type of service to resolve.</param>
		/// <returns>All services registered to serve the provided interface.</returns>
		public override IEnumerable<object> ResolveAll(Type serviceType)
		{
			Func<Type, IEnumerable<object>> listResolver;
			if (!listResolvers.TryGetValue(serviceType, out listResolver))
				//return Activator.CreateInstance(serviceType.MakeArrayType(), 0) as Array;
				return new object[0];
			
			return listResolver(serviceType);
			//var instances = listResolver(serviceType).ToList().ToArray();
			//var returnArray = Activator.CreateInstance(serviceType.MakeArrayType(), instances.Length) as Array;
			//Array.Copy(instances, returnArray, instances.Length);
			//return returnArray;
		}

		/// <summary>Resolves all services.</summary>
		/// <returns>All registered services.</returns>
		public override IEnumerable<ServiceInfo> Diagnose()
		{
			return resolvers.Keys
				.Select(t => new ServiceInfo { Key = t.FullName, ServiceType = t, ImplementationType = t, Resolve = () => Resolve(t), ResolveAll = () => ResolveAll(t) });
		}

		/// <summary>Resolves all services of the given type.</summary>
		/// <typeparam name="T">The type of service to resolve.</typeparam>
		/// <returns>All services registered to serve the provided interface.</returns>
		public override IEnumerable<T> ResolveAll<T>()
		{
			return ResolveAll(typeof(T)).Cast<T>();
		}

		public override void Release(object instance)
		{
			foreach (var pair in container)
			{
				if (pair.Value == instance)
				{
					container.Remove(pair.Key);
					break;
				}
			}
		}

		public override void StartComponents()
		{
			isInitialized = true;
			List<Type> keys = new List<Type>(waitingList.Keys);
			foreach (var key in keys)
			{
				try
				{
					CheckForAutoStart(key.FullName.ToLowerInvariant(), key, waitingList[key]);
					waitingList.Remove(key);
				}
				catch (MissingDependencyException)
				{
				}
			}
		}

		private object ReturnContainerInstance(Type serviceType)
		{
			if (!container.ContainsKey(serviceType))
				throw new N2Exception("Couldn't find any service of the type " + serviceType);

			return container[serviceType];
		}

		private void CheckForAutoStart(string key, Type serviceType, Type classType)
		{
			if (IsAutoStart(classType))
			{
				if (!isInitialized)
				{
					waitingList[serviceType] = classType;
					return;
				}

				IAutoStart instance = Resolve(serviceType) as IAutoStart;
				logger.Info("Starting " + instance);
				instance.Start();
			}
		}

		private static bool IsAutoStart(Type classType)
		{
			return typeof(IAutoStart).IsAssignableFrom(classType);
		}

		private void RegisterSingletonResolver(string key, Type serviceType, Type classType)
		{
			if (resolvers.ContainsKey(serviceType))
			{
				logger.Info("Already contains service " + serviceType + ". " + classType + " is now default.");
			}

			var instanceResolver = CreateInstanceResolver(key, classType);
			AddBaseTypes(resolvers, serviceType, instanceResolver);

			var listResolver = CreateListResolver(serviceType, instanceResolver);
			AddBaseTypes(listResolvers, serviceType, listResolver);
		}

		private Func<Type, object> CreateInstanceResolver(string key, Type classType)
		{
			if (classType.ContainsGenericParameters)
			{
				return delegate(Type type)
				{
					object instance;
					if (container.TryGetValue(type, out instance))
						return instance;

					logger.Info("Creating " + classType);
					object componentInstance = CreateInstance(key, type, classType);
					container[type] = componentInstance;
					return componentInstance;
				};
			}
			else
			{
				return delegate(Type type)
				{
					object instance;
					if (container.TryGetValue(classType, out instance))
						return instance;

					object componentInstance = CreateInstance(key, type, classType);
					container[classType] = componentInstance;
					container[type] = componentInstance;
					return componentInstance;
				};
			}
		}

		private Func<Type, IEnumerable<object>> CreateListResolver(Type serviceType, Func<Type, object> resolver)
		{
			Func<Type, IEnumerable<object>> previousListResolver, listResolver;
			if (listResolvers.TryGetValue(serviceType, out previousListResolver))
			{
				listResolver = delegate(Type type)
				{
					List<object> instances = new List<object>();
					instances.AddRange(previousListResolver(type));
					instances.Add(resolver(type));
					return instances;
				};
			}
			else
			{
				listResolver = delegate(Type type)
				{
					return new object[] { resolver(type) };
				};
			}
			return listResolver;
		}

		private void AddBaseTypes<T>(IDictionary<Type, Func<Type, T>> resolvers, Type serviceType, Func<Type, T> resolver)
		{
			resolvers[serviceType] = resolver;
			if (serviceType.IsClass && serviceType != typeof(object))
				AddBaseTypes(resolvers, serviceType.BaseType, resolver);
		}

		private void RegisterTransientResolver(string key, Type serviceType, Type classType)
		{
			if (IsAutoStart(classType))
				resolvers[serviceType] = delegate(Type type) 
				{
					var instance = CreateInstance(key, type, classType);
					((IAutoStart)instance).Start();
					return instance;
				};
			else
				resolvers[serviceType] = delegate(Type type) { return CreateInstance(key, type, classType); };
		}

		protected object CreateInstance(string key, Type serviceType, Type classType)
		{
			if (classType.ContainsGenericParameters)
			{
				Type[] arguments = serviceType.GetGenericArguments();
				classType = classType.MakeGenericType(arguments);
			}

			ConstructorFindInfo constructorInfo = FindBestConstructor(classType);

			if (constructorInfo.ConstructorInfo == null)
			{
				var errorMessage = new StringBuilder("Could not find resolvable constructor for class " + classType.FullName);

				foreach (ParameterInfo parameter in constructorInfo.CouldNotFindParameters)
				{
					errorMessage.AppendLine("\nCould not resolve " + parameter.ParameterType);
				}

				throw new MissingDependencyException(errorMessage.ToString());
			}
			ConstructorInfo constructor = constructorInfo.ConstructorInfo;
			
			object[] parameters = CreateConstructorParameters(constructor.GetParameters());
			object componentInstance = constructor.Invoke(parameters);
			AddComponentInstance(key, serviceType, componentInstance);
			return componentInstance;
		}

		protected virtual object[] CreateConstructorParameters(ParameterInfo[] parameterInfos)
		{
			var parameters = new object[parameterInfos.Length];
			for (int i = 0; i < parameterInfos.Length; i++)
			{
				var parameterType = parameterInfos[i].ParameterType;
				if(parameterType.IsArray)
				{
					var instancesArray = ResolveAll(parameterType.GetElementType()).ToArray();
					var injectedArray = Activator.CreateInstance(parameterType, instancesArray.Length) as Array;
					Array.Copy(instancesArray, injectedArray, instancesArray.Length);
					parameters[i] = injectedArray;
				}
				else
					parameters[i] = Resolve(parameterType);
			}
			return parameters;
		}

		protected virtual ConstructorFindInfo FindBestConstructor(Type classType)
		{
			int maxParameters = -1;
			ConstructorInfo bestConstructor = null;
			ParameterInfo[] couldNotFindParameters = null;
			foreach (ConstructorInfo constructor in classType.GetConstructors())
			{
				ParameterInfo[] parameters = constructor.GetParameters();
				ParameterInfo[] couldNotFindParametersTemp = ResolveAllParameters(parameters);
				if (parameters.Length <= maxParameters || couldNotFindParametersTemp.Length != 0)
				{
					couldNotFindParameters = couldNotFindParametersTemp;
					continue;
				}

				bestConstructor = constructor;
				maxParameters = parameters.Length;
				couldNotFindParameters = new ParameterInfo[0];
			}

			return new ConstructorFindInfo
			       	{
			       		ConstructorInfo = bestConstructor,
			       		CouldNotFindParameters = couldNotFindParameters
			       	};
		}

		private ParameterInfo[] ResolveAllParameters(IEnumerable<ParameterInfo> parameters)
		{
			var result = new List<ParameterInfo>();
			foreach (ParameterInfo parameter in parameters)
			{
				bool resolverExists = resolvers.ContainsKey(parameter.ParameterType);
				bool genericResolverExists = (parameter.ParameterType.IsGenericType && resolvers.ContainsKey(parameter.ParameterType.GetGenericTypeDefinition()));
				bool arrayResolverExists = (parameter.ParameterType.IsArray && resolvers.ContainsKey(parameter.ParameterType.GetElementType()));
				if (!resolverExists && !genericResolverExists && !arrayResolverExists && !parameter.ParameterType.IsArray)
				{
					result.Add(parameter);
				}
			}
			return result.ToArray();
		}

		#region Nested type: ConstructorFindInfo

		public class ConstructorFindInfo
		{
			public ConstructorInfo ConstructorInfo { get; set; }

			public ParameterInfo[] CouldNotFindParameters { get; set; }
		}

		#endregion

		class MissingDependencyException : Exception
		{
			public MissingDependencyException(string message)
				: base(message)
			{
			}
		}
	}
}