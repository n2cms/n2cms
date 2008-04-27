using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Globalization
{
	public class TranslationOption
	{
		string editUrl;
		ILanguage language;
		string flagUrl;
		ContentItem existingItem;

		public TranslationOption(string editUrl, ILanguage language, ContentItem existingItem)
		{
			this.editUrl = editUrl;
			this.language = language;
			this.existingItem = existingItem;
			this.flagUrl = GetFlag(language); ;
		}

		public ILanguage Language
		{
			get { return language; }
			set { language = value; }
		}

		public bool IsNew
		{
			get { return ExistingItem == null; }
		}

		public string EditUrl
		{
			get { return editUrl; }
			set { editUrl = value; }
		}

		public ContentItem ExistingItem
		{
			get { return existingItem; }
			set { existingItem = value; }
		}

		public string FlagUrl
		{
			get { return flagUrl; }
			set { flagUrl = value; }
		}

		protected string GetFlag(ILanguage language)
		{
			string flagUrl = language.FlagUrl;
			if (string.IsNullOrEmpty(flagUrl))
				return string.Format(Utility.ToAbsolute("~/Edit/Globalization/flags/{0}.png"), language.LanguageCode);
			else
				return flagUrl;
		}
	}
}
