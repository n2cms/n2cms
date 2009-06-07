using System;
using System.Collections.Generic;
using N2.Templates.Mvc.Items.Pages;
using N2.Web.Mvc;
using N2.Web.UI;

namespace N2.Templates.Mvc.Models
{
	public class SearchModel : IItemContainer<AbstractSearch>
	{
		public SearchModel(AbstractSearch currentItem, ICollection<ContentItem> results)
		{
			CurrentItem = currentItem;
			Results = results;
		}

		/// <summary>Gets the item associated with the item container.</summary>
		ContentItem IItemContainer.CurrentItem
		{
			get { return CurrentItem; }
		}

		public AbstractSearch CurrentItem { get; private set; }
		public ICollection<ContentItem> Results { get; private set; }

		public string SearchTerm { get; set; }

		public bool HasSearchTerm
		{
			get { return !String.IsNullOrEmpty(SearchTerm); }
		}
	}
}