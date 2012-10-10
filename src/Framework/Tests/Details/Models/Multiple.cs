using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Details;
using N2.Collections;

namespace N2.Tests.Details.Models
{
	[WithEditableTitle]
	[WithEditableName]
	public class DecoratedItem : ContentItem
	{
		[EditableChildren("Children", "Children", 100)]
		public ItemList EditableChildren
		{
			get { return null; }
		}

		[EditableChildren("GenericChildren", "GenericChildren", 100)]
		public IList<BaseItem> GenericChildren
		{
			get { return null; }
		}

		[EditableItem]
		public virtual OtherItem TheItem { get; set; }

		public override ItemList GetChildren(string childZoneName)
		{
			return base.GetChildren(new ZoneFilter(childZoneName));
		}
	}

	[WithEditableTitle]
	[WithEditableName]
	public class OtherItem : ContentItem
	{
	}

	public class BaseItem : ContentItem
	{
	}

	public class SuperficialItem : BaseItem
	{
	}
}
