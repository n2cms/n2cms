using N2.Engine;
using N2.Plugin;

namespace N2.Resources
{
	[Service]
	public class ResourceInitializer : IAutoStart
	{
		readonly Configuration.ConfigurationManagerWrapper configFactory;

		public ResourceInitializer(Configuration.ConfigurationManagerWrapper configFactory)
		{
			this.configFactory = configFactory;
		}

		#region IAutoStart Members

		/// <summary>
		/// Initializes the paths to the resource files from the N2 configuration. This happens AFTER the
		/// N2.Resources.Register() constructor which initializes the defaults.
		/// </summary>
		/// <see cref="N2.Resources.Register"/>
		public void Start()
		{
			if (configFactory.Sections.Web.Resources.Debug.HasValue)
				Register.Debug = configFactory.Sections.Web.Resources.Debug.Value;

			Register.JQueryJsPath = configFactory.Sections.Web.Resources.JQueryJsPath;
			Register.JQueryUiPath = configFactory.Sections.Web.Resources.JQueryUiPath;
			Register.JQueryPluginsPath = configFactory.Sections.Web.Resources.JQueryPluginsPath;

			Register.AngularJsRoot = configFactory.Sections.Web.Resources.AngularJsRoot;
			Register.AngularStrapJsPath = configFactory.Sections.Web.Resources.AngularStrapJsPath;
			Register.AngularUiJsPath = configFactory.Sections.Web.Resources.AngularUiJsPath;

			Register.CkEditorJsPath = configFactory.Sections.Web.Resources.CkEditorPath;

			Register.FancyboxJsPath = configFactory.Sections.Web.Resources.FancyboxJsPath;
			Register.FancyboxCssPath = configFactory.Sections.Web.Resources.FancyboxCssPath;

			Register.PartsJsPath = configFactory.Sections.Web.Resources.PartsJsPath;
			Register.PartsCssPath = configFactory.Sections.Web.Resources.PartsCssPath;

			Register.BootstrapCssPath = configFactory.Sections.Web.Resources.BootstrapCssPath;
			Register.BootstrapJsPath = configFactory.Sections.Web.Resources.BootstrapJsPath;
			Register.BootstrapDatePickerCssPath = configFactory.Sections.Web.Resources.BootstrapDatePickerCssPath;
			Register.BootstrapDatePickerJsPath = configFactory.Sections.Web.Resources.BootstrapDatePickerJsPath;
			Register.BootstrapTimePickerCssPath = configFactory.Sections.Web.Resources.BootstrapTimePickerCssPath;
			Register.BootstrapTimePickerJsPath = configFactory.Sections.Web.Resources.BootstrapTimePickerJsPath;

			Register.IconsCssPath = configFactory.Sections.Web.Resources.IconsCssPath;
		}

		public void Stop()
		{
		}

		#endregion
	}
}
