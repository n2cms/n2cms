using System;
using System.ComponentModel;

namespace N2.Web.UI
{
	[Obsolete("This interface has been deprecated. Either use IItemContainer or IContentTemplate")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IPageItemContainer
	{
		[Obsolete("This interface has been deprecated. Either use IItemContainer or IContentTemplate")]
		ContentItem CurrentPage { get; }
		[Obsolete("This interface has been deprecated. Either use IItemContainer or IContentTemplate")]
		ContentItem CurrentItem { get; }
	}
}