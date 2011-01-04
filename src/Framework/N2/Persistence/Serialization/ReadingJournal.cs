using System;
using System.Collections.Generic;

namespace N2.Persistence.Serialization
{
	public class ReadingJournal : IImportRecord
	{
		readonly IList<ContentItem> readItems = new List<ContentItem>();
        readonly IList<Attachment> attachments = new List<Attachment>();
        readonly IList<Attachment> failedAttachments = new List<Attachment>();
		readonly IList<Exception> errors = new List<Exception>();
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

		public IList<Attachment> FailedAttachments
		{
			get { return failedAttachments; }
		}
		public IList<Exception> Errors
		{
			get { return errors; }
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

		public void Error(Exception ex)
		{
			Errors.Add(ex);
		}
	}
}
