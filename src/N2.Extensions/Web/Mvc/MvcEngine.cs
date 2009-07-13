using System;
using System.Reflection;
using System.Web.Mvc;
using Castle.Core;
using N2.Engine;
using N2.Web.Mvc.Html;

namespace N2.Web.Mvc
{
	public class MvcEngine : ContentEngine
	{
		public static MvcEngine Create()
		{
			var engine = new MvcEngine();

			N2.Context.Initialize(engine);

			engine.Initialize();

			RegisterAdditionalComponents(engine);

			ViewEngines.Engines.Clear();
			ViewEngines.Engines.Add(new N2ViewEngine());

			ControllerBuilder.Current.SetControllerFactory(new N2ControllerFactory(engine));

			return engine;
		}

		private static void RegisterAdditionalComponents(IEngine engine)
		{
			engine.AddComponent("n2.mvc.controllerMapper", typeof (IControllerMapper), typeof (ControllerMapper));
			engine.AddComponent("n2.mvc.templateRenderer", typeof (ITemplateRenderer), typeof (TemplateRenderer));
		}

		/// <summary>Registers a component in the IoC container.</summary>
		/// <param name="key">A unique key.</param>
		/// <param name="classType">The type of component to register.</param>
		public override void AddComponent(string key, Type classType)
		{
			LifestyleType lifeStyle = LifestyleType.Singleton;

			if (IsController(classType))
				lifeStyle = LifestyleType.Transient;

			Container.AddComponentLifeStyle(key, classType, lifeStyle);
		}

		/// <summary>Registers a component in the IoC container.</summary>
		/// <param name="key">A unique key.</param>
		/// <param name="serviceType">The type of service to provide.</param>
		/// <param name="classType">The type of component to register.</param>
		public override void AddComponent(string key, Type serviceType, Type classType)
		{
			LifestyleType lifeStyle = LifestyleType.Singleton;

			if (IsController(classType))
				lifeStyle = LifestyleType.Transient;

			Container.AddComponentLifeStyle(key, serviceType, classType, lifeStyle);
		}

		public void RegisterControllers(Assembly assembly)
		{
			foreach (Type type in assembly.GetExportedTypes())
				if (IsController(type))
					AddComponent(type.FullName.ToLower(), type);
		}

		private bool IsController(Type type)
		{
			return type != null
			       && !type.IsAbstract
			       && typeof (IController).IsAssignableFrom(type);
		}
	}
}