using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Collections;

namespace N2.Definitions
{
    /// <summary>
    /// When defined on a content item this tells the management UI to ask the model 
    /// for children rather than querying the database directly.
    /// </summary>
    public interface IActiveChildren
    {
        IEnumerable<ContentItem> GetChildren(ItemFilter filter);
    }
}
