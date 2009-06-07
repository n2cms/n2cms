using System;
using N2.Templates.Mvc.Items.Items;
using N2.Web.Mvc;
using N2.Web.UI;

namespace N2.Templates.Mvc.Models
{
	public class TrackingModel : IItemContainer<Tracking>
	{
		public TrackingModel(Tracking currentItem, bool track)
		{
			CurrentItem = currentItem;
			Track = track;
		}

		/// <summary>Gets the item associated with the item container.</summary>
		ContentItem IItemContainer.CurrentItem
		{
			get { return CurrentItem; }
		}

		public Tracking CurrentItem { get; set; }

		public bool Track { get; set; }
	}
}