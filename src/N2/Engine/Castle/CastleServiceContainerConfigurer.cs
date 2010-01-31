using System.Configuration;
using Castle.Core.Resource;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Windsor.Installer;
using N2.Configuration;
using N2.Engine;
using N2.Engine.Configuration;

namespace N2.Engine.Castle
{
	public class CastleServiceContainerConfigurer : IServiceContainerConfigurer
	{
		private readonly IWindsorContainer container;

		public CastleServiceContainerConfigurer(IWindsorContainer container)
		{
			this.container = container;
		}

		public void Configure(IEngine engine, EngineSection engineConfig)
		{
			engine.Container.AddComponentInstance("n2.engine", typeof(IEngine), engine);

			IResource resource = DetermineResource(engineConfig, ConfigurationManager.GetSection("castle") != null);
			ProcessResource(resource);
			InstallComponents();
		}

		/// <summary>Either reads the castle configuration from the castle configuration section or uses a default configuration compiled into the n2 assembly.</summary>
		/// <param name="engineConfig">The n2 engine section from the configuration file.</param>
		/// <param name="hasCastleSection">Load the container using default castle configuration.</param>
		/// <returns>A castle IResource used to build the inversion of control container.</returns>
		private static IResource DetermineResource(EngineSection engineConfig, bool hasCastleSection)
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
		private void ProcessResource(IResource resource)
		{
			var interpreter = new XmlInterpreter(resource);
			interpreter.ProcessResource(resource, container.Kernel.ConfigurationStore);
		}

		/// <summary>Sets up components in the inversion of control container.</summary>
		private void InstallComponents()
		{
			var installer = new DefaultComponentInstaller();
			installer.SetUp(container, container.Kernel.ConfigurationStore);
		}
	}
}