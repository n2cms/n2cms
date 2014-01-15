namespace N2.Integrity
{
    /// <summary>
    /// Determines which zones an item is allowed to exist in.
    /// </summary>
    public enum AllowedZones
    {
        /// <summary>The item may be added to a zone with any name.</summary>
        All,
        /// <summary>The item may be added to zones with a name but not without zones.</summary>
        AllNamed,
        /// <summary>Allowed in the specified zones. This is the default setting.</summary>
        SpecifiedZones,
        /// <summary>The item is not allowed to be placed in a zone.</summary>
        None
    }
}
