using System.Configuration;

namespace N2.Configuration
{
	/// <summary>
	/// Configuration related to versioning.
	/// </summary>
	public class VersionsElement : ConfigurationElement
	{
		/// <summary>Whether versions are stored when saving items using the editor interface.</summary>
		[ConfigurationProperty("enabled", DefaultValue = true)]
		public bool Enabled
		{
			get { return (bool)base["enabled"]; }
			set { base["enabled"] = value; }
		}

		/// <summary>Max versions to store for each item.</summary>
		[ConfigurationProperty("maximumPerItem", DefaultValue = 100)]
		public int MaximumPerItem
		{
			get { return (int)base["maximumPerItem"]; }
			set { base["maximumPerItem"] = value; }
		}
	}
}
