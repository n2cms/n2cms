using System;
using System.Collections.Generic;
using N2.Persistence;

namespace N2.Serialization
{
	public class ReadingJournal : IImportRecord
	{
		private readonly IList<ContentItem> readItems = new List<ContentItem>();
		private readonly IList<Attachment> attachments = new List<Attachment>();

		public event EventHandler<ItemEventArgs> ItemAdded;

		public IList<ContentItem> ReadItems
		{
			get { return readItems; }
		}

		public ContentItem LastItem
		{
			get
			{
				if (readItems.Count == 0)
					return null;
				return readItems[readItems.Count-1];
			}
		}

		public ContentItem RootItem
		{
			get
			{
				if(readItems.Count == 0)
					return null;
				return readItems[0];
			}
		}

		public IList<Attachment> Attachments
		{
			get { return attachments; }
		}

		public void Report(ContentItem item)
		{
			readItems.Add(item);
			if(ItemAdded != null)
				ItemAdded.Invoke(this, new ItemEventArgs(item));
		}

		public void Report(Attachment a)
		{
			attachments.Add(a);
		}

		public ContentItem Find(int itemiD)
		{
			foreach (ContentItem previousItem in readItems)
			{
				if (previousItem.ID == itemiD)
				{
					return previousItem;
				}
			}
			return null;
		}
	}
}
