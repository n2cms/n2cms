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
				ControllerMap[id.ItemType] = controllerDefinition.ControllerName;
				IList<IPathFinder> finders = PathDictionary.GetFinders(id.ItemType);
				if (0 == finders.Where(f => f is ActionResolver).Count())
				{
					var methods = controllerDefinition.AdapterType.GetMethods().Select(m => m.Name).ToArray();
					var actionResolver = new ActionResolver(methods);
					PathDictionary.PrependFinder(id.ItemType, actionResolver);
				}
			}
		}

		public string GetControllerName(Type type)
		{
			return ControllerMap[type];
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
			throw new N2Exception("Found no controller for type '" + itemType + "' among " + controllerDefinitions.Count + " found controllers.");
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