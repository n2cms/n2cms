using System.Web.Mvc;
using N2.Web.Mvc;
using N2.Plugin;
using System.Web.Routing;
using N2.Engine;
using N2.Web;
using N2.Definitions;

namespace N2.Management.Myself.Analytics
{
    [AutoInitialize]
    public class ManagementAreaRegistration : IPluginInitializer
    {
        public virtual void RegisterArea(RouteCollection routes, ViewEngineCollection viewEngines, IEngine engine)
        {
            var route = new ContentRoute<IManagementHomePart>(engine);
            routes.Insert(0, route);

            var viewLocationFormats = new[] { Url.ResolveTokens("{ManagementUrl}/Myself/Analytics/Views/{1}/{0}.ascx"), Url.ResolveTokens("{ManagementUrl}/Myself/Analytics/Views/Shared/{0}.ascx") };
            viewEngines.Insert(0, new PrivateViewEngineDecorator(new WebFormViewEngine { AreaViewLocationFormats = viewLocationFormats, PartialViewLocationFormats = viewLocationFormats }, route));
        }

        #region IPluginInitializer Members

        public void Initialize(Engine.IEngine engine)
        {
            var routes = engine.Resolve<IProvider<RouteCollection>>().Get();
            var viewEngines = engine.Resolve<IProvider<ViewEngineCollection>>().Get();
            RegisterArea(routes, viewEngines, engine);
        }

        #endregion
    }
}
