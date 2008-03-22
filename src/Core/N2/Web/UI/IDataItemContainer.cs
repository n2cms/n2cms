using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Web.UI
{
    /// <summary>Defines an interface that controls can use when traversing the control hierarchy to find the current data item.</summary>
    [Obsolete("Serves no useful purpose, use CurrentItem instead.")]
	public interface IDataItemContainer : IItemContainer
    {
        ContentItem CurrentData { get; set; }
    }
}