using System.Configuration;

namespace N2.Configuration
{
    public static class AllowInstallationOption
    {
        public const string AnonymousUser = "AnonymousUser";
        public const string Administrator = "Administrator";
        public const string No = "No";
        public static string Parse(string configValue)
        {
            if (configValue == AnonymousUser || "true".Equals(configValue, System.StringComparison.InvariantCultureIgnoreCase))
                return AnonymousUser;
            if (configValue == No || "false".Equals(configValue, System.StringComparison.InvariantCultureIgnoreCase))
                return No;
            
            return Administrator;
        }
    }

    public class InstallerElement : ConfigurationElement
    {
        /// <summary>When set to true this setting will cause the database connection to be verified upon startup. If the database connection is down the first user is redirected to an installation screen.</summary>
        [ConfigurationProperty("checkInstallationStatus", DefaultValue = true)]
        public bool CheckInstallationStatus
        {
            get { return (bool)base["checkInstallationStatus"]; }
            set { base["checkInstallationStatus"] = value; }
        }

        /// <summary>When set to false this option disallows usage of the potentially dangerous interfaces located below /n2/installation/.</summary>
        [ConfigurationProperty("allowInstallation", DefaultValue = AllowInstallationOption.Administrator)]
        public string AllowInstallation
        {
            get { return (string)base["allowInstallation"]; }
            set { base["allowInstallation"] = value; }
        }

        //[ConfigurationProperty("installedVersion")]
        //public string InstalledVersion
        //{
        //  get { return (string)base["installedVersion"]; }
        //  set { base["installedVersion"] = value; }
        //}

        [ConfigurationProperty("welcomeUrl", DefaultValue = "{ManagementUrl}/Installation/Begin/Default.aspx")]
        public string WelcomeUrl
        {
            get { return (string)base["welcomeUrl"]; }
            set { base["welcomeUrl"] = value; }
        }

        [ConfigurationProperty("installUrl", DefaultValue = "{ManagementUrl}/Installation/Default.aspx")]
        public string InstallUrl
        {
            get { return (string)base["installUrl"]; }
            set { base["installUrl"] = value; }
        }

        [ConfigurationProperty("upgradeUrl", DefaultValue = "{ManagementUrl}/Installation/Upgrade.aspx")]
        public string UpgradeUrl
        {
            get { return (string)base["upgradeUrl"]; }
            set { base["upgradeUrl"] = value; }
        }

        [ConfigurationProperty("rebaseUrl", DefaultValue = "{ManagementUrl}/Installation/Rebase.aspx")]
        public string RebaseUrl
        {
            get { return (string)base["rebaseUrl"]; }
            set { base["rebaseUrl"] = value; }
        }

        [ConfigurationProperty("fixClass", DefaultValue = "{ManagementUrl}/Installation/FixClass.aspx")]
        public string FixClassUrl
        {
            get { return (string)base["fixClass"]; }
            set { base["fixClass"] = value; }
        }
    }
}
