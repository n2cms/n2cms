using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Castle.Core;
using Castle.Facilities.Startable;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.LifecycleConcerns;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using N2.Plugin;
using Castle.MicroKernel.SubSystems.Naming;
using Castle.Core.Internal;

namespace N2.Engine.Castle
{
	/// <summary>
	/// Wraps usage of the Castle Windsor inversion of control container.
	/// </summary>
	public class WindsorServiceContainer : ServiceContainerBase
	{
		private readonly IWindsorContainer container;

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
			container.Register(Component.For(serviceType).Instance(instance).Named(key));
		}

		public override void AddComponent(string key, Type serviceType, Type classType)
		{
			container.Register(Component.For(serviceType).ImplementedBy(classType).Named(key));
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
		public override Array ResolveAll(Type serviceType)
		{
			return container.ResolveAll(serviceType);
		}

		/// <summary>Resolves all services of the given type.</summary>
		/// <typeparam name="T">The type of service to resolve.</typeparam>
		/// <returns>All services registered to serve the provided interface.</returns>
		public override T[] ResolveAll<T>()
		{
			return container.ResolveAll<T>();
		}

		public override void StartComponents()
		{
			container.AddFacility<StartableFacility>();
			container.AddFacility<AutoStartFacility>();
		}

		/// <summary>
		/// This class is derived from the Castle Startable facility, with appropriate modifications
		/// </summary>
		class AutoStartFacility : AbstractFacility
		{
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
				Trace.Write("Starting components: ");
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
					Trace.Write(instance + ", ");
					instance.Start();
				}
				Trace.WriteLine(" out of " + Kernel.GraphNodes.Length);
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

			class StartConcern : ILifecycleConcern, ICommissionConcern
			{
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
						Trace.WriteLine("Starting " + component);
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
		}
	}
}