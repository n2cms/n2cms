using System;
using System.Reflection;
using System.Web.Mvc;
using N2.Engine;
using System.Linq;

namespace N2.Web.Mvc
{
	/// <summary>
	/// Used for the initialisation of the runtime for Mvc-specific N2 applications
	/// </summary>
	public static class MvcEngine
	{
		/// <summary>
		/// Creates an instance of the correct engine for the current environment (regular or medium trust) & registers additional components for the MVC functionality
		/// </summary>
		/// <returns></returns>
		public static IEngine Create()
		{
			IEngine engine = N2.Context.Initialize(false);

			return MvcInitialize(engine);
		}

		/// <summary>
		/// Creates an instance of the correct engine for the current environment (regular or medium trust) & registers additional components for the MVC functionality
		/// </summary>
		/// <returns></returns>
		public static IEngine Create(IServiceContainer container)
		{
			IEngine engine = new ContentEngine(container, EventBroker.Instance, new ContainerConfigurer());
			N2.Context.Replace(engine);
			return MvcInitialize(engine);
		}

		private static IEngine MvcInitialize(IEngine engine)
		{
			ControllerBuilder.Current.SetControllerFactory(engine.Resolve<IControllerFactory>());

			return engine;
		}
	}

	/// <summary>
	/// Extension methods for the Mvc functionality
	/// </summary>
	public static class MvcEngineExtensions
	{
		/// <summary>
		/// Registers all controllers in the given assembly with the Engine, so that they may be resolved correctly.
		/// </summary>
		/// <param name="engine"></param>
		/// <param name="assembly"></param>
		public static void RegisterControllers(this IEngine engine, Assembly assembly)
		{
			foreach (Type type in assembly.GetExportedTypes())
				if (IsController(type))
					engine.Container.AddComponentLifeStyle(type.FullName.ToLower(), type, ComponentLifeStyle.Transient);
		}

		/// <summary>
		/// Registers all controllers in assemblies normally considered for content definition with the Engine, so that they may be resolved correctly.
		/// </summary>
		/// <param name="engine"></param>
		public static void RegisterAllControllers(this IEngine engine)
		{
			foreach (Type type in engine.Resolve<ITypeFinder>().Find(typeof(IController)).Where(t => !t.IsAbstract).Where(t => !t.IsInterface))
				engine.Container.AddComponentLifeStyle(type.FullName.ToLower(), type, ComponentLifeStyle.Transient);
		}

		private static bool IsController(Type type)
		{
			return type != null
			       && !type.IsAbstract
			       && typeof (IController).IsAssignableFrom(type);
		}
	}
}