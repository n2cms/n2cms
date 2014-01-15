using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Definitions
{
    /// <summary>
    /// An interface that marks an item that overrides the regular url generation. A
    /// <see cref="ContentItem"/> with this attribute is assigned the given url instead
    /// of the default (strucutre based) url. Children of this item receive corresponding 
    /// urls.
    /// </summary>
    public interface IUrlSource
    {
        /// <summary>The direct url to the content item. This value is cached by the system after first read.</summary>
        string DirectUrl { get; }
    }
}
