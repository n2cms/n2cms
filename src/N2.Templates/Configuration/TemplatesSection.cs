using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Web.UI;
using System.Web.Configuration;

namespace N2.Templates.Configuration
{
    public class TemplatesSection : ConfigurationSection
    {
        /// <summary>The database flavour decides which propertes the nhibernate configuration will receive.</summary>
        [ConfigurationProperty("mailConfiguration", DefaultValue = MailConfigSource.ContentRootOrConfiguration)]
        public MailConfigSource MailConfiguration
        {
            get { return (MailConfigSource)base["mailConfiguration"]; }
            set { base["mailConfiguration"] = value; }
        }
    }
}
