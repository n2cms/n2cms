using System.Configuration;
using System;

namespace N2.Templates.Configuration
{
	/// <summary>
	/// Configuration options for the N2 functional templates project.
	/// </summary>
    public class TemplatesSection : ConfigurationSection
    {
        /// <summary>The database flavour decides which propertes the nhibernate configuration will receive.</summary>
        [ConfigurationProperty("mailConfiguration", DefaultValue = MailConfigSource.ContentRootOrConfiguration)]
        public MailConfigSource MailConfiguration
        {
            get { return (MailConfigSource)base["mailConfiguration"]; }
            set { base["mailConfiguration"] = value; }
        }

        /// <summary>The master page used for template pages.</summary>
        [ConfigurationProperty("masterPageFile", DefaultValue = "~/Templates/UI/Layouts/Top+SubMenu.Master")]
		public string MasterPageFile
        {
            get { return (string)base["masterPageFile"]; }
            set { base["masterPageFile"] = value; }
		}

		/// <summary>Image resize handler url.</summary>
		[ConfigurationProperty("imageHandlerPath")]
		public string ImageHandlerPath
		{
			get { return (string)base["imageHandlerPath"]; }
			set { base["imageHandlerPath"] = value; }
		}

        /// <summary>Configuration related to the wiki module.</summary>
        [ConfigurationProperty("wiki")]
        public WikiElement Wiki
        {
            get { return (WikiElement)base["wiki"]; }
            set { base["wiki"] = value; }
        }
    }
}
