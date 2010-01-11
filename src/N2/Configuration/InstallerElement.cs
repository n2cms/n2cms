using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace N2.Configuration
{
    public class InstallerElement : ConfigurationElement
    {
        /// <summary>When set to true this setting will cause the database connection to be verified upon startup. If the database connection is down the first user is redirected to an installation screen.</summary>
        [ConfigurationProperty("checkInstallationStatus", DefaultValue = false)]
        public bool CheckInstallationStatus
        {
            get { return (bool)base["checkInstallationStatus"]; }
            set { base["checkInstallationStatus"] = value; }
        }

		[ConfigurationProperty("installUrl", DefaultValue = "~/N2/Installation/Begin/Default.aspx")]
        public string InstallUrl
        {
            get { return (string)base["installUrl"]; }
            set { base["installUrl"] = value; }
        }
    }
}
