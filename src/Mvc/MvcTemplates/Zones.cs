namespace N2.Templates.Mvc
{
    /// <summary>
    /// Provides access to some zone-related constants. Use these constants 
    /// instead of strings for better compile-time checking.
    /// </summary>
    public class Zones
    {
        /// <summary>Right column on whole site.</summary>
        public const string SiteRight = "SiteRight";
        /// <summary>Above content on whole site.</summary>
        public const string SiteTop = "SiteTop";
        /// <summary>Left of content on whole site.</summary>
        public const string SiteLeft = "SiteLeft";

        /// <summary>To the right on this and child pages.</summary>
        public const string RecursiveRight = "RecursiveRight";
        /// <summary>To the left on this and child pages.</summary>
        public const string RecursiveLeft = "RecursiveLeft";
        /// <summary>Above the content area on this and child pages.</summary>
        public const string RecursiveAbove = "RecursiveAbove";
        /// <summary>Below the content area on this and child pages.</summary>
        public const string RecursiveBelow = "RecursiveBelow";

        
        /// <summary>Right on this page.</summary>
        public const string Right = "Right";
        /// <summary>Left on this page</summary>
        public const string Left = "Left";

        /// <summary>On the left side of a two column container</summary>
        public const string ColumnLeft = "ColumnLeft";
        /// <summary>On the right side of a two column container</summary>
        public const string ColumnRight = "ColumnRight";

        /// <summary>In the content column (below text) on this page.</summary>
        public const string Content = "Content";

        /// <summary> Questions on FAQ pages </summary>
        public const string Questions = "Questions";
    }
}
