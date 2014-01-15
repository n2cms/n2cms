using System;

namespace N2.Web.Mvc
{
    /// <summary>
    /// Maps content types to controllers.
    /// </summary>
    public interface IControllerMapper
    {
        /// <summary>Gets the controller associated with a given content type.</summary>
        /// <param name="contentType">The type of content item whose controller to get.</param>
        /// <returns>A controller name if a controller was found.</returns>
        string GetControllerName(Type contentType);

        /// <summary>Returns true if the given controller has the given action.</summary>
        /// <param name="controllerName">The controller to check.</param>
        /// <param name="actionName">The action to verify.</param>
        /// <returns>True if the controller has the action.</returns>
        bool ControllerHasAction(string controllerName, string actionName);

        /// <summary>Returns true if the given controller is a controller handling content items.</summary>
        /// <param name="controllerName">The controller to check.</param>
        /// <returns>True if it is a content controller (has [Controls] attribute), otherwise false.</returns>
        bool IsContentController(string controllerName);
    }
}
