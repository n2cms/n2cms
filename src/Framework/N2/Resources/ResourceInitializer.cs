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
			Register.Debug = configFactory.Sections.Web.Resources.Debug;
			Register.JQueryPath = configFactory.Sections.Web.Resources.JQueryPath;
			Register.JQueryUiPath = configFactory.Sections.Web.Resources.JQueryUiPath;
			Register.JQueryPluginsPath = configFactory.Sections.Web.Resources.JQueryPluginsPath;
			Register.CKEditorPath = configFactory.Sections.Web.Resources.CKEditorPath;
			Register.PartsJsPath = configFactory.Sections.Web.Resources.PartsJsPath;
			Register.PartsCssPath = configFactory.Sections.Web.Resources.PartsCssPath;
			Register.TwitterBootstrapCssPath = configFactory.Sections.Web.Resources.TwitterBootstrapCssPath;
			Register.TwitterBootstrapResponsiveCssPath = configFactory.Sections.Web.Resources.TwitterBootstrapResponsiveCssPath;
			Register.TwitterBootstrapJsPath = configFactory.Sections.Web.Resources.TwitterBootstrapJsPath;
		}

		public void Stop()
		{
		}

		#endregion
	}
}
