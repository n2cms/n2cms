using System;

namespace N2.Templates.Mvc
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
        
        /// <summary>The expandable details container.</summary>
        public const string Details = "details";

        /// <summary>Advanced tab while editing.</summary>
        public const string Advanced = "advanced";

        public const int ContentIndex = 0;
        public const int AdvancedIndex = 100;
    }
}
