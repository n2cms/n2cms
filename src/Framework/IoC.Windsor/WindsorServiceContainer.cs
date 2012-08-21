using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using Castle.Facilities.Startable;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using N2.Plugin;

namespace N2.Engine.Castle
{
	/// <summary>
	/// Wraps usage of the Castle Windsor inversion of control container.
	/// </summary>
	public class WindsorServiceContainer : ServiceContainerBase
	{
		private readonly IWindsorContainer container;
		private readonly Engine.Logger<WindsorServiceContainer> logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="WindsorServiceContainer"/> 
		/// class using an empty Windsor container.
		/// </summary>
		public WindsorServiceContainer() : this(new WindsorContainer())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WindsorServiceContainer"/> 
		/// class using an existing container.  Any additional/replacement service 
		/// defined by the user will be added to the container.
		/// </summary>
		/// <param name="container">The container.</param>
		public WindsorServiceContainer(IWindsorContainer container)
		{
			this.container = container;
			container.Kernel.ComponentModelCreated += new ComponentModelDelegate(Kernel_ComponentModelCreated);
		}

		void Kernel_ComponentModelCreated(ComponentModel model)
		{
			if (typeof(IAutoStart).IsAssignableFrom(model.Implementation))
			{
				model.AddService(typeof(IAutoStart));
			}
		}

		public IWindsorContainer Container
		{
			get { return container; }
		}

		public override void Release(object instance)
		{
			container.Release(instance);
		}

		public override void AddComponentLifeStyle(string key, Type type, ComponentLifeStyle lifeStyle)
		{
			LifestyleType lifeStyleType = lifeStyle == ComponentLifeStyle.Singleton
			                              	? LifestyleType.Singleton
			                              	: LifestyleType.Transient;

			container.Register(Component.For(type).Named(key).LifeStyle.Is(lifeStyleType));
		}

		public override void AddComponentWithParameters(string key, Type serviceType, Type classType, IDictionary<string, string> properties)
		{
			logger.InfoFormat("Add component, key:{0}, service:{1}, implementation:{2}, properties:{3}" + key, serviceType, serviceType, classType, properties.Count);

			var registration = Component.For(serviceType)
				.ImplementedBy(classType);

			if (properties.Count > 0)
			{
				registration.DependsOn(properties.Select(p => Dependency.OnValue(p.Key, p.Value)));
			}

			container.Register(registration);
		}

		public override void AddComponentInstance(string key, Type serviceType, object instance)
		{
			logger.InfoFormat("Add component, key:{0}, service:{1}, instance:{2}" + key, serviceType, serviceType, instance);
			
			container.Register(Component.For(serviceType).Instance(instance).Named(key));
		}

		public override void AddComponent(string key, Type serviceType, Type classType)
		{
			logger.InfoFormat("Add component, key:{0}, service:{1}, implementation:{2}" + key, serviceType, serviceType, classType);

			var registration = Component.For(serviceType).ImplementedBy(classType).Named(key);
			var constructorParams = classType.GetConstructors().OrderByDescending(c => c.GetParameters().Length).Take(1).SelectMany(c => c.GetParameters()).ToArray();
			if (constructorParams.Any(p => p.ParameterType.IsArray))
			{
				foreach (var constructorParameter in constructorParams.Where(p => p.ParameterType.IsArray))
				{
					var elementType = constructorParameter.ParameterType.GetElementType();
					if (container.Kernel.GetHandler(constructorParameter.ParameterType.FullName) != null)
						continue;
					container.Register(Component.For(constructorParameter.ParameterType).Named(constructorParameter.ParameterType.FullName).UsingFactoryMethod(m => m.ResolveAll(elementType)));
				}
				registration = registration
					.UsingFactoryMethod(k => Activator.CreateInstance(classType, constructorParams.Select(cp => Resolve(cp.ParameterType)).ToArray()));
			}
			container.Register(registration);
		}

		public override object Resolve(string key)
		{
			return container.Resolve<object>(key, new Arguments());
		}

		public override object Resolve(Type type)
		{
			return container.Resolve(type);
		}

		/// <summary>Resolves all services serving the given interface.</summary>
		/// <param name="serviceType">The type of service to resolve.</param>
		/// <returns>All services registered to serve the provided interface.</returns>
		public override IEnumerable<object> ResolveAll(Type serviceType)
		{
			return container.ResolveAll(serviceType).Cast<object>();
		}

		/// <summary>Resolves all services.</summary>
		/// <returns>All registered services.</returns>
		public override IEnumerable<ServiceInfo> Diagnose()
		{
			return container.Kernel.GraphNodes.OfType<ComponentModel>()
				.Select(cm => new ServiceInfo { Key = cm.Name, ServiceTypes = cm.Services, ServiceType = cm.Services.FirstOrDefault(), ImplementationType = cm.Implementation, Resolve = () => Resolve(cm.Services.First()), ResolveAll = () => ResolveAll(cm.Services.First()) });
		}

		/// <summary>Resolves all services of the given type.</summary>
		/// <typeparam name="T">The type of service to resolve.</typeparam>
		/// <returns>All services registered to serve the provided interface.</returns>
		public override IEnumerable<T> ResolveAll<T>()
		{
			return container.ResolveAll<T>();
		}

		public override void StartComponents()
		{
			logger.Info("Start startable components");

			container.AddFacility<StartableFacility>();

			foreach (var startable in container.ResolveAll<IAutoStart>())
				startable.Start();

			container.Kernel.ComponentCreated += new ComponentInstanceDelegate(Kernel_ComponentCreated);
		}

		void Kernel_ComponentCreated(ComponentModel model, object instance)
		{
			if (instance is IAutoStart)
				(instance as IAutoStart).Start();
		}
	}
}