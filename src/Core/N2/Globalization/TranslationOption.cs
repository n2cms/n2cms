using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Globalization
{
	public class TranslationOption
	{
		string url;
		bool isNew;
		ILanguageRoot language;

		public TranslationOption(string url, bool isNew, ILanguageRoot language)
		{
			this.url = url;
			this.isNew = isNew;
			this.language = language;
		}

		public ILanguageRoot Language
		{
			get { return language; }
			set { language = value; }
		}

		public bool IsNew
		{
			get { return isNew; }
			set { isNew = value; }
		}

		public string Url
		{
			get { return url; }
			set { url = value; }
		}
	}
}
