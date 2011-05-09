using System;
using System.Collections.Generic;

namespace N2.Collections
{
	/// <summary>
	/// Filters items not belonging to a certain parent item.
	/// </summary>
	public class ParentFilter : ItemFilter
	{
		ContentItem parent;
		private int? parentID;

		public ParentFilter(int parentID)
		{
			this.parentID = parentID;
		}

		public ParentFilter(ContentItem parent)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");

			this.parent = parent;
			this.parentID = parent.ID;
		}

		public override bool Match(ContentItem item)
		{
			if (parent != null)
			{
				return item != parent
						&& Utility.GetTrail(item).StartsWith(Utility.GetTrail(parent));
			}

			while ((item = item.Parent) != null)
			{
				if (item.ID == parentID)
					return true;
			}
			return false;
		}

		#region Static Methods
		public static void Filter(IList<ContentItem> items, ContentItem parent)
		{
			Filter(items, new ParentFilter(parent));
		}
		public static void Filter(IList<ContentItem> items, int parentID)
		{
			Filter(items, new ParentFilter(parentID));
		}
		#endregion
	}
}
