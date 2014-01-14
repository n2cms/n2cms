using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Templates
{
    /// <summary>
    /// Provides access to tab-related constants. Use these constants instead of
    /// strings for better compile-time checking. Please note that you still 
    /// need to make sure the zone is defined for the container you chose.
    /// </summary> 
    public class Tabs
    {
        /// <summary>The default content tab when editing.</summary>
        public const string Content = "content";

        /// <summary>SEO tab while editing.</summary>
        public const string Seo = "seo";

        /// <summary>Advanced tab while editing.</summary>
        public const string Advanced = "advanced";

        public const int ContentIndex = 0;
        public const int SeoIndex = 50;
        public const int AdvancedIndex = 100;
    }
}
