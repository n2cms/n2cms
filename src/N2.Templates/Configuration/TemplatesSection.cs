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
        static OutputCacheParameters defaultOutputCacheParameters = new OutputCacheParameters();

        public static OutputCacheParameters DefaultOutputCacheParameters
        {
            get { return TemplatesSection.defaultOutputCacheParameters; }
        }

        static TemplatesSection()
        {
            TemplatesSection config = WebConfigurationManager.GetSection("n2/templates") as TemplatesSection;
            if (config != null && config.OutputCache.Enabled)
            {
                defaultOutputCacheParameters.Enabled = config.OutputCache.Enabled;
                defaultOutputCacheParameters.VaryByParam = config.OutputCache.VaryByParam;
                defaultOutputCacheParameters.CacheProfile = config.OutputCache.CacheProfile;
                defaultOutputCacheParameters.Duration = config.OutputCache.Duration;
            }
            else
            {
                defaultOutputCacheParameters.Enabled = false;
            }
        }

        /// <summary>The database flavour decides which propertes the nhibernate configuration will receive.</summary>
        [ConfigurationProperty("mailConfiguration", DefaultValue = MailConfigSource.ContentRootOrConfiguration)]
        public MailConfigSource MailConfiguration
        {
            get { return (MailConfigSource)base["mailConfiguration"]; }
            set { base["mailConfiguration"] = value; }
        }

        /// <summary>Configures output cache for the templates.</summary>
        [ConfigurationProperty("outputCache")]
        public OutputCacheElement OutputCache
        {
            get { return (OutputCacheElement)base["outputCache"]; }
        }
    }
}
