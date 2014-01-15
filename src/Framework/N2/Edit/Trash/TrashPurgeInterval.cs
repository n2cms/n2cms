namespace N2.Edit.Trash
{
    /// <summary>
    /// Specifies intervals after which deleted items can be purged from trash.
    /// </summary>
    public enum TrashPurgeInterval
    {
        /// <summary>Purge after a day.</summary>
        Dayly = 1,
        /// <summary>Purge after 7 days.</summary>
        Weekly = 7,
        /// <summary>Purge after 31 days.</summary>
        Monthly = 31,
        /// <summary>Purge after 92 days.</summary>
        Quarterly = 92,
        /// <summary>Purge after 365 days.</summary>
        Yearly = 365,
        /// <summary>Never purge</summary>
        Never = int.MaxValue
    }
}
