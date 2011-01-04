using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Engine.Globalization
{
	/// <summary>
	/// Used to cache information about a language available in the system.
	/// </summary>
	public class LanguageInfo
	{
		/// <summary>The identifier of item that is the root of this language.</summary>
		public int ID { get; set; }

		/// <summary>The language code of this language.</summary>
		public string LanguageCode { get; set; }
	}
}
