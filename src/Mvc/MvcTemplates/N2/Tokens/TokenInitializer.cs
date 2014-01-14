using System.Web.Mvc;
using N2.Engine;
using N2.Plugin;
using N2.Web.Mvc;

namespace N2.Management.Tokens
{
    [AutoInitialize]
    public class TokenInitializer : IPluginInitializer
    {
        public void Initialize(Engine.IEngine engine)
        {
            if (engine.Config.Sections.Web.Tokens.BuiltInEnabled)
            {
                var viewEngines = engine.Resolve<IProvider<ViewEngineCollection>>().Get();
                viewEngines.RegisterTokenViewEngine();
            }
        }
    }
}
