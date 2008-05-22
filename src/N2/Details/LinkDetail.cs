using System;
using System.Diagnostics;

namespace N2.Details
{
	/// <summary>
	/// A link detail used to relate items to each other.
	/// </summary>
	[DebuggerDisplay("{Name}: LinkedItem: {LinkedItem}")]
	public class LinkDetail : ContentDetail
	{
		public LinkDetail() : base()
		{
		}

		public LinkDetail(ContentItem containerItem, string name, ContentItem value)
		{
			ID = 0;
			EnclosingItem = containerItem;
			Name = name;
			LinkedItem = value;
		}

		private ContentItem linkedItem;

		public virtual ContentItem LinkedItem
		{
			get { return linkedItem; }
			set
			{
				linkedItem = value;
				if (value != null)
					LinkValue = value.ID;
				else
					LinkValue = null;
			}
		}

		private int? linkValue;

		protected virtual int? LinkValue
		{
			get { return linkValue; }
			set { linkValue = value; }
		}

		public override object Value
		{
			get { return LinkedItem; }
			set { LinkedItem = (ContentItem) value; }
		}

		public override Type ValueType
		{
			get { return typeof (ContentItem); }
		}
	}
}