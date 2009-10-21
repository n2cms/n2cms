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
		private readonly IDictionary<string, string[]> _controllerActionMap = new Dictionary<string, string[]>();

		public ControllerMapper(ITypeFinder typeFinder, IDefinitionManager definitionManager)
		{
			IList<ControlsAttribute> controllerDefinitions = FindControllers(typeFinder);
			foreach (ItemDefinition id in definitionManager.GetDefinitions())
			{
				IAdapterDescriptor controllerDefinition = GetControllerFor(id.ItemType, controllerDefinitions);
				if(controllerDefinition != null)
				{
					ControllerMap[id.ItemType] = controllerDefinition.ControllerName;

					// interacting with static context is tricky, here I made the assumtion that the last
					// finder is the most relevat and takes the place of previous ones, this makes a few 
					// tests pass and doesn't seem to be called in production
					foreach (var finder in PathDictionary.GetFinders(id.ItemType).Where(f => f is ActionResolver))
						PathDictionary.RemoveFinder(id.ItemType, finder);

					// Use MVC's ReflectedControllerDescriptor to find all actions on the Controller
					var methods = new ReflectedControllerDescriptor(controllerDefinition.AdapterType)
						.GetCanonicalActions()
						.Select(m => m.ActionName).ToArray();
					var actionResolver = new ActionResolver(this, methods);

					_controllerActionMap[controllerDefinition.ControllerName] = methods;

					PathDictionary.PrependFinder(id.ItemType, actionResolver);
				}
			}
		}

		public string GetControllerName(Type type)
		{
			string name;
			ControllerMap.TryGetValue(type, out name);
			return name;
		}

		public bool ControllerHasAction(string controllerName, string actionName)
		{
			if(!_controllerActionMap.ContainsKey(controllerName))
				return false;

			foreach(var action in _controllerActionMap[controllerName])
			{
				if(String.Equals(action, actionName, StringComparison.InvariantCultureIgnoreCase))
					return true;
			}
			return false;
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