using N2.Engine;
using N2.Plugin;

namespace N2.Resources
{
    [Service]
    public class ResourceInitializer : IAutoStart
    {
        Configuration.ConfigurationManagerWrapper configFactory;

        public ResourceInitializer(Configuration.ConfigurationManagerWrapper configFactory)
        {
            this.configFactory = configFactory;
        }

        #region IAutoStart Members

        public void Start()
        {
            if (configFactory.Sections.Web.Resources.Debug.HasValue)
                Register.Debug = configFactory.Sections.Web.Resources.Debug.Value;
            Register.JQueryPath = configFactory.Sections.Web.Resources.JQueryPath;
            Register.JQueryUiPath = configFactory.Sections.Web.Resources.JQueryUiPath;
            Register.JQueryPluginsPath = configFactory.Sections.Web.Resources.JQueryPluginsPath;
            Register.AngularPath = configFactory.Sections.Web.Resources.AngularPath;
            Register.AngularResourcesPath = configFactory.Sections.Web.Resources.AngularResourcesPath;
            Register.CKEditorPath = configFactory.Sections.Web.Resources.CKEditorPath;
            Register.FancyboxPath = configFactory.Sections.Web.Resources.FancyboxPath;
            Register.PartsJsPath = configFactory.Sections.Web.Resources.PartsJsPath;
            Register.PartsCssPath = configFactory.Sections.Web.Resources.PartsCssPath;
            Register.TwitterBootstrapCssPath = configFactory.Sections.Web.Resources.TwitterBootstrapCssPath;
            Register.TwitterBootstrapResponsiveCssPath = configFactory.Sections.Web.Resources.TwitterBootstrapResponsiveCssPath;
            Register.TwitterBootstrapJsPath = configFactory.Sections.Web.Resources.TwitterBootstrapJsPath;
            Register.IconsCssPath = configFactory.Sections.Web.Resources.IconsCssPath;
        }

        public void Stop()
        {
        }

        #endregion
    }
}
