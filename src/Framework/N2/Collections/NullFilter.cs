using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Collections
{
    /// <summary>
    /// A very positive filter that match any item.
    /// </summary>
    public class NullFilter : ItemFilter
    {
        public override bool Match(ContentItem item)
        {
            return true;
        }
    }
}
