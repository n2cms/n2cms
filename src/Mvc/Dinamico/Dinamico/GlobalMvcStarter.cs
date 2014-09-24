using System.Web.Mvc;
using System.Web.Routing;
using N2.Definitions.Runtime;
using N2.Engine;
using N2.Plugin;
using N2.Web.Mvc;

namespace Dinamico
{
    /// <summary>
    /// This initialization code is placed in a separate file not to overwrite existing global config when adding dinamico as library package (nuget)
    /// </summary>
    [AutoInitialize]
    public class GlobalMvcStarter : IPluginInitializer
    {
        #region IPluginInitializer Members

        public void Initialize(N2.Engine.IEngine engine)
        {
            //log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(Server.MapPath("~/N2/Installation/log4net.config")));

            RegisterControllerFactory(ControllerBuilder.Current, engine);
            RegisterRoutes(RouteTable.Routes, engine);
            RegisterViewEngines(ViewEngines.Engines);
            RegisterViewTemplates(engine);
        }

        #endregion


        private void RegisterViewTemplates(IEngine engine)
        {
			var reg = engine.RegisterViewTemplates<Controllers.ContentPagesController>();
            TryRegisterViewTemplate<Controllers.ContentPartsController>(reg);
            TryRegisterViewTemplate<Controllers.SlideshowController>(reg);
        }

	    private void TryRegisterViewTemplate<T>(ViewTemplateRegistrator registrator) where T : IController
	    {
		    try
		    {
				registrator.Add<T>();
		    }
		    catch (System.Exception ex)
		    {
			    Logger.Error(ex);
		    }
	    }

        public static void RegisterControllerFactory(ControllerBuilder controllerBuilder, IEngine engine)
        {
            // Registers controllers in the solution for dependency injection using the IoC container provided by N2
            engine.RegisterAllControllers();

            var controllerFactory = engine.Resolve<ControllerFactoryConfigurator>()
                .NotFound<Controllers.StartPageController>(sc => sc.NotFound())
                .ControllerFactory;

            controllerBuilder.SetControllerFactory(controllerFactory);
        }

        public static void RegisterRoutes(RouteCollection routes, IEngine engine)
        {
            routes.MapContentRoute("Content", engine);
        }

        public static void RegisterViewEngines(ViewEngineCollection viewEngines)
        {
            viewEngines.RegisterThemeViewEngine<RazorViewEngine>("~/Dinamico/Themes/");
            viewEngines.DecorateViewTemplateRegistration();
        }
    }
}
