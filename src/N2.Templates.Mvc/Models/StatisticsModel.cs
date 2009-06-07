using System;
using System.Collections.Generic;
using N2.Templates.Mvc.Items.Items;
using N2.Web.Mvc;
using N2.Web.UI;

namespace N2.Templates.Mvc.Models
{
	public class StatisticsModel : IItemContainer<Statistics>
	{
		public StatisticsModel(Statistics currentItem)
		{
			CurrentItem = currentItem;
		}

		public IEnumerable<ContentItem> LatestChanges { get; set; }

		public int ChangesLastWeek { get; set; }

		public double VersionsPerItem { get; set; }

		public int? PagesServed { get; set; }

		public int NumberOfPages { get; set; }

		public int NumberOfItems { get; set; }

		#region IItemContainer<Statistics> Members

		/// <summary>Gets the item associated with the item container.</summary>
		ContentItem IItemContainer.CurrentItem
		{
			get { return CurrentItem; }
		}

		public Statistics CurrentItem { get; private set; }

		#endregion
	}
}