using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Addons.Wiki
{
    /// <summary>
    /// Provides access to tab-related constants. Use these constants instead of
    /// strings for better compile-time checking.
    /// </summary> 
    public static class Tabs
    {
        /// <summary>The default content tab when editing.</summary>
        public const string Content = "content";

        /// <summary>Advanced tab while editing.</summary>
        public const string Advanced = "advanced";

        public const int ContentIndex = 0;
        public const int AdvancedIndex = 100;
    }
}
