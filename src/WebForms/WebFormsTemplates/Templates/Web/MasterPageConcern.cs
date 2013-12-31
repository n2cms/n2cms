using System.IO;
using System.Linq;
using System.Configuration;
using N2.Templates.Configuration;
using N2.Web.UI;
using N2.Templates.Services;
using N2.Templates.Web.UI;
using N2.Engine;
using N2.Configuration;
using N2.Web;

namespace N2.Templates.Web
{
    /// <summary>
    /// Applies the template defined in the n2/templates configuration section 
    /// to the page.
    /// </summary>
    [Service(typeof(ContentPageConcern))]
    public class MasterPageConcern : ContentPageConcern
    {
        private IEngine engine;
        private string configuredMasterPagePath;

        public MasterPageConcern(IEngine engine)
        {
            this.engine = engine;
            var section = engine.Config.GetContentSection<TemplatesSection>("templates");
            if (section != null)
                configuredMasterPagePath = section.MasterPageFile;
        }

        public override void OnPreInit(System.Web.UI.Page page, ContentItem item)
        {
            if (string.IsNullOrEmpty(page.MasterPageFile))
                return;

            if (!string.IsNullOrEmpty(configuredMasterPagePath))
                page.MasterPageFile = configuredMasterPagePath;

            var defaultUrl = page.ResolveUrl(page.MasterPageFile);
            var alternateUrl = engine.ResolveAdapter<RequestAdapter>(item).ResolveTargetingUrl(defaultUrl);
            if (!string.Equals(alternateUrl, defaultUrl, System.StringComparison.InvariantCultureIgnoreCase))
                page.MasterPageFile = alternateUrl;
        }
    }
}
