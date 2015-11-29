using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N2.Engine.Globalization
{
	/// <summary>
	/// Provides translated texts for end user user interfaces.
	/// </summary>
	public interface ITranslator
	{
		string Translate(string key, string fallback = null);
		IDictionary<string, string> GetTranslations();
	}
}
