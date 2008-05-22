using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Web.UI;
using N2.Web.UI;
using N2.Web.UI.WebControls;

namespace N2.Details
{
	/// <summary>
	/// Defines an editable sub-item. The edited item is found by looking for 
	/// children with the <see cref="DefaultChildName"/> name.
	/// </summary>
	/// <example>
	/// 	[N2.Details.WithEditableChild(typeof(ChildItem), "News", 10)]
	///		public class ParentItem : N2.ContentItem
	///		{
	///		}
	/// </example>
	[AttributeUsage(AttributeTargets.Class)]
	public class WithEditableChildAttribute : EditableItemAttribute
	{
		private Type childType;

		public Type ChildType
		{
			get { return childType; }
			set { childType = value; }
		}

		public WithEditableChildAttribute(Type childType, string childName, int sortOrder)
		{
			this.ChildType = childType;
			this.DefaultChildName = childName;
			this.Name = childName;
			this.SortOrder = sortOrder;
		}

		protected override ContentItem GetChild(ContentItem item)
		{
			ContentItem childItem = item.GetChild(this.DefaultChildName);
			if (childItem == null)
			{
				childItem = CreateChild(item, this.ChildType);
			}
			return childItem;
		}
	}
}
