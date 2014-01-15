using System;
using System.Web.Mvc;
using N2.Engine;

namespace N2.Web.Mvc
{
    [Obsolete("Type moved to ControllerFactoryConfigurator.ControllerFactory")]
    [Service(typeof(IControllerFactory))]
    public class N2ControllerFactory : ServiceLocatingControllerFactory
    {
        /// <summary>
        /// Creates a new instance of the <see cref="N2ControllerFactory"/> class.
        /// </summary>
        /// <param name="engine">The N2 engine instance to use when creating controllers.</param>
        public N2ControllerFactory(IEngine engine)
            : base(engine)
        {
        }
    }
}
