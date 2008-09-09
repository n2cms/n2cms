using N2.Engine;
using N2.Plugin;

namespace N2.Templates.Services
{
    [AutoInitialize]
    public class SEOInitializer : IPluginInitializer
    {
        public void Initialize(IEngine engine)
        {
            engine.AddComponent("n2.templates.seo.definitions", typeof(SEODefinitionAppender));
            engine.AddComponent("n2.templates.seo.modifier", typeof(SEOPageModifier));
        }
    }
}