using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Definitions
{
	/// <summary>
	/// Used in combination with [SortChildren] attribute to define custom sorting of a certain type's children.
	/// </summary>
	/// <example>
	/// // Method 1: Sort children with separate service
	/// [SortChildren(typeof(MyCustomSorter))]
	/// public class MyCustomContainer : ContentItem
	/// {
	/// }
	/// 
	/// public class MyCustomSorter : IChildrenSorter
	/// {
	///    // custom sorting implemenation
	/// }
	/// 
	/// // Method 2: Parent content item implements sorting
	/// [SortChildren(SortBy.CustomSorter)]
	/// public class MySelfSortableContainer : ContentItem, IChildrenSorter
	/// {
	///    // custom sorting implemenation
	/// }
	/// 
	/// </example>
	public interface IChildrenSorter
	{
		IEnumerable<ContentItem> ReorderChildren(ContentItem item);
	}
}
