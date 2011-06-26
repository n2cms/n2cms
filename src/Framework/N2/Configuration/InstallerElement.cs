using System.Configuration;

namespace N2.Configuration
{
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
		[ConfigurationProperty("allowInstallation", DefaultValue = true)]
		public bool AllowInstallation
		{
			get { return (bool)base["allowInstallation"]; }
			set { base["allowInstallation"] = value; }
		}

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