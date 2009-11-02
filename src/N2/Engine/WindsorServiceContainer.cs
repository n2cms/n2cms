using System;
using System.Configuration;
using Castle.Core;
using Castle.Core.Resource;
using Castle.Facilities.Startable;
using Castle.MicroKernel;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Windsor.Installer;
using N2.Configuration;
using N2.Plugin;

namespace N2.Engine
{
	public class WindsorServiceContainer : ServiceContainerBase
	{
		private readonly IWindsorContainer container;

		/// <summary>
		/// Initializes a new instance of the <see cref="WindsorServiceContainer"/> class using an empty Windsor container.
		/// </summary>
		public WindsorServiceContainer() : this(new WindsorContainer())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WindsorServiceContainer"/> class using an existing container.  Any additional/replacement service defined by the user will be added to the container.
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

			container.AddComponentLifeStyle(key, type, lifeStyleType);
		}

		public override void AddComponentInstance(string key, Type type, object instance)
		{
			container.Kernel.AddComponentInstance(key, type, instance);
		}

		public override void AddComponent(string key, Type type, Type classType)
		{
			container.AddComponent(key, type, classType);
		}

		public override object Resolve(string key)
		{
			return container.Resolve(key);
		}

		public override object Resolve(Type type)
		{
			return container.Resolve(type);
		}

		public override void AddFacility(string key, object facility)
		{
			var castleFacility = facility as IFacility;

			if (castleFacility != null)
				container.Kernel.AddFacility(key, castleFacility);
			else
				throw new ArgumentException("Only classes implementing Castle.MicroKernel.IFacilty are supported.");
		}

		public override void StartComponents()
		{
			StartComponents(container.Kernel);
			container.Kernel.ComponentCreated += KernelComponentCreated;
		}

		public override void Configure(IEngine engine, EngineSection engineConfig)
		{
			AddComponentInstance("n2.engine", typeof(IEngine), engine);

			IResource resource = DetermineResource(engineConfig, ConfigurationManager.GetSection("castle") != null);
			ProcessResource(resource);
			InstallComponents();
		}

		private static void KernelComponentCreated(ComponentModel model, object instance)
		{
			if (instance is IStartable)
				return;

			var startable = instance as IAutoStart;
			if (startable != null)
			{
				startable.Start();
			}
		}

		private void StartComponents(IKernel kernel)
		{
			var naming = (INamingSubSystem) kernel.GetSubSystem(SubSystemConstants.NamingKey);
			foreach (GraphNode node in kernel.GraphNodes)
			{
				var model = node as ComponentModel;
				if (model != null)
				{
					if (typeof (IStartable).IsAssignableFrom(model.Implementation) ||
					    typeof (IAutoStart).IsAssignableFrom(model.Implementation))
					{
						IHandler h = naming.GetHandler(model.Name);
						if (h.CurrentState == HandlerState.Valid)
						{
							object component = kernel[model.Name];
							if (component is IStartable)
								(component as IStartable).Start();
							else if (component is IAutoStart)
								(component as IAutoStart).Start();
						}
					}
				}
			}
			container.AddFacility<StartableFacility>();
		}

		/// <summary>Either reads the castle configuration from the castle configuration section or uses a default configuration compiled into the n2 assembly.</summary>
		/// <param name="engineConfig">The n2 engine section from the configuration file.</param>
		/// <param name="hasCastleSection">Load the container using default castle configuration.</param>
		/// <returns>A castle IResource used to build the inversion of control container.</returns>
		protected IResource DetermineResource(EngineSection engineConfig, bool hasCastleSection)
		{
			if (engineConfig != null)
			{
				if (!string.IsNullOrEmpty(engineConfig.CastleSection))
					return new ConfigResource(engineConfig.CastleSection);
				return new AssemblyResource(engineConfig.CastleConfiguration);
			}

			if (!hasCastleSection)
				throw new ConfigurationErrorsException(
					"Couldn't find a suitable configuration section for N2 CMS. Either add an n2/engine or a castle configuartion section to web.config. Note that this section may have changed from previous versions. Please verify that the configuartion is properly updated.");

			return new ConfigResource();
		}

		/// <summary>Processes the castle resource and build the castle configuration store.</summary>
		/// <param name="resource">The resource to use. This may be derived from the castle section in web.config or a default xml compiled into the assembly.</param>
		protected void ProcessResource(IResource resource)
		{
			var interpreter = new XmlInterpreter(resource);
			interpreter.ProcessResource(resource, container.Kernel.ConfigurationStore);
		}

		/// <summary>Sets up components in the inversion of control container.</summary>
		protected void InstallComponents()
		{
			var installer = new DefaultComponentInstaller();
			installer.SetUp(container, container.Kernel.ConfigurationStore);
		}
	}
}