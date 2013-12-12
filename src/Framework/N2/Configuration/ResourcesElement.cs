using System.Configuration;

namespace N2.Configuration
{
    public class ResourcesElement : ConfigurationElement
    {
        public const string JQueryVersion = "1.10.2";

        /// <summary>Whether to make registered web resources debuggable.</summary>
        [ConfigurationProperty("debug")]
        public bool? Debug
        {
            get { return (bool?)base["debug"]; }
            set { base["debug"] = value; }
        }

        /// <summary>The path to the included jQuery javascript resource.</summary>
        [ConfigurationProperty("jQueryPath", DefaultValue = "{ManagementUrl}/Resources/Js/jquery-" + JQueryVersion + ".min.js")]
        public string JQueryPath
        {
            get { return (string)base["jQueryPath"]; }
            set { base["jQueryPath"] = value; }
        }

        /// <summary>The path to the included jQuery UI javascript resource.</summary>
        [ConfigurationProperty("jQueryUiPath", DefaultValue = "{ManagementUrl}/Resources/jquery-ui-1.10.2.custom/js/jquery-ui-1.10.2.custom.min.js")]
        public string JQueryUiPath
        {
            get { return (string)base["jQueryUiPath"]; }
            set { base["jQueryUiPath"] = value; }
        }

        /// <summary>The path to the included jQuery plgins javascript resource.</summary>
        [ConfigurationProperty("jQueryPluginsPath", DefaultValue = "{ManagementUrl}/Resources/Js/plugins.ashx?v=" + JQueryVersion)]
        public string JQueryPluginsPath
        {
            get { return (string)base["jQueryPluginsPath"]; }
            set { base["jQueryPluginsPath"] = value; }
        }

        /// <summary>The path to the included angular javascript resource.</summary>
        [ConfigurationProperty("angularPath", DefaultValue = "{ManagementUrl}/Resources/angular-1.1.5/angular.min.js")]
        public string AngularPath
        {
            get { return (string)base["angularPath"]; }
            set { base["angularPath"] = value; }
        }

        /// <summary>The path to the included angular javascript resource.</summary>
        [ConfigurationProperty("angularResourcesPath", DefaultValue = "{ManagementUrl}/Resources/angular-1.1.5/angular-resource.min.js")]
        public string AngularResourcesPath
        {
            get { return (string)base["angularResourcesPath"]; }
            set { base["angularResourcesPath"] = value; }
        }

        /// <summary>The path to the included tiny MCE javascript resource.</summary>
        [ConfigurationProperty("ckEditorPath", DefaultValue = "{ManagementUrl}/Resources/ckeditor/ckeditor.js?v=" + JQueryVersion)]
        public string CKEditorPath
        {
            get { return (string)base["ckEditorPath"]; }
            set { base["ckEditorPath"] = value; }
        }

        [ConfigurationProperty("FancyboxPath", DefaultValue = N2.Resources.Register.DefaultFancyboxPath)]
        public string FancyboxPath
        {
            get { return (string)base["FancyboxPath"]; }
            set { base["FancyboxPath"] = value; }
        }

        [ConfigurationProperty("partsJsPath", DefaultValue = "{ManagementUrl}/Resources/Js/parts.js?v=" + JQueryVersion)]
        public string PartsJsPath
        {
            get { return (string)base["partsJsPath"]; }
            set { base["partsJsPath"] = value; }
        }

        [ConfigurationProperty("partsCssPath", DefaultValue = "{ManagementUrl}/Resources/Css/parts.css?v=" + JQueryVersion)]
        public string PartsCssPath
        {
            get { return (string)base["partsCssPath"]; }
            set { base["partsCssPath"] = value; }
        }

        [ConfigurationProperty("twitterBootstrapCssPath", DefaultValue = N2.Resources.Register.DefaultBootstrapCssPath)]
        public string TwitterBootstrapCssPath
        {
            get { return (string)base["twitterBootstrapCssPath"]; }
            set { base["twitterBootstrapCssPath"] = value; }
        }

        [ConfigurationProperty("twitterBootstrapResponsiveCssPath", DefaultValue = N2.Resources.Register.DefaultBootstrapResponsiveCssPath)]
        public string TwitterBootstrapResponsiveCssPath
        {
            get { return (string)base["twitterBootstrapResponsiveCssPath"]; }
            set { base["twitterBootstrapResponsiveCssPath"] = value; }
        }

        [ConfigurationProperty("twitterBootstrapJsPath", DefaultValue = N2.Resources.Register.DefaultBootstrapJsPath)]
        public string TwitterBootstrapJsPath
        {
            get { return (string)base["twitterBootstrapJsPath"]; }
            set { base["twitterBootstrapJsPath"] = value; }
        }

        [ConfigurationProperty("iconsCssPath", DefaultValue = N2.Resources.Register.DefaultIconsCssPath)]
        public string IconsCssPath
        {
            get { return (string)base["iconsCssPath"]; }
            set { base["iconsCssPath"] = value; }
        }
    }
}
