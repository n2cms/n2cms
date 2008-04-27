using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Globalization
{
	public interface ILanguage
	{
		string FlagUrl { get; }
		string LanguageTitle { get; }
		string LanguageCode { get; }
	}
}
