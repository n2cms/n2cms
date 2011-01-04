using System;
using System.Collections.Generic;
using System.Text;
using N2.Plugin;
using N2.Web;

namespace N2.Addons.Wiki
{
    [AutoInitialize]
    public class WikiInitializer : IPluginInitializer
    {
        #region IPluginInitializer Members

        public void Initialize(N2.Engine.IEngine engine)
        {
            engine.AddComponent("n2.wiki.parser", typeof(WikiParser));
            engine.AddComponent("n2.wiki.renderer", typeof(WikiRenderer));
        }

        #endregion
    }
}
