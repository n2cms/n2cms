using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using N2.Engine;

namespace N2.Web.Mvc
{
    /// <summary>
    /// Configures a controller factory used by ASP.NET MVC applications.
    /// </summary>
    [Service]
    public class ControllerFactoryConfigurator
    {
        ServiceLocatingControllerFactory controllerFactory;

        /// <summary>Creates an instance.</summary>
        /// <param name="controllerFactory">The controller factory used.</param>
        public ControllerFactoryConfigurator(ServiceLocatingControllerFactory controllerFactory)
        {
            this.controllerFactory = controllerFactory;
        }

        /// <summary>Sets the controller used when no controller is found.</summary>
        /// <param name="notFoundControllerSelector">A type selector methosd.</param>
        /// <returns>The same object for fluent registration.</returns>
        public virtual ControllerFactoryConfigurator NotFound<T>(Expression<Func<T, ActionResult>> expression) where T : IController
        {
            controllerFactory.NotFound(expression);
            return this;
        }

        /// <summary>The controller factory instance.</summary>
        public virtual IControllerFactory ControllerFactory { get { return controllerFactory; } }
    }
}
