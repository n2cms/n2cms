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
        static OutputCacheParameters outputCacheParameters = new OutputCacheParameters();
        static bool outputCacheEnabled = false;

        public static bool OutputCacheEnabled
        {
            get { return TemplatesSection.outputCacheEnabled; }
        }

        public static OutputCacheParameters OutputCacheParameters
        {
            get
            {
                OutputCacheParameters parameters = new OutputCacheParameters();
                parameters.CacheProfile = outputCacheParameters.CacheProfile;
                parameters.Duration = outputCacheParameters.Duration;
                parameters.Enabled = outputCacheParameters.Enabled;
                parameters.Location = outputCacheParameters.Location;
                parameters.NoStore = outputCacheParameters.NoStore;
                parameters.SqlDependency = outputCacheParameters.SqlDependency;
                parameters.VaryByContentEncoding = outputCacheParameters.VaryByContentEncoding;
                parameters.VaryByControl = parameters.VaryByControl;
                parameters.VaryByCustom = parameters.VaryByCustom;
                parameters.VaryByHeader = parameters.VaryByHeader;
                parameters.VaryByParam = parameters.VaryByParam;
                return parameters;
            }
        }

        static TemplatesSection()
        {
            TemplatesSection config = WebConfigurationManager.GetSection("n2/templates") as TemplatesSection;
            if (config != null && config.OutputCache.Enabled)
            {
                outputCacheEnabled = true;
                outputCacheParameters.Enabled = config.OutputCache.Enabled;
                outputCacheParameters.VaryByParam = config.OutputCache.VaryByParam;
                outputCacheParameters.CacheProfile = config.OutputCache.CacheProfile;
                outputCacheParameters.Duration = config.OutputCache.Duration;
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
