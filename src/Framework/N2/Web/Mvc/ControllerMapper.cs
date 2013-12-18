using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using N2.Definitions;
using N2.Engine;

namespace N2.Web.Mvc
{
    /// <summary>
    /// Maps content types to controllers.
    /// </summary>
    [Service(typeof(IControllerMapper))]
    public class ControllerMapper : IControllerMapper
    {
        private readonly IDictionary<Type, string> _controllerMap = new Dictionary<Type, string>();
        private readonly IDictionary<string, string[]> _controllerActionMap = new Dictionary<string, string[]>(StringComparer.InvariantCultureIgnoreCase);

        public ControllerMapper(ITypeFinder typeFinder, IDefinitionManager definitionManager)
        {
            IList<ControlsAttribute> controllerDefinitions = FindControllers(typeFinder);
            foreach (ItemDefinition id in definitionManager.GetDefinitions())
            {
                IAdapterDescriptor controllerDefinition = GetControllerFor(id, controllerDefinitions);
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

        /// <summary>Gets the controller associated with a given content type.</summary>
        /// <param name="contentType">The type of content item whose controller to get.</param>
        /// <returns>A controller name if a controller was found.</returns>
        public string GetControllerName(Type type)
        {
            string name;
            ControllerMap.TryGetValue(type, out name);
            return name;
        }

        /// <summary>Returns true if the given controller has the given action.</summary>
        /// <param name="controllerName">The controller to check.</param>
        /// <param name="actionName">The action to verify.</param>
        /// <returns>True if the controller has the action.</returns>
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

        private static IAdapterDescriptor GetControllerFor(ItemDefinition definition, IList<ControlsAttribute> controllerDefinitions)
        {
            if (definition.Metadata.ContainsKey("ControlledBy"))
                return new ControlsAttribute(definition.ItemType) { AdapterType = (Type)definition.Metadata["ControlledBy"] };

            Type itemType = definition.ItemType;
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
            var controllerLookup = new Dictionary<Type, ControlsAttribute>();
            foreach (Type controllerType in typeFinder.Find(typeof(IController)))
            {
                foreach (ControlsAttribute attr in controllerType.GetCustomAttributes(typeof(ControlsAttribute), false))
                {
                    if (controllerLookup.ContainsKey(attr.ItemType))
                        throw new N2Exception("Duplicate controller " + controllerType.Name + " declared for item type " +
                                              attr.ItemType.Name +
                                              " The controller " + controllerLookup[attr.ItemType].AdapterType.Name +
                                              " already handles this type and two controllers cannot handle the same item type.");

                    attr.AdapterType = controllerType;
                    controllerLookup.Add(attr.ItemType, attr);
                }
            }

            var controllerDefinitions = new List<ControlsAttribute>(controllerLookup.Values);
            controllerDefinitions.Sort();

            return controllerDefinitions;
        }

        /// <summary>Returns true if the given controller is a controller handling content items.</summary>
        /// <param name="controllerName">The controller to check.</param>
        /// <returns>True if it is a content controller (has [Controls] attribute), otherwise false.</returns>
        public bool IsContentController(string controllerName)
        {
            return _controllerActionMap.ContainsKey(controllerName);
        }
    }
}
