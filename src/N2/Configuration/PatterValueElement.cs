using System.Configuration;

namespace N2.Configuration
{
	/// <summary>
	/// Defines a replacment pattern for the name editor.
	/// </summary>
	public class PatterValueElement : ConfigurationElement
	{
		public PatterValueElement()
		{
		}

		public PatterValueElement(string pattern, string value, bool serverValidate)
		{
			Pattern = pattern;
			Value = value;
			ServerValidate = serverValidate;
		}

		/// <summary>A regular expression pattern used match replacements. This pattern should work both server and client side.</summary>
		[ConfigurationProperty("pattern")]
		public string Pattern
		{
			get { return (string)base["pattern"]; }
			set { base["pattern"] = value; }
		}

		/// <summary>The string to replace the pattern with.</summary>
		[ConfigurationProperty("value")]
		public string Value
		{
			get { return (string)base["value"]; }
			set { base["value"] = value; }
		}

		/// <summary>Validate on the server side that the pattern is not present in the name.</summary>
		[ConfigurationProperty("serverValidate", DefaultValue = true)]
		public bool ServerValidate
		{
			get { return (bool)base["serverValidate"]; }
			set { base["serverValidate"] = value; }
		}
	}
}