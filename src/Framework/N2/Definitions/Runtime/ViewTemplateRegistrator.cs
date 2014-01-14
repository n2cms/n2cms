using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using N2.Engine;
using N2.Web.Mvc;

namespace N2.Definitions.Runtime
{
    public static class ViewTemplateRegistratorExtensions
    {
        /// <summary>Analyzes views in the controllers's view folder looking for dynamic template registrations.</summary>
        /// <typeparam name="T">The type of controller to analyze.</typeparam>
        /// <param name="engine">The engine from which to resolve the <see cref="ViewTemplateRegistrator"/>.</param>
        /// <returns>The singleton <see cref="ViewTemplateRegistrator"/> instance.</returns>
        public static ViewTemplateRegistrator RegisterViewTemplates<T>(this IEngine engine) where T : IController
        {
            return engine.Resolve<ViewTemplateRegistrator>().Add<T>();
        }

        /// <summary>Analyzes views in the controllers's view folder looking for dynamic template registrations.</summary>
        /// <typeparam name="T">The type of controller to analyze.</typeparam>
        /// <param name="engine">The engine from which to resolve the <see cref="ViewTemplateRegistrator"/>.</param>
        /// <param name="viewFileExtension">The type of view file to analyze. By the default .cshtml files are analyzed.</param>
        /// <returns>The singleton <see cref="ViewTemplateRegistrator"/> instance.</returns>
        public static ViewTemplateRegistrator RegisterViewTemplates<T>(this IEngine engine, string viewFileExtension) where T : IController
        {
            return engine.Resolve<ViewTemplateRegistrator>().Add<T>(viewFileExtension);
        }
    }

    /// <summary>
    /// Conveys information about controllers registered for view template discovery.
    /// </summary>
    [Service]
    public class ViewTemplateRegistrator
    {
        public ViewTemplateRegistrator()
        {
            QueuedRegistrations = new Queue<ViewTemplateSource>();
        }

        Engine.Logger<ViewTemplateRegistrator> logger;

        public event EventHandler RegistrationAdded;

        public Queue<ViewTemplateSource> QueuedRegistrations { get; set; }

        /// <summary>Analyzes views in the controllers's view folder looking for dynamic template registrations.</summary>
        /// <typeparam name="T">The type of controller to analyze.</typeparam>
        /// <param name="engine">The engine from which to resolve the <see cref="ViewTemplateRegistrator"/>.</param>
        /// <returns>The singleton <see cref="ViewTemplateRegistrator"/> instance.</returns>
        public ViewTemplateRegistrator Add<T>() where T : IController
        {
            return Add<T>(".cshtml");
        }

        /// <summary>Analyzes views in the controllers's view folder looking for dynamic template registrations.</summary>
        /// <typeparam name="T">The type of controller to analyze.</typeparam>
        /// <param name="engine">The engine from which to resolve the <see cref="ViewTemplateRegistrator"/>.</param>
        /// <param name="viewFileExtension">The type of view file to analyze. By the default .cshtml files are analyzed.</param>
        /// <returns>The singleton <see cref="ViewTemplateRegistrator"/> instance.</returns>
        public ViewTemplateRegistrator Add<T>(string viewFileExtension) where T : IController
        {
            var controllerType = typeof(T);
            string controllerName = controllerType.Name.Substring(0, controllerType.Name.Length - "Controller".Length);
            Type modelType = typeof(ContentItem);
            Type contentControllerType = Utility.GetBaseTypes(controllerType).FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ContentController<>));
            if (contentControllerType != null)
                modelType = contentControllerType.GetGenericArguments().First();
            var source = new ViewTemplateSource { ControllerName = controllerName, ModelType = modelType, ViewFileExtension = viewFileExtension };

            logger.DebugFormat("Enqueuing view template for controller {0} with model {1} looking for extension {2}", controllerName, modelType, viewFileExtension);

            QueuedRegistrations.Enqueue(source);

            if (RegistrationAdded != null)
                RegistrationAdded.Invoke(this, new EventArgs());

            return this;
        }
    }
}
