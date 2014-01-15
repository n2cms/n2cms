using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Collections
{
    /// <summary>
    /// A state hint which helps avoiding querying for children in certain situations.
    /// </summary>
    [Flags]
    public enum CollectionState
    {
        /// <summary>Any children status has not been determined.</summary>
        Unknown = 0,

        /// <summary>The child collection is big. The size determined to be large is configurable.</summary>
        IsLarge = 1,

        /// <summary>The child collection contains no item (leaf node).</summary>
        IsEmpty = 2,
        
        /// <summary>Has child pages accessible to everyone but which are visible.</summary>
        ContainsVisiblePublicPages = 4,
        /// <summary>Has child pages accessible to everyone but not visible.</summary>
        ContainsHiddenPublicPages = 8,
        /// <summary>Has child pages which requires special read permissions.</summary>
        ContainsVisibleSecuredPages = 16,
        /// <summary>Has child pages which requires special read permissions.</summary>
        ContainsHiddenSecuredPages = 32,
        
        /// <summary>Has child parts which are accessible to everyone.</summary>
        ContainsPublicParts = 128,
        /// <summary>Has child parts which requires special read permissions.</summary>
        ContainsSecuredParts = 256
    }
}
