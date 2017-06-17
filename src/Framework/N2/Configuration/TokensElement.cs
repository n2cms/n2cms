using System.Configuration;

namespace N2.Configuration
{
	public class TokensElement : ConfigurationElement
	{
		/// <summary>Register built-in tokens view engine.</summary>
		[ConfigurationProperty("builtInEnabled", DefaultValue = true)]
		public bool BuiltInEnabled
		{
			get { return (bool)base["builtInEnabled"]; }
			set { base["builtInEnabled"] = value; }
		}
	}
}
