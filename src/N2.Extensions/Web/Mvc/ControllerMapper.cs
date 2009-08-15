using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using N2.Definitions;
using N2.Engine;

namespace N2.Web.Mvc
{
	public class ControllerMapper : IControllerMapper
	{
		private readonly IDictionary<Type, string> _controllerMap = new Dictionary<Type, string>();

		public ControllerMapper(ITypeFinder typeFinder, IDefinitionManager definitionManager)
		{
			IList<ControlsAttribute> controllerDefinitions = FindControllers(typeFinder);
			foreach (ItemDefinition id in definitionManager.GetDefinitions())
			{
				IAdapterDescriptor controllerDefinition = GetControllerFor(id.ItemType, controllerDefinitions);
				if(controllerDefinition != null)
				{
					ControllerMap[id.ItemType] = controllerDefinition.ControllerName;
					IList<IPathFinder> finders = PathDictionary.GetFinders(id.ItemType);
					if (0 == finders.Where(f => f is ActionResolver).Count())
					{
						// TODO: Get the list of methods from a list of actions retrieved from somewhere within MVC
						var methods = new ReflectedControllerDescriptor(controllerDefinition.AdapterType)
							.GetCanonicalActions()
							.Select(m => m.ActionName).ToArray();
						var actionResolver = new ActionResolver(this, methods);
						PathDictionary.PrependFinder(id.ItemType, actionResolver);
					}
				}
			}
		}

		public string GetControllerName(Type type)
		{
			string name;
			ControllerMap.TryGetValue(type, out name);
			return name;
		}

		private IDictionary<Type, string> ControllerMap
		{
			get { return _controllerMap; }
		}

		private static IAdapterDescriptor GetControllerFor(Type itemType, IList<ControlsAttribute> controllerDefinitions)
		{
			foreach (ControlsAttribute controllerDefinition in controllerDefinitions)
			{
				if (controllerDefinition.ItemType.IsAssignableFrom(itemType))
				{
					return controllerDefinition;
				}
			}
			return null;
		}

		private static IList<ControlsAttribute> FindControllers(ITypeFinder typeFinder)
		{
			var controllerDefinitions = new List<ControlsAttribute>();
			foreach (Type controllerType in typeFinder.Find(typeof(IController)))
			{
				foreach (ControlsAttribute attr in controllerType.GetCustomAttributes(typeof(ControlsAttribute), false))
				{
					attr.AdapterType = controllerType;
					controllerDefinitions.Add(attr);
				}
			}
			controllerDefinitions.Sort();
			return controllerDefinitions;
		}
	}
}