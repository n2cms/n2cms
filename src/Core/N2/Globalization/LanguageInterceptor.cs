using System;
using System.Collections.Generic;
using System.Text;
using Castle.Core;
using N2.Persistence;
using N2.Web;
using N2.Definitions;

namespace N2.Globalization
{
	public class LanguageInterceptor : IStartable
	{
		private const string DeletingKey = "LanguageInterceptor_Deleting";

		private readonly IPersister persister;
		private readonly IDefinitionManager definitions;
		private readonly IWebContext context;
		private readonly ILanguageGateway gateway;

		public LanguageInterceptor(IPersister persister, IDefinitionManager definitions, IWebContext context, ILanguageGateway gateway)
		{
			this.persister = persister;
			this.definitions = definitions;
			this.context = context;
			this.gateway = gateway;
		}


		void definitions_ItemCreated(object sender, ItemEventArgs e)
		{
			if (context.QueryString[LanguageGateway.LanguageKey] != null)
			{
				if (e.AffectedItem is ILanguage)
					return;

				UpdateSortOrder(e.AffectedItem);
			}
		}

		private void UpdateSortOrder(ContentItem item)
		{
			int languageKey;
			if (int.TryParse(context.QueryString[LanguageGateway.LanguageKey], out languageKey))
			{
				ContentItem translation = persister.Get(languageKey);
				if(translation != null)
					item.SortOrder = translation.SortOrder;
			}
		}

		void persister_ItemDeleting(object sender, CancellableItemEventArgs e)
		{
			// prevent infinite recursion
			if (context.RequestItems[DeletingKey] != null)
				return;

			ContentItem item = e.AffectedItem;
			using (new DictionaryScope(context.RequestItems, DeletingKey, item))
			{
				if (item is ILanguage)
					return;
				
				DeleteTranslations(item);
			}
		}

		private void DeleteTranslations(ContentItem item)
		{
			foreach (ContentItem translatedItem in gateway.FindTranslations(item))
			{
				if (translatedItem != item)
				{
					persister.Delete(translatedItem);
				}
			}
		}

		void persister_ItemMoved(object sender, DestinationEventArgs e)
		{
			ContentItem item = e.AffectedItem;
			if (item is ILanguage)
				return;
			ILanguage language = gateway.GetLanguage(item);

			if (language != null)
			{
				ContentItem destination = e.Destination;

				MoveTranslations(item, language, destination);
			}
		}

		private void MoveTranslations(ContentItem item, ILanguage language, ContentItem destination)
		{
			foreach (ContentItem translatedItem in gateway.FindTranslations(item))
			{
				ILanguage translationsLanguage = gateway.GetLanguage(translatedItem);
				ContentItem translatedDestination = gateway.GetTranslation(destination, translationsLanguage);
				if (translationsLanguage != language && translatedDestination != null && translatedItem.Parent != translatedDestination)
				{
					persister.Move(translatedItem, translatedDestination);
				}
			}
		}

		void persister_ItemSaved(object sender, ItemEventArgs e)
		{
			ContentItem item = e.AffectedItem;
			ILanguage language = gateway.GetLanguage(item);
			if (language != null)
			{
				UpdateLanguageKey(item);
			}
		}

		private void UpdateLanguageKey(ContentItem item)
		{
			int languageKey = item.ID;
			if (context.QueryString[LanguageGateway.LanguageKey] != null)
			{
				int.TryParse(context.QueryString[LanguageGateway.LanguageKey], out languageKey);
			}
			if (item[LanguageGateway.LanguageKey] == null)
			{
				if (languageKey != item.ID)
					EnsureLanguageKeyOnInitialTranslation(item, languageKey);

				item[LanguageGateway.LanguageKey] = languageKey;
				persister.Save(item);
			}
		}

		private void EnsureLanguageKeyOnInitialTranslation(ContentItem item, int languageKey)
		{	
			ContentItem initialTranslation = persister.Get(languageKey);
			if (initialTranslation[LanguageGateway.LanguageKey] == null)
			{
				initialTranslation[LanguageGateway.LanguageKey] = languageKey;
				persister.Save(initialTranslation);
			}
		}

		#region IStartable Members

		public void Start()
		{
			persister.ItemSaved += persister_ItemSaved;
			persister.ItemMoved += persister_ItemMoved;
			persister.ItemDeleting += persister_ItemDeleting;
			definitions.ItemCreated += definitions_ItemCreated;
		}

		public void Stop()
		{
			persister.ItemSaved -= persister_ItemSaved;
			persister.ItemMoved -= persister_ItemMoved;
			persister.ItemDeleting -= persister_ItemDeleting;
			definitions.ItemCreated -= definitions_ItemCreated;
		}

		#endregion
	}
}
