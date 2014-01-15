using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Definitions
{
    /// <summary>
    /// Marks a content item that redirects to another location.
    /// </summary>
    public interface IRedirect
    {
        /// <summary>The url the item redirects to.</summary>
        string RedirectUrl { get; }

        /// <summary>Another item the item redirects to, or null if the redirect url isn't a content item.</summary>
        ContentItem RedirectTo { get; }
    }
}
