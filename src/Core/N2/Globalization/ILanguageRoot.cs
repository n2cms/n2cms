using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Globalization
{
	public interface ILanguageRoot
	{
		string FlagUrl { get; }
		string LanguageTitle { get; }
		string LanguageCode { get; }
	}
}
