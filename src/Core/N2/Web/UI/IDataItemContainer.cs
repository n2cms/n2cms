using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Web.UI
{
    /// <summary>
	/// Defines an interface used for setting the content item associated with a part template (ascx).
	/// </summary>
	public interface IDataItemContainer : IItemContainer
    {
        ContentItem CurrentItem { get; set; }
    }
}