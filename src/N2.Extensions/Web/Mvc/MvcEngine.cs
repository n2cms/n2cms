using System;
using System.Reflection;
using System.Security;
using System.Web.Mvc;
using N2.Engine;
using N2.Engine.MediumTrust;
using N2.Web.Mvc.Html;

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
			IEngine engine;
			try
			{
				engine = new ContentEngine();
			}
			catch (SecurityException)
			{
				engine = new MediumTrustEngine();
			}

			return Initialize(engine);
		}

		/// <summary>
		/// Creates an instance of the correct engine for the current environment (regular or medium trust) & registers additional components for the MVC functionality
		/// </summary>
		/// <returns></returns>
		public static IEngine Create(IServiceContainer container)
		{
			IEngine engine = new ContentEngine(container);

			return Initialize(engine);
		}

		private static IEngine Initialize(IEngine engine)
		{
			Context.Initialize(engine);

			engine.Initialize();

			RegisterAdditionalComponents(engine);

			ControllerBuilder.Current.SetControllerFactory(engine.Resolve<IControllerFactory>());

			return engine;
		}

		private static void RegisterAdditionalComponents(IEngine engine)
		{
			engine.AddComponent("n2.mvc.controllerFactory", typeof(IControllerFactory), typeof(N2ControllerFactory));
			engine.AddComponent("n2.mvc.controllerMapper", typeof(IControllerMapper), typeof(ControllerMapper));
			engine.AddComponent("n2.mvc.templateRenderer", typeof (ITemplateRenderer), typeof (TemplateRenderer));
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
					engine.AddComponentLifeStyle(type.FullName.ToLower(), type, ComponentLifeStyle.Transient);
		}

		private static bool IsController(Type type)
		{
			return type != null
			       && !type.IsAbstract
			       && typeof (IController).IsAssignableFrom(type);
		}
	}
}