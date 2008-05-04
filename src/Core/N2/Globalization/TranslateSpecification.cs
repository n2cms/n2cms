using System;
using System.Collections.Generic;
using System.Text;
using N2.Definitions;

namespace N2.Globalization
{
	/// <summary>
	/// Information used by the edit interface about an existing or potential translation.
	/// </summary>
	public class TranslateSpecification
	{
		string editUrl;
		ILanguage language;
		string flagUrl;
		ContentItem existingItem;
		ItemDefinition definition;

		public TranslateSpecification(string editUrl, ILanguage language, ContentItem existingItem, ItemDefinition definition)
		{
			this.editUrl = editUrl;
			this.language = language;
			this.existingItem = existingItem;
			this.definition = definition;
			this.flagUrl = GetFlag(language); ;
		}

		public ItemDefinition Definition
		{
			get { return definition; }
			set { definition = value; }
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
