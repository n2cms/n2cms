using System;
using N2.Definitions;
using N2.Edit;

namespace N2.Engine.Globalization
{
	/// <summary>
	/// Information used by the edit interface about an existing or potential translation.
	/// </summary>
	public class TranslateSpecification
	{
		public TranslateSpecification(string editUrl, ILanguage language, ContentItem existingItem, ItemDefinition definition,
		                              IEditUrlManager editUrlManager)
		{
			EditUrl = editUrl;
			Language = language;
			ExistingItem = existingItem;
			Definition = definition;
			FlagUrl = GetFlag(language, editUrlManager);
		}

		public ItemDefinition Definition { get; set; }

		public ILanguage Language { get; set; }

		public bool IsNew
		{
			get { return ExistingItem == null; }
		}

		public string EditUrl { get; set; }

		public ContentItem ExistingItem { get; set; }

		public string FlagUrl { get; set; }

		protected string GetFlag(ILanguage language, IEditUrlManager editUrlManager)
		{
			string flagUrl = language.FlagUrl;
			if (string.IsNullOrEmpty(flagUrl))
				return string.Format(editUrlManager.ResolveResourceUrl("{ManagementUrl}/Resources/Img/Flags/{0}.png"), language.LanguageCode);

			return flagUrl;
		}
	}
}