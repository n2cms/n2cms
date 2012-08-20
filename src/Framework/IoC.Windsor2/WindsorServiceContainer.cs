using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Castle.Core;
using Castle.Core.Internal;
using Castle.Facilities.Startable;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Naming;
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
			logger.Info("Add component " + classType);

			var registration = Component.For(serviceType)
				.ImplementedBy(classType);

			foreach(var keyValue in properties)
			{
				registration.Parameters(Parameter.ForKey(keyValue.Key).Eq(keyValue.Value));
			}

			container.Register(registration);
		}

		public override void AddComponentInstance(string key, Type serviceType, object instance)
		{
			logger.Info("Add component instance " + instance);
			
			container.Register(Component.For(serviceType).Instance(instance).Named(key));
		}

		public override void AddComponent(string key, Type serviceType, Type classType)
		{
			logger.Info("Add component " + classType);

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
			return container.Resolve(key, new Arguments());
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
				.Select(cm => new ServiceInfo { Key = cm.Name, ServiceTypes = new [] { cm.Service }, ServiceType = cm.Service, ImplementationType = cm.Implementation, Resolve = () => Resolve(cm.Service), ResolveAll = () => ResolveAll(cm.Service) });
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
			container.AddFacility<AutoStartFacility>();
		}

		/// <summary>
		/// This class is derived from the Castle Startable facility, with appropriate modifications
		/// </summary>
		class AutoStartFacility : AbstractFacility
		{
			private readonly Engine.Logger<AutoStartFacility> logger;
			private readonly ArrayList waitingList = new ArrayList();

			// Don't check the waiting list while this flag is set as this could result in
			// duplicate singletons.
			private bool inStart;

			protected override void Init()
			{
				Kernel.ComponentModelCreated += OnComponentModelCreated;
				Kernel.ComponentRegistered += OnComponentRegistered;

				StartComponents();
			}

			private void StartComponents()
			{
				var naming = (INamingSubSystem)Kernel.GetSubSystem(SubSystemConstants.NamingKey);
				logger.Debug("Starting components: ");
				int started = 0;
				foreach (GraphNode node in Kernel.GraphNodes)
				{
					var model = node as ComponentModel;
					if (model == null)
						continue;
					if (!typeof(IAutoStart).IsAssignableFrom(model.Implementation))
						continue;

					IHandler h = naming.GetHandler(model.Name);
					if (h.CurrentState != HandlerState.Valid)
						continue;

					var instance = Kernel.Resolve(model.Name, new Arguments()) as IAutoStart;
					logger.Info("Started " + instance);
					instance.Start();
					started++;
				}
				logger.DebugFormat("Started {0} of {1} components", started, Kernel.GraphNodes.Length);
			}

			private void OnComponentRegistered(String key, IHandler handler)
			{
				bool startable = (bool?)handler.ComponentModel.ExtendedProperties["startable"] ?? false;

				if (startable)
				{
					if (handler.CurrentState == HandlerState.WaitingDependency)
					{
						AddHandlerToWaitingList(handler);
					}
					else
					{
						Start(key);
					}
				}

				CheckWaitingList();
			}

			private void OnComponentModelCreated(ComponentModel model)
			{
				bool startable = CheckIfComponentImplementsIStartable(model);

				logger.DebugFormat("Created component ot type {0}, startable = {1}", model.Service, startable);

				model.ExtendedProperties["startable"] = startable;

				if (!startable)
					return;

				model.Lifecycle.Add(StartConcern.Instance);
				model.Lifecycle.AddFirst(StopConcern.Instance);
			}

			private void OnHandlerStateChanged(object source, EventArgs args)
			{
				CheckWaitingList();
			}

			private void AddHandlerToWaitingList(IHandler handler)
			{
				waitingList.Add(handler);

				handler.OnHandlerStateChanged += OnHandlerStateChanged;
			}

			/// <summary>
			/// For each new component registered,
			/// some components in the WaitingDependency
			/// state may have became valid, so we check them
			/// </summary>
			private void CheckWaitingList()
			{
				if (inStart)
					return;

				var handlers = (IHandler[])waitingList.ToArray(typeof(IHandler));

				IList validList = new ArrayList();

				foreach (IHandler handler in handlers)
				{
					if (handler.CurrentState != HandlerState.Valid)
						continue;

					validList.Add(handler);
					waitingList.Remove(handler);

					handler.OnHandlerStateChanged -= OnHandlerStateChanged;
				}

				foreach (IHandler handler in validList)
				{
					Start(handler.ComponentModel.Name);
				}
			}

			/// <summary>
			/// Request the component instance
			/// </summary>
			/// <param name="key"></param>
			private void Start(String key)
			{
				try
				{
					inStart = true;

					object instance = Kernel.Resolve(key, new Arguments());
				}
				finally
				{
					inStart = false;
				}
			}

			private static bool CheckIfComponentImplementsIStartable(ComponentModel model)
			{
				return typeof(IAutoStart).IsAssignableFrom(model.Implementation);
			}


			#region Start & Stop Concern
			class StartConcern : ILifecycleConcern, ICommissionConcern
			{
				private readonly Engine.Logger<StartConcern> logger;
				private static readonly StartConcern instance = new StartConcern();

				private StartConcern()
				{
				}

				public static StartConcern Instance
				{
					get { return instance; }
				}

				public void Apply(ComponentModel model, object component)
				{
					var autoStart = component as IAutoStart;
					if (autoStart != null && !(component is IStartable))
					{
						logger.Info("Starting " + component);
						autoStart.Start();
					}
				}
			}

			class StopConcern : ILifecycleConcern, IDecommissionConcern
			{
				private static readonly StopConcern instance = new StopConcern();

				private StopConcern()
				{
				}

				public static StopConcern Instance
				{
					get { return instance; }
				}

				public void Apply(ComponentModel model, object component)
				{
					var autoStart = component as IAutoStart;
					if (autoStart != null)
					{
						autoStart.Stop();
					}
				}
			}
			#endregion
		}
	}
}